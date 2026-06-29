using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IProductCategoryRepository
    {
        List<ProductCategory> GetAll();
        ProductCategory? GetById(int id);
        List<ProductCategory> GetByIds(List<int> ids);
        void Add(ProductCategory productCategory);
        void Update(ProductCategory productCategory);
    }
}
