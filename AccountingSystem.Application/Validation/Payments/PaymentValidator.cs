using System.Linq;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.Payments
{
    public class PaymentValidator
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentValidator(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public PaymentValidationResult Validate(int invoiceId, Payment payment)
        {
            var result = new PaymentValidationResult();

            var invoice = _invoiceRepository.GetById(invoiceId);

            if (invoice == null)
            {
                result.Errors.Add(PaymentValidationError.InvoiceNotFound);
                return result;
            }

            if (invoice.IsInvoiceArchived)
            {
                result.Errors.Add(PaymentValidationError.InvoiceArchived);
                return result;
            }

            if (payment.Amount <= 0)
            {
                result.Errors.Add(PaymentValidationError.InvalidAmount);
            }

            var alreadyPaid = invoice.Payments.Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - alreadyPaid;

            if (payment.Amount > remaining)
            {
                result.Errors.Add(PaymentValidationError.AmountExceedsRemaining);
            }

            return result;
        }
    }
}