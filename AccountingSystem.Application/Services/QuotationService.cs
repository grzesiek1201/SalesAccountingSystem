using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
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

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<QuotationService> logger)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            _logger.LogInformation("Starting AddQuotation for CustomerId: {CustomerId}", quotation.Customer?.Id);

            var quotations = _quotationRepository.GetAll();

            var result = _validator.Validate(quotation, quotations);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddQuotation validation failed. Errors: {Errors}", result.Errors);

                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _quotationRepository.Add(quotation);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation added successfully. QuotationId: {QuotationId}", quotation.Id);

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }

        public QuotationEditResult EditQuotation(Quotation quotation)
        {
            _logger.LogInformation("Starting EditQuotation. QuotationId: {QuotationId}", quotation.Id);

            var existing = _quotationRepository.GetById(quotation.Id);

            if (existing == null)
            {
                _logger.LogWarning("Quotation not found. Id: {QuotationId}", quotation.Id);
                return QuotationEditResult.NotFound;
            }

            if (existing.IsQuotationArchived)
            {
                _logger.LogWarning("Attempt to edit archived quotation. Id: {QuotationId}", quotation.Id);
                return QuotationEditResult.QuotationArchived;
            }

            var otherQuotations = (_quotationRepository.GetAll() ?? new List<Quotation>())
                .Where(x => x.Id != quotation.Id)
                .ToList();

            var result = _validator.Validate(quotation, otherQuotations);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditQuotation validation failed. Id: {QuotationId}, Errors: {Errors}",
                    quotation.Id, result.Errors);

                return QuotationEditResult.InvalidData;
            }

            existing.Status = quotation.Status;
            existing.Customer = quotation.Customer;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation edited successfully. Id: {QuotationId}", quotation.Id);

            return QuotationEditResult.Success;
        }

        public List<Quotation> GetAllQuotations()
        {
            _logger.LogInformation("Fetching all quotations");

            return _quotationRepository.GetAll();
        }

        public Quotation? FindQuotation(int id)
        {
            _logger.LogInformation("Finding quotation by Id: {QuotationId}", id);

            return _quotationRepository.GetById(id);
        }

        public ArchiveQuotationResult ArchiveQuotation(int id)
        {
            _logger.LogInformation("Archiving quotation. Id: {QuotationId}", id);

            var existing = _quotationRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Quotation not found for archive. Id: {QuotationId}", id);
                return ArchiveQuotationResult.NotFound;
            }

            existing.IsQuotationArchived = true;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Quotation archived successfully. Id: {QuotationId}", id);

            return ArchiveQuotationResult.Success;
        }
    }
}