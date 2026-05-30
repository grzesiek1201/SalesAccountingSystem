using AccountingSystem.AccountingSystem.Application.DTOs;
using AccountingSystem.AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.AccountingSystem.Domain.Entities;
using AccountingSystem.AccountingSystem.Domain.Enums;
using AccountingSystem.AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.AccountingSystem.Application.Services
{
    internal class InvoiceService
    {
        private readonly AppDbContext _context;
        private readonly InvoiceValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(
            AppDbContext context,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _validator = validator;
            _unitOfWork = unitOfWork;
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

            _unitOfWork.Save();

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

            _unitOfWork.Save();

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
                return ArchiveInvoiceResult.NotFound;

            existing.IsInvoiceArchived = true;

            _unitOfWork.Save();

            return ArchiveInvoiceResult.Success;
        }
    }
}