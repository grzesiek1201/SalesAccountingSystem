using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Invoices
{
    public class InvoiceAddResponse
    {
        public InvoiceAddResult Result { get; set; }

        public List<InvoiceValidationError> Errors { get; set; } = new List<InvoiceValidationError>();

        public bool IsSuccess => Result == InvoiceAddResult.Success;
    }
}
