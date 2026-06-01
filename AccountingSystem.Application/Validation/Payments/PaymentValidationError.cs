namespace AccountingSystem.Application.Validation.Payments
{
    public enum PaymentValidationError
    {
        None,
        InvoiceNotFound,
        InvoiceArchived,
        InvalidAmount,
        AmountExceedsRemaining
    }
}