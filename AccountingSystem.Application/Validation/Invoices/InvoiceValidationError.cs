public enum InvoiceValidationError
{
    InvoiceNull,
    EmptyInvoiceNumber,
    EmptyCustomer,
    NoItems,
    InvoiceItemNull,
    EmptyProduct,
    InvalidQuantity,
    InvalidUnitPrice,
    InvalidDiscountPercent,
    InvalidIssueDate,
    InvalidDueDate,
    DuplicateProduct,
    InvalidStatus,
    InvalidTotalAmount
}
