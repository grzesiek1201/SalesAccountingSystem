using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class QuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QuotationService> _logger;
        private readonly NumberSequenceService _numberSequenceService;
        private readonly OrderService _orderService;
        private readonly QuotationToOrderMapper _mapper;

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            NumberSequenceService numberSequenceService,
            OrderService orderService,
            QuotationToOrderMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _orderService = orderService;
            _mapper = mapper;
        }

        // ================= ADD =================

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            _logger.LogInformation("AddQuotation start. CustomerId: {CustomerId}", quotation.CustomerId);

            quotation.QuotationNumber =
                _numberSequenceService.GetNext(DocumentType.Quotation);

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

        public QuotationEditResult EditQuotation(Quotation input)
        {
            _logger.LogInformation("EditQuotation start. Id: {Id}", input.Id);

            var existing = _quotationRepository.GetById(input.Id);

            if (existing == null)
            {
                _logger.LogWarning("Quotation not found: {Id}", input.Id);
                return QuotationEditResult.NotFound;
            }

            if (existing.IsQuotationArchived)
            {
                _logger.LogWarning("Quotation archived: {Id}", input.Id);
                return QuotationEditResult.QuotationArchived;
            }

            if (input.CustomerId != 0)
                existing.CustomerId = input.CustomerId;

            if (input.Status != default)
                existing.Status = input.Status;

            if (input.Items != null && input.Items.Any())
                existing.Items = input.Items;

            var validation = _validator.Validate(
                existing,
                _quotationRepository.GetAll().Where(x => x.Id != existing.Id).ToList(),
                isEdit: true);

            if (!validation.IsValid)
            {
                _logger.LogWarning("EditQuotation invalid: {Id}", input.Id);
                return QuotationEditResult.InvalidData;
            }

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("EditQuotation success: {Id}", input.Id);

            return QuotationEditResult.Success;
        }

        // ================= CONVERT =================

        public ConvertQuotationResult ConvertToOrder(int id)
        {
            _logger.LogInformation("ConvertToOrder start. Id: {Id}", id);

            var quotation = _quotationRepository.GetById(id);

            if (quotation == null)
            {
                _logger.LogWarning("Quotation not found: {Id}", id);
                return ConvertQuotationResult.NotFound;
            }

            if (quotation.IsQuotationArchived)
            {
                _logger.LogWarning("Quotation archived: {Id}", id);
                return ConvertQuotationResult.InvalidData;
            }

            if (quotation.Status != QuotationStatus.Accepted)
            {
                _logger.LogWarning("Quotation not accepted: {Id}", id);
                return ConvertQuotationResult.InvalidData;
            }

            _logger.LogInformation("Mapping quotation -> order: {Id}", id);

            var order = _mapper.Map(quotation);

            var result = _orderService.AddOrder(order);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order creation failed: {Id}", id);
                return ConvertQuotationResult.InvalidData;
            }

            quotation.Status = QuotationStatus.ConvertedToOrder;

            _quotationRepository.Update(quotation);
            _unitOfWork.Save();

            _logger.LogInformation("Convert success: {Id}", id);

            return ConvertQuotationResult.Success;
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

        public ArchiveQuotationResult ArchiveQuotation(int id)
        {
            _logger.LogInformation("ArchiveQuotation {Id}", id);

            var existing = _quotationRepository.GetById(id);

            if (existing == null)
                return ArchiveQuotationResult.NotFound;

            existing.IsQuotationArchived = true;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation archived: {QuotationId}", id);

            return ArchiveQuotationResult.Success;
        }
    }
}