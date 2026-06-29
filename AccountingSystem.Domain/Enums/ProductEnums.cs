namespace AccountingSystem.Domain.Enums
{
    public enum ProductAddResult
    {
        Success,
        InvalidData
    }

    public enum ProductEditResult
    {
        Success,
        NotFound,
        InvalidData,
        ProductArchived
    }

    public enum ProductArchiveResult
    {
        Success,
        NotFound,
    }

    public enum ValidateProductResult
    {
        IsValid,
        NotValid

    }

}
