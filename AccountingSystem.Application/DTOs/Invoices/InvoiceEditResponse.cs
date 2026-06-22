using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class InvoiceEditResponse
    {
        public InvoiceEditResult Result { get; set; }

        public List<InvoiceValidationError> Errors { get; set; } = new List<InvoiceValidationError>();

        public bool IsSuccess => Result == InvoiceEditResult.Success;
    }
}
