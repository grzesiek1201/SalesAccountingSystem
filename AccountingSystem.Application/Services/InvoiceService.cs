using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly InvoiceValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public InvoiceAddResponse AddInvoice(Invoice invoice)
        {
            _logger.LogInformation("Starting AddInvoice. CustomerId: {CustomerId}", invoice.Customer?.Id);

            var invoices = _invoiceRepository.GetAll();
            var result = _validator.Validate(invoice, invoices);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddInvoice validation failed. Errors: {Errors}", result.Errors);

                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();

            _logger.LogInformation("Invoice added successfully. InvoiceId: {InvoiceId}", invoice.Id);

            return new InvoiceAddResponse
            {
                Result = InvoiceAddResult.Success
            };
        }

        public InvoiceEditResult EditInvoice(Invoice invoice)
        {
            _logger.LogInformation("Starting EditInvoice. InvoiceId: {InvoiceId}", invoice.Id);

            var existing = _invoiceRepository.GetById(invoice.Id);

            if (existing == null)
            {
                _logger.LogWarning("Invoice not found. Id: {InvoiceId}", invoice.Id);
                return InvoiceEditResult.NotFound;
            }

            if (existing.IsInvoiceArchived)
            {
                _logger.LogWarning("Attempt to edit archived invoice. Id: {InvoiceId}", invoice.Id);
                return InvoiceEditResult.InvoiceArchived;
            }

            var otherInvoices = _invoiceRepository.GetAll()?.ToList() ?? new List<Invoice>();
            var result = _validator.Validate(invoice, otherInvoices);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditInvoice validation failed. Id: {InvoiceId}, Errors: {Errors}",
                    invoice.Id, result.Errors);

                return InvoiceEditResult.InvalidData;
            }

            existing.Status = invoice.Status;
            existing.Customer = invoice.Customer;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Invoice edited successfully. Id: {InvoiceId}", invoice.Id);

            return InvoiceEditResult.Success;
        }

        public List<Invoice> GetAllInvoices()
        {
            _logger.LogInformation("Fetching all invoices");
            return _invoiceRepository.GetAll();
        }

        public Invoice? FindInvoice(int id)
        {
            _logger.LogInformation("Finding invoice by Id: {InvoiceId}", id);
            return _invoiceRepository.GetById(id);
        }

        public ArchiveInvoiceResult ArchiveInvoice(int id)
        {
            _logger.LogInformation("Archiving invoice. Id: {InvoiceId}", id);

            var existing = _invoiceRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Invoice not found for archive. Id: {InvoiceId}", id);
                return ArchiveInvoiceResult.NotFound;
            }

            existing.IsInvoiceArchived = true;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Invoice archived successfully. Id: {InvoiceId}", id);

            return ArchiveInvoiceResult.Success;
        }

        public PaymentAddResult AddPayment(int invoiceId, Payment payment)
        {
            _logger.LogInformation("Starting AddPayment. InvoiceId: {InvoiceId}, Amount: {Amount}",
                invoiceId, payment.Amount);

            var invoice = _invoiceRepository.GetById(invoiceId);

            if (invoice == null)
            {
                _logger.LogWarning("Invoice not found. Id: {InvoiceId}", invoiceId);
                return PaymentAddResult.InvoiceNotFound;
            }

            if (invoice.IsInvoiceArchived)
            {
                _logger.LogWarning("Attempt to add payment to archived invoice. Id: {InvoiceId}", invoiceId);
                return PaymentAddResult.InvoiceArchived;
            }

            if (payment.Amount <= 0)
            {
                _logger.LogWarning("Invalid payment amount: {Amount}", payment.Amount);
                return PaymentAddResult.InvalidAmount;
            }

            var totalPaid = invoice.Payments.Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - totalPaid;

            if (payment.Amount > remaining)
            {
                _logger.LogWarning("Payment exceeds remaining amount. Remaining: {Remaining}, Attempted: {Amount}",
                    remaining, payment.Amount);

                return PaymentAddResult.AmountExceedsRemaining;
            }

            payment.InvoiceId = invoiceId;
            payment.PaymentDate = DateTime.Now;

            invoice.Payments.Add(payment);

            UpdateInvoicePaymentState(invoice);

            _unitOfWork.Save();

            _logger.LogInformation("Payment added successfully. InvoiceId: {InvoiceId}, Amount: {Amount}",
                invoiceId, payment.Amount);

            return PaymentAddResult.Success;
        }

        private void UpdateInvoicePaymentState(Invoice invoice)
        {
            var totalPaid = invoice.Payments.Sum(p => p.Amount);

            if (totalPaid <= 0)
            {
                invoice.Status = InvoiceStatus.Unpaid;
                invoice.PaidDate = null;
                return;
            }

            if (totalPaid < invoice.TotalAmount)
            {
                invoice.Status = InvoiceStatus.PartiallyPaid;
                invoice.PaidDate = null;
                return;
            }

            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidDate = DateTime.Now;
        }

        public void RecalculateInvoiceStatus(Invoice invoice)
        {
            var paid = invoice.Payments.Sum(p => p.Amount);

            if (paid <= 0)
                invoice.Status = InvoiceStatus.Issued;
            else if (paid < invoice.TotalAmount)
                invoice.Status = InvoiceStatus.PartiallyPaid;
            else
                invoice.Status = InvoiceStatus.Paid;

            if (invoice.DueDate < DateTime.Now && invoice.Status != InvoiceStatus.Paid)
                invoice.Status = InvoiceStatus.Overdue;
        }
    }
}