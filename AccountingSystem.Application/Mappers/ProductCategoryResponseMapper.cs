using AccountingSystem.Application.DTOs.ProductCategories;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Mappers
{
    public class ProductCategoryResponseMapper
    {
        public ProductCategoryResponse Map(ProductCategory p)
        {
            return new ProductCategoryResponse
            {
                Id = p.Id,
                Name = p.Name
            };
        }
    }
}

