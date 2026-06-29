namespace AccountingSystem.Domain.Enums
{
    public enum ProductCategoryAddResult
    {
        Success,
        InvalidData
    }

    public enum ProductCategoryEditResult
    {
        Success,
        NotFound,
        InvalidData,
        ProductCategoryInactive
    }

    public enum ProductCategoryStatusResult
    {
        Success,
        NotFound,
    }

    public enum ValidateCategoryProductResult
    {
        IsValid,
        NotValid

    }

}
