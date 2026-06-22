using AccountingSystem.Application.DTOs.Quotations;
using AccountingSystem.Application.Helpers.Snapshots;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class QuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationService> _logger;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            NumberSequenceService numberSequenceService,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        // ================= ADD =================

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            _logger.LogInformation("AddQuotation start. CustomerId: {CustomerId}", quotation.CustomerId);

            var customer = _customerRepository.GetById(quotation.CustomerId);
            if (customer == null)
                return new QuotationAddResponse { Result = QuotationAddResult.InvalidData };

            var productIds = quotation.Items?
                .Select(i => i.ProductId)
                .ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            quotation.QuotationNumber =
                _numberSequenceService.GetNext(DocumentType.Quotation);

            quotation.ApplyCustomerSnapshot(customer);

            quotation.Items = ItemSnapshotHelper.SnapshotQuotationItems(
                quotation.Items,
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

        public QuotationEditResponse EditQuotation(Quotation input)
        {
            _logger.LogInformation("EditQuotation start. Id: {Id}", input.Id);

            var existing = _quotationRepository.GetById(input.Id);

            if (existing == null)
                return new QuotationEditResponse { Result = QuotationEditResult.NotFound };

            if (existing.IsQuotationArchived)
                return new QuotationEditResponse { Result = QuotationEditResult.QuotationArchived };

            var productIds = input.Items?.Select(i => i.ProductId).ToList() ?? new List<int>();

            var products = _productRepository
                .GetByIds(productIds)
                .ToDictionary(p => p.Id);

            if (input.Items != null && input.Items.Any())
            {
                var itemProductIds = input.Items.Select(i => i.ProductId).ToList();

                var productDict = _productRepository
                    .GetByIds(productIds)
                    .ToDictionary(p => p.Id);

                existing.Items = ItemSnapshotHelper.SnapshotQuotationItems(
                    input.Items,
                    products);
            }

            if (input.CustomerId != 0 && input.CustomerId != existing.CustomerId)
            {
                var customer = _customerRepository.GetById(input.CustomerId);
                if (customer == null)
                    return new QuotationEditResponse { Result = QuotationEditResult.InvalidData };

                existing.ApplyCustomerSnapshot(customer);
            }

            if (input.Status != default)
                existing.Status = input.Status;

            var validation = _validator.Validate(
                existing,
                _quotationRepository.GetAll().Where(x => x.Id != existing.Id).ToList(),
                isEdit: true);

            if (!validation.IsValid)
                return new QuotationEditResponse { Result = QuotationEditResult.InvalidData };

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return new QuotationEditResponse
            {
                Result = QuotationEditResult.Success
            };
        }

        // ================= STATUS =================

        public QuotationStatusResult ChangeQuotationStatus(int id, QuotationStatus newStatus)
        {
            _logger.LogInformation("ChangeStatus {Id} -> {Status}", id, newStatus);

            var quotation = _quotationRepository.GetById(id);

            if (quotation == null)
                return QuotationStatusResult.NotFound;

            if (quotation.IsQuotationArchived)
                return QuotationStatusResult.InvalidOperation;

            quotation.Status = newStatus;

            _quotationRepository.Update(quotation);
            _unitOfWork.Save();

            return QuotationStatusResult.Success;
        }

        // ================= READ =================

        public List<Quotation> GetAllQuotations()
        {
            _logger.LogInformation("GetAllQuotations");
            return _quotationRepository.GetAll();
        }

        public Quotation? FindQuotation(int id)
        {
            _logger.LogInformation("FindQuotation: {QuotationId}", id);
            return _quotationRepository.GetById(id);
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