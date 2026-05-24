using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Application.Services
{
    internal class QuotationService
    {
        private List<Quotation> quotations = new List<Quotation>();
        public int nextId;
        private readonly QuotationValidator _validator;

        public QuotationService(QuotationValidator validator)
        {
            _validator = validator;
        }

        public QuotationAddResponse AddQuotation(Quotation quotation)
        {
            var result = _validator.Validate(quotation, quotations);

            if (!result.IsValid)
            {
                return new QuotationAddResponse
                {
                    Result = QuotationAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            quotation.Id = nextId;
            nextId++;

            quotations.Add(quotation);

            return new QuotationAddResponse
            {
                Result = QuotationAddResult.Success
            };
        }

        public Domain.Enums.QuotationEditResult EditQuotation(Quotation quotation)
        {
            var existing = quotations.Find(x => x.Id == quotation.Id);

            if (existing == null)
                return Domain.Enums.QuotationEditResult.NotFound;

            if (existing.IsQuotationArchived)
                return Domain.Enums.QuotationEditResult.QuotationArchived;

            var otherQuotations = quotations.Where(x => x.Id != quotation.Id).ToList();
            var result = _validator.Validate(quotation, otherQuotations);

            if (!result.IsValid)
                return Domain.Enums.QuotationEditResult.InvalidData;

            existing.Status = quotation.Status;
            existing.Customer = quotation.Customer;

            return Domain.Enums.QuotationEditResult.Success;
        }

        public List<Quotation> GetAllQuotations()
        {
            return quotations;
        }

        public Quotation FindQuotation(int Id)
        {
            return quotations.FirstOrDefault(x => x.Id == Id);
        }

        public Domain.Enums.ArchiveQuotationResult ArchiveQuotation(int Id)
        {
            var existing = quotations.Find(x => x.Id == Id);
            if (existing == null)
            {
                return Domain.Enums.ArchiveQuotationResult.NotFound;
            }
            existing.IsQuotationArchived = true;
            return Domain.Enums.ArchiveQuotationResult.Success;
        }
    }
}
