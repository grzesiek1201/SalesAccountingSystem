using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
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
        private readonly NumberSequenceService _numberSequenceService;
        private readonly OrderToInvoiceMapper _mapper;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            InvoiceValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<InvoiceService> logger,
            NumberSequenceService numberSequenceService,
            OrderToInvoiceMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _numberSequenceService = numberSequenceService;
            _mapper = mapper;
        }

        // ================= ADD =================

        public InvoiceAddResponse AddInvoice(Invoice invoice)
        {
            _logger.LogInformation("AddInvoice start. CustomerId: {CustomerId}", invoice.Customer?.Id);

            invoice.InvoiceNumber =
                _numberSequenceService.GetNext(DocumentType.Invoice);

            var validation = _validator.Validate(
                invoice,
                _invoiceRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning("AddInvoice validation failed. Errors: {Errors}", validation.Errors);

                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
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

        // ================= CREATE FROM ORDER =================

        public InvoiceAddResponse CreateFromOrder(Order order)
        {
            _logger.LogInformation("CreateFromOrder start. OrderId: {OrderId}", order?.Id);

            if (order == null)
            {
                _logger.LogWarning("Order is null");
                return new InvoiceAddResponse { Result = InvoiceAddResult.InvalidData };
            }

            var invoice = _mapper.Map(order);

            if (order == null)
            {
                _logger.LogWarning("Mapper returned null for OrderId: {OrderId}", order.Id);
                return new InvoiceAddResponse { Result = InvoiceAddResult.InvalidData };
            }

            _logger.LogInformation(
                "Mapped order to order. CustomerId: {CustomerId}, Items: {Count}",
                invoice.CustomerId,
                invoice.Items?.Count ?? 0);
                invoice.InvoiceNumber =
                _numberSequenceService.GetNext(DocumentType.Invoice);

            var validation = _validator.Validate(invoice, _invoiceRepository.GetAll());

            if (!validation.IsValid)
            {
                _logger.LogWarning(
                    "Invoice from order invalid. OrderId: {OrderId}, Errors: {Errors}",
                    order.Id,
                    validation.Errors);

                return new InvoiceAddResponse
                {
                    Result = InvoiceAddResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _invoiceRepository.Add(invoice);
            _unitOfWork.Save();

            _logger.LogInformation(
                "Invoice created from order SUCCESS. InvoiceId: {InvoiceId}",
                order.Id);

            return new InvoiceAddResponse
            {
                Result = InvoiceAddResult.Success
            };
        }

        // ================= EDIT =================

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

            var validation = _validator.Validate(invoice,
                _invoiceRepository.GetAll().Where(x => x.Id != invoice.Id).ToList());

            if (!validation.IsValid)
            {
                _logger.LogWarning("EditInvoice validation failed. Id: {InvoiceId}, Errors: {Errors}",
                    invoice.Id, validation.Errors);

                return InvoiceEditResult.InvalidData;
            }

            existing.Status = invoice.Status;
            existing.Customer = invoice.Customer;

            _invoiceRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Invoice edited successfully. Id: {InvoiceId}", invoice.Id);

            return InvoiceEditResult.Success;
        }

        // ================= STATUS =================

        public InvoiceStatusResult ChangeInvoiceStatus(int id, InvoiceStatus newStatus)
        {
            _logger.LogInformation("ChangeStatus {Id} -> {Status}", id, newStatus);

            var invoice = _invoiceRepository.GetById(id);

            if (invoice == null)
                return InvoiceStatusResult.NotFound;

            if (invoice.IsInvoiceArchived)
                return InvoiceStatusResult.InvalidOperation;

            invoice.Status = newStatus;

            _invoiceRepository.Update(invoice);
            _unitOfWork.Save();

            return InvoiceStatusResult.Success;
        }

        // ================= READ =================

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

        // ================= ARCHIVE =================

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