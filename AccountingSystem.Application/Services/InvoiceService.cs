using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly InvoiceValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public InvoiceAddResponse AddInvoice(Invoice invoice)
        {
            var invoices = _invoiceRepository.GetAll();

            var result = _validator.Validate(invoice, invoices);

            if (!result.IsValid)
            {
                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();

            return new InvoiceAddResponse
            {
                Result = InvoiceAddResult.Success
            };
        }

        public InvoiceEditResult EditInvoice(Invoice invoice)
        {
            var existing = _invoiceRepository.GetById(invoice.Id);

            if (existing == null)
                return InvoiceEditResult.NotFound;

            if (existing.IsInvoiceArchived)
                return InvoiceEditResult.InvoiceArchived;

            var otherInvoices = _invoiceRepository.GetAll()?.ToList() ?? new List<Invoice>();

            var result = _validator.Validate(invoice, otherInvoices);

            if (!result.IsValid)
                return InvoiceEditResult.InvalidData;

            existing.Status = invoice.Status;
            existing.Customer = invoice.Customer;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            return InvoiceEditResult.Success;
        }

        public List<Invoice> GetAllInvoices()
        {
            return _invoiceRepository.GetAll();
        }

        public Invoice? FindInvoice(int id)
        {
            return _invoiceRepository.GetById(id);
        }

        public ArchiveInvoiceResult ArchiveInvoice(int id)
        {
            var existing = _invoiceRepository.GetById(id);

            if (existing == null)
                return ArchiveInvoiceResult.NotFound;

            existing.IsInvoiceArchived = true;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            return ArchiveInvoiceResult.Success;
        }

        public PaymentAddResult AddPayment(int invoiceId, Payment payment)
        {
            var invoice = _invoiceRepository.GetById(invoiceId);

            if (invoice == null)
                return PaymentAddResult.InvoiceNotFound;

            if (invoice.IsInvoiceArchived)
                return PaymentAddResult.InvoiceArchived;

            if (payment.Amount <= 0)
                return PaymentAddResult.InvalidAmount;

            var totalPaid = invoice.Payments.Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - totalPaid;

            if (payment.Amount > remaining)
                return PaymentAddResult.AmountExceedsRemaining;

            payment.InvoiceId = invoiceId;
            payment.PaymentDate = DateTime.Now;

            invoice.Payments.Add(payment);

            UpdateInvoicePaymentState(invoice);

            _unitOfWork.Save();

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