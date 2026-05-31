using System.Collections.Generic;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
    }
}