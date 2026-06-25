using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class PaymentAddResponse
    {
        public PaymentAddResult Result { get; set; }

        public List<PaymentValidationError> Errors { get; set; } = new List<PaymentValidationError>();

        public bool IsSuccess => Result == PaymentAddResult.Success;
    }
}
