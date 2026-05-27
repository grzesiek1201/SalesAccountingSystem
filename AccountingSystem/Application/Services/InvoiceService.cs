using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;
using AccountingSystem.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    internal class InvoiceService
    {
        private readonly AppDbContext _context;
        private readonly InvoiceValidator _validator;

        public InvoiceService(AppDbContext context, InvoiceValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public InvoiceAddResponse AddInvoice(Invoice invoice)
        {
            var invoices = _context.Invoices.ToList();

            var result = _validator.Validate(invoice, invoices);

            if (!result.IsValid)
            {
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return new InvoiceAddResponse
            {
                Result = InvoiceAddResult.Success
            };
        }

        public InvoiceEditResult EditInvoice(Invoice invoice)
        {
            var existing = _context.Invoices
                .FirstOrDefault(x => x.Id == invoice.Id);

            if (existing == null)
                return InvoiceEditResult.NotFound;

            if (existing.IsInvoiceArchived)
                return InvoiceEditResult.InvoiceArchived;

            var otherInvoices = _context.Invoices
                .Where(x => x.Id != invoice.Id)
                .ToList();

            var result = _validator.Validate(invoice, otherInvoices);

            if (!result.IsValid)
                return InvoiceEditResult.InvalidData;

            existing.Status = invoice.Status;
            existing.Customer = invoice.Customer;

            _context.SaveChanges();

            return InvoiceEditResult.Success;
        }

        public List<Invoice> GetAllInvoices()
        {
            return _context.Invoices.ToList();
        }

        public Invoice FindInvoice(int id)
        {
            return _context.Invoices
                .FirstOrDefault(x => x.Id == id);
        }

        public ArchiveInvoiceResult ArchiveInvoice(int id)
        {
            var existing = _context.Invoices
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
            {
                return ArchiveInvoiceResult.NotFound;
            }

            existing.IsInvoiceArchived = true;

            _context.SaveChanges();

            return ArchiveInvoiceResult.Success;
        }
    }
}

