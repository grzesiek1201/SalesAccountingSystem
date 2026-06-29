using AccountingSystem.Application.DTOs.ProductCategories;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IProductCategoryService
    {
        ProductCategoryAddResponse AddProductCategory(CreateProductCategoryRequest request);
        ProductCategoryEditResponse EditProductCategory(UpdateProductCategoryRequest request);
        List<ProductCategoryResponse> GetAllProductCategories();
        ProductCategoryResponse? GetProductCategoryById(int id);
        ProductCategoryStatusResult ChangeProductCategoryStatus(int id, bool IsActive);
    }
}
