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
        private readonly QuotationToOrderMapper _mapper;
        private readonly NumberSequenceService _numberSequenceService;

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger,
            QuotationToOrderMapper mapper)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // ================= CONVERT =================

        public ConvertQuotationResult ConvertToOrder(int id)
        {
            _logger.LogInformation("Converting quotation. Id: {QuotationId}", id);

            var existing = _quotationRepository.GetById(id);

            if (existing == null)
                return ConvertQuotationResult.NotFound;

            if (existing.IsQuotationArchived)
                return ConvertQuotationResult.InvalidData;

            if (existing.Status != QuotationStatus.Accepted)
                return ConvertQuotationResult.InvalidData;

            if (existing.Status == QuotationStatus.ConvertedToOrder)
                return ConvertQuotationResult.InvalidData;

            existing.Status = QuotationStatus.ConvertedToOrder;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation converted successfully. Id: {QuotationId}", id);

            return ConvertQuotationResult.Success;
        }

        // ================= ADD =================

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            _logger.LogInformation("AddQuotation. CustomerId: {CustomerId}", quotation.CustomerId);

            var quotations = _quotationRepository.GetAll();
            var result = _validator.Validate(quotation, quotations);

            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed");
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _quotationRepository.Add(quotation);
            _unitOfWork.Save();

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }

        // ================= EDIT =================

        public QuotationEditResult EditQuotation(Quotation quotation)
        {
            var existing = _quotationRepository.GetById(quotation.Id);

            if (existing == null)
                return QuotationEditResult.NotFound;

            if (existing.IsQuotationArchived)
                return QuotationEditResult.QuotationArchived;

            var other = _quotationRepository.GetAll()
                .Where(x => x.Id != quotation.Id)
                .ToList();

            var result = _validator.Validate(quotation, other);

            if (!result.IsValid)
                return QuotationEditResult.InvalidData;

            existing.Status = quotation.Status;
            existing.Customer = quotation.Customer;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return QuotationEditResult.Success;
        }

        // ================= READ =================

        public List<Quotation> GetAllQuotations()
            => _quotationRepository.GetAll();

        public Quotation? FindQuotation(int id)
            => _quotationRepository.GetById(id);

        // ================= ARCHIVE =================

        public ArchiveQuotationResult ArchiveQuotation(int id)
        {
            var existing = _quotationRepository.GetById(id);

            if (existing == null)
                return ArchiveQuotationResult.NotFound;

            existing.IsQuotationArchived = true;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return ArchiveQuotationResult.Success;
        }

        // ================= DOMAIN FLOW (RECOMMENDED PLACE) =================

        public Order CreateOrderFromQuotation(Quotation quotation)
        {
            return _mapper.Map(quotation);
        }
    }
}