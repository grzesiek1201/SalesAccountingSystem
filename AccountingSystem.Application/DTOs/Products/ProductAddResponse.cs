using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.DTOs.Products
{
    public class ProductAddResponse
    {
        public ProductAddResult Result { get; set; }

        public List<ProductValidationError> Errors { get; set; } = new List<ProductValidationError>();

        public bool IsSuccess => Result == ProductAddResult.Success;
    }
}
