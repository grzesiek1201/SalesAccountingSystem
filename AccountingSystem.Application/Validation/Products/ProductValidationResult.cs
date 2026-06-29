namespace AccountingSystem.Application.Validation.Products
{
    public class ProductValidationResult
    {
        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();
        public bool IsValid => Errors.Count == 0;

    }
}
