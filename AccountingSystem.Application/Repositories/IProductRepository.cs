using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? GetById(int id);
        List<Product> GetByIds(List<int> ids);
        void Add(Product product);
        void Update(Product product);
    }
}