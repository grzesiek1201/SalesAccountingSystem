using AccountingSystem.Application.DTOs.Invoices;
using AccountingSystem.Application.DTOs.Payments;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly IInvoiceStatusCalculator _statusCalculator;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            IInvoiceStatusCalculator statusCalculator)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _statusCalculator = statusCalculator;
        }

        // ================= CREATE =================

        public PaymentAddResponse AddPayment(CreatePaymentRequest request)
        {
            _logger.LogInformation(
                "AddPayment InvoiceId={InvoiceId}, Amount={Amount}",
                request.InvoiceId,
                request.Amount);

            var invoice = _invoiceRepository.GetById(request.InvoiceId);

            if (invoice == null)
            {
                return new PaymentAddResponse
                {
                    Result = PaymentAddResult.InvoiceNotFound,
                    Errors = new List<PaymentValidationError>
            {
                PaymentValidationError.InvoiceNotFound
            }
                };
            }

            if (invoice.IsInvoiceArchived)
            {
                return new PaymentAddResponse
                {
                    Result = PaymentAddResult.InvoiceArchived,
                    Errors = new List<PaymentValidationError>
            {
                PaymentValidationError.InvoiceArchived
            }
                };
            }

            if (request.Amount <= 0)
            {
                return new PaymentAddResponse
                {
                    Result = PaymentAddResult.InvalidAmount,
                    Errors = new List<PaymentValidationError>
            {
                PaymentValidationError.InvalidAmount
            }
                };
            }

            var alreadyPaid = _paymentRepository.GetTotalPaidForInvoice(request.InvoiceId);
            var remaining = invoice.TotalAmount - alreadyPaid;

            _logger.LogInformation(
                "Payment validation: InvoiceId={InvoiceId}, InvoiceTotal={InvoiceTotal}, AlreadyPaid={AlreadyPaid}, Remaining={Remaining}, RequestedAmount={RequestedAmount}",
                request.InvoiceId,
                invoice.TotalAmount,
                alreadyPaid,
                remaining,
                request.Amount);

            if (request.Amount > remaining)
            {
                return new PaymentAddResponse
                {
                    Result = PaymentAddResult.AmountExceedsRemaining,
                    Errors = new List<PaymentValidationError>
            {
                PaymentValidationError.AmountExceedsRemaining
            }
                };
            }

            var payment = new Payment
            {
                InvoiceId = request.InvoiceId,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
            };

            payment.Complete();

            _paymentRepository.Add(payment);

            var totalPaid = _paymentRepository.GetTotalPaidForInvoice(request.InvoiceId);

            _statusCalculator.Recalculate(invoice, totalPaid);

            _unitOfWork.Save();

            return new PaymentAddResponse
            {
                Result = PaymentAddResult.Success
            };
        }

        // ================= GET =================

        public IEnumerable<PaymentResponse> GetPaymentsForInvoice(int invoiceId)
        {
            return _paymentRepository
                .GetByInvoiceId(invoiceId)
                .Select(p => new PaymentResponse
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Status = p.Status,
                    InvoiceId = p.InvoiceId
                });
        }

        // ================= DELETE =================

        public PaymentDeleteResult DeletePayment(int paymentId)
        {
            var payment = _paymentRepository.GetById(paymentId);

            if (payment == null)
                return PaymentDeleteResult.NotFound;

            var invoice = _invoiceRepository.GetById(payment.InvoiceId);

            if (invoice == null)
                return PaymentDeleteResult.NotFound;

            _paymentRepository.Delete(payment);

            var totalPaid = _paymentRepository.GetTotalPaidForInvoice(payment.InvoiceId);
            _statusCalculator.Recalculate(invoice, totalPaid);

            _unitOfWork.Save();

            return PaymentDeleteResult.Success;
        }
    }
}