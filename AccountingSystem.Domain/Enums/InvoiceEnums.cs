namespace AccountingSystem.Domain.Enums
{

    public enum InvoiceAddResult
    {
        Success,
        InvalidData
    }

    public enum InvoiceEditResult
    {
        Success,
        NotFound,
        InvalidData,
        InvoiceArchived
    }

    public enum ArchiveInvoiceResult
    {
        Success,
        NotFound,
    }

    public enum ValidateInvoiceResult
    {
        IsValid,
        NotValid
    }

    public enum PaymentAddResult
    {
        Success,
        InvoiceNotFound,
        InvoiceArchived,
        InvalidAmount,
        AmountExceedsRemaining
    }

    public enum InvoiceStatusResult
    {
        Success,
        NotFound,
        InvalidOperation,
    }
}
