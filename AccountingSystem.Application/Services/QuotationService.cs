using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Helpers.Snapshots;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using AccountingSystem.Application.Mappers;

namespace AccountingSystem.Application.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationService> _logger;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly QuotationResponseMapper _mapper;

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            INumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            QuotationResponseMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // ================= ADD =================

        public QuotationAddResponse AddQuotation(CreateQuotationRequest request)
        {
            _logger.LogInformation("AddQuotation start. CustomerId: {CustomerId}", request.CustomerId);

            var customer = _customerRepository.GetById(request.CustomerId);
            if (customer == null)
                return new QuotationAddResponse { Result = QuotationAddResult.InvalidData };

            var productIds = request.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            var quotation = new Quotation
            {
                CustomerId = request.CustomerId,
                Status = QuotationStatus.Draft,
                DateCreated = DateTime.UtcNow,
                QuotationNumber = _numberSequenceService.GetNext(DocumentType.Quotation)
            };

            quotation.ApplyCustomerSnapshot(customer);

            var domainItems = request.Items?
                .Select(x => new QuotationItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DiscountPercent = x.DiscountPercent
                })
                .ToList() ?? new List<QuotationItem>();

            quotation.Items = ItemSnapshotHelper.SnapshotQuotationItems(
                domainItems,
                products);

            var validation = _validator.Validate(
                quotation,
                _quotationRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning("AddQuotation invalid: {Errors}", validation.Errors);

                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _quotationRepository.Add(quotation);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation created: {Id}", quotation.Id);

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }

        // ================= EDIT =================

        public QuotationEditResponse EditQuotation(UpdateQuotationRequest request)
        {
            _logger.LogInformation("EditQuotation start. Id: {Id}", request.Id);

            var existing = _quotationRepository.GetById(request.Id);

            if (existing == null)
                return new QuotationEditResponse { Result = QuotationEditResult.NotFound };

            if (existing.IsQuotationArchived)
                return new QuotationEditResponse { Result = QuotationEditResult.QuotationArchived };

            if (request.Items != null && request.Items.Any())
            {
                var domainItems = request.Items
                    .Select(x => new QuotationItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        DiscountPercent = x.DiscountPercent
                    })
                    .ToList();

                var productIds = domainItems.Select(i => i.ProductId).ToList();

                var products = _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);

                existing.Items = ItemSnapshotHelper.SnapshotQuotationItems(
                    domainItems,
                    products);
            }

            if (request.CustomerId != 0 && request.CustomerId != existing.CustomerId)
            {
                var customer = _customerRepository.GetById(request.CustomerId);

                if (customer == null)
                    return new QuotationEditResponse { Result = QuotationEditResult.InvalidData };

                existing.ApplyCustomerSnapshot(customer);
            }

            if (request.Status != default)
                existing.Status = request.Status;

            var validation = _validator.Validate(
                existing,
                _quotationRepository.GetAll().Where(x => x.Id != existing.Id).ToList(),
                isEdit: true);

            if (!validation.IsValid)
                return new QuotationEditResponse
                {
                    Result = QuotationEditResult.InvalidData,
                    Errors = validation.Errors
                };

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return new QuotationEditResponse
            {
                Result = QuotationEditResult.Success
            };
        }

        // ================= STATUS =================

        public QuotationStatusResponse ChangeQuotationStatus(int id, StatusQuotationRequest request)
        {
            _logger.LogInformation("ChangeStatus {Id} -> {Status}", id, request);

            var quotation = _quotationRepository.GetById(id);

            if (quotation == null)
                return new QuotationStatusResponse { Result = QuotationStatusResult.NotFound };

            if (quotation.IsQuotationArchived)
                return new QuotationStatusResponse { Result = QuotationStatusResult.InvalidOperation };

            quotation.Status = request.Status;

            _quotationRepository.Update(quotation);
            _unitOfWork.Save();

            return new QuotationStatusResponse { Result = QuotationStatusResult.Success };
        }

        // ================= READ =================

        public List<QuotationResponse> GetAllQuotations()
        {
            _logger.LogInformation("GetAllQuotations");

            return _quotationRepository.GetAll()
                .Select(q => _mapper.Map(q))
                .ToList();
        }

        public QuotationResponse? FindQuotation(int id)
        {
            _logger.LogInformation("FindQuotation: {QuotationId}", id);

            var quotation = _quotationRepository.GetById(id);

            if (quotation == null)
                return null;

            return _mapper.Map(quotation);
        }

        // ================= ARCHIVE =================

        public QuotationArchiveResult ArchiveQuotation(int id)
        {
            _logger.LogInformation("ArchiveQuotation {Id}", id);

            var existing = _quotationRepository.GetById(id);

            if (existing == null)
                return QuotationArchiveResult.NotFound;

            existing.IsQuotationArchived = true;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation archived: {QuotationId}", id);

            return QuotationArchiveResult.Success;
        }
    }
}