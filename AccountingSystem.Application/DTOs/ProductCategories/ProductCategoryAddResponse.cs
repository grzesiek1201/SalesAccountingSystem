using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.ProductCategories
{
    public class ProductCategoryAddResponse
    {
        public ProductCategoryAddResult Result { get; set; }

        public List<ProductCategoryValidationError> Errors { get; set; } = new List<ProductCategoryValidationError>();

        public bool IsSuccess => Result == ProductCategoryAddResult.Success;
    }
}
