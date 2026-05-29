using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;
using AccountingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    internal class QuotationService
    {
        private readonly AppDbContext _context;
        private readonly QuotationValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public QuotationService(
            AppDbContext context,
            QuotationValidator validator,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            var quotations = _context.Quotations.ToList();

            var result = _validator.Validate(quotation, quotations);

            if (!result.IsValid)
            {
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _context.Quotations.Add(quotation);

            _unitOfWork.Save();

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }

        public QuotationEditResult EditQuotation(Quotation quotation)
        {
            var existing = _context.Quotations
                .FirstOrDefault(x => x.Id == quotation.Id);

            if (existing == null)
                return QuotationEditResult.NotFound;

            if (existing.IsQuotationArchived)
                return QuotationEditResult.QuotationArchived;

            var otherQuotations = _context.Quotations
                .Where(x => x.Id != quotation.Id)
                .ToList();

            var result = _validator.Validate(quotation, otherQuotations);

            if (!result.IsValid)
                return QuotationEditResult.InvalidData;

            existing.Status = quotation.Status;
            existing.Customer = quotation.Customer;

            _unitOfWork.Save();

            return QuotationEditResult.Success;
        }

        public List<Quotation> GetAllQuotations()
        {
            return _context.Quotations.ToList();
        }

        public Quotation FindQuotation(int id)
        {
            return _context.Quotations
                .FirstOrDefault(x => x.Id == id);
        }

        public ArchiveQuotationResult ArchiveQuotation(int id)
        {
            var existing = _context.Quotations
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return ArchiveQuotationResult.NotFound;

            existing.IsQuotationArchived = true;

            _unitOfWork.Save();

            return ArchiveQuotationResult.Success;
        }
    }
}