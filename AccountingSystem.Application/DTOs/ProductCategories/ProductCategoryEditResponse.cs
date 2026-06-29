using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.ProductCategories
{
    public class ProductCategoryEditResponse
    {
        public ProductCategoryEditResult Result { get; set; }

        public List<ProductCategoryValidationError> Errors { get; set; } = new List<ProductCategoryValidationError>();

        public bool IsSuccess => Result == ProductCategoryEditResult.Success;
    }
}
