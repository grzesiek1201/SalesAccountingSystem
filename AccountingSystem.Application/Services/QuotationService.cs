using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class QuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public QuotationService(
            IQuotationRepository quotationRepository,
            QuotationValidator validator,
            IUnitOfWork unitOfWork)
        {
            _quotationRepository = quotationRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            var quotations = _quotationRepository.GetAll();

            var result = _validator.Validate(quotation, quotations);

            if (!result.IsValid)
            {
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

        public QuotationEditResult EditQuotation(Quotation quotation)
        {
            var existing = _quotationRepository.GetById(quotation.Id);

            if (existing == null)
                return QuotationEditResult.NotFound;

            if (existing.IsQuotationArchived)
                return QuotationEditResult.QuotationArchived;

            var otherQuotations = (_quotationRepository.GetAll() ?? new List<Quotation>())
                .Where(x => x.Id != quotation.Id)
                .ToList();

            var result = _validator.Validate(quotation, otherQuotations);

            if (!result.IsValid)
                return QuotationEditResult.InvalidData;

            existing.Status = quotation.Status;
            existing.Customer = quotation.Customer;

            _quotationRepository.Update(existing);
            _unitOfWork.Save();

            return QuotationEditResult.Success;
        }

        public List<Quotation> GetAllQuotations()
        {
            return _quotationRepository.GetAll();
        }

        public Quotation? FindQuotation(int id)
        {
            return _quotationRepository.GetById(id);
        }

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
    }
}