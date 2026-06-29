using AccountingSystem.Application.DTOs.Products;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Interfaces
{
    public interface IProductService
    {
        ProductAddResponse AddProduct(CreateProductRequest request);
        ProductEditResponse EditProduct(UpdateProductRequest request);
        List<ProductResponse> GetAllProducts();
        ProductResponse? GetProductById(int id);
        ProductArchiveResult ArchiveProduct(int id);
    }
}
