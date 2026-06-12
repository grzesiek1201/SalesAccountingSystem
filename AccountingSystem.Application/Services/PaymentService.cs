using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly InvoiceService _invoiceService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork,
            InvoiceService invoiceService,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            _invoiceService = invoiceService;
            _logger = logger;
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
                _logger.LogWarning("Invoice archived. Id: {InvoiceId}", invoiceId);
                return PaymentAddResult.InvoiceArchived;
            }

            if (payment.Amount <= 0)
            {
                _logger.LogWarning("Invalid payment amount: {Amount}", payment.Amount);
                return PaymentAddResult.InvalidAmount;
            }

            var alreadyPaid = invoice.Payments.Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - alreadyPaid;

            if (payment.Amount > remaining)
            {
                _logger.LogWarning("Payment exceeds remaining. Remaining: {Remaining}, Amount: {Amount}",
                    remaining, payment.Amount);

                return PaymentAddResult.AmountExceedsRemaining;
            }

            payment.InvoiceId = invoiceId;
            payment.PaymentDate = DateTime.Now;
            payment.Status = PaymentStatus.Completed;

            _paymentRepository.Add(payment);
            invoice.Payments.Add(payment);

            _invoiceService.RecalculateInvoiceStatus(invoice);

            _unitOfWork.Save();

            _logger.LogInformation("Payment added successfully. InvoiceId: {InvoiceId}", invoiceId);

            return PaymentAddResult.Success;
        }

        public IQueryable<Payment> GetPaymentsForInvoice(int invoiceId)
        {
            _logger.LogInformation("Fetching payments for invoice. Id: {InvoiceId}", invoiceId);

            return _paymentRepository
                .GetByInvoiceId(invoiceId)
                .AsQueryable();
        }

        public void DeletePayment(int paymentId)
        {
            _logger.LogInformation("Deleting payment. Id: {PaymentId}", paymentId);

            var payment = _paymentRepository.GetById(paymentId);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found. Id: {PaymentId}", paymentId);
                return;
            }

            var invoice = _invoiceRepository.GetById(payment.InvoiceId);

            _paymentRepository.Delete(payment);

            _invoiceService.RecalculateInvoiceStatus(invoice);

            _unitOfWork.Save();

            _logger.LogInformation("Payment deleted successfully. Id: {PaymentId}", paymentId);
        }
    }
}