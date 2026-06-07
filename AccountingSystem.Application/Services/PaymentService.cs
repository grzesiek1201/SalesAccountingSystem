using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
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

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork,
            InvoiceService invoiceService)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            _invoiceService = invoiceService;
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

            var alreadyPaid = invoice.Payments.Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - alreadyPaid;

            if (payment.Amount > remaining)
                return PaymentAddResult.AmountExceedsRemaining;

            payment.InvoiceId = invoiceId;
            payment.PaymentDate = DateTime.Now;
            payment.Status = PaymentStatus.Completed;

            _paymentRepository.Add(payment);

            invoice.Payments.Add(payment);

            _invoiceService.RecalculateInvoiceStatus(invoice);

            _unitOfWork.Save();

            return PaymentAddResult.Success;
        }

        public IQueryable<Payment> GetPaymentsForInvoice(int invoiceId)
        {
            return _paymentRepository
                .GetByInvoiceId(invoiceId)
                .AsQueryable();
        }

        public void DeletePayment(int paymentId)
        {
            var payment = _paymentRepository.GetById(paymentId);

            if (payment == null)
                return;

            var invoice = _invoiceRepository.GetById(payment.InvoiceId);

            _paymentRepository.Delete(payment);

            _invoiceService.RecalculateInvoiceStatus(invoice);

            _unitOfWork.Save();
        }
    }
}