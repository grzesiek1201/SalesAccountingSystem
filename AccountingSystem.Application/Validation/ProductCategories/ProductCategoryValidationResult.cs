namespace AccountingSystem.Application.Validation.ProductCategories
{
    public class ProductCategoryValidationResult
    {
        public List<ProductCategoryValidationError> Errors { get; set; } = new List<ProductCategoryValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
