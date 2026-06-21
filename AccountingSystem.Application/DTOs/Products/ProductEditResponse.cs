using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Products
{
    public class ProductEditResponse
    {
        public ProductEditResult Result { get; set; }

        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();

        public bool IsSuccess => Result == ProductEditResult.Success;
    }
}
