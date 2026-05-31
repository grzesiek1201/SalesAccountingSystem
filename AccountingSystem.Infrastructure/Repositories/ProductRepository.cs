using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
            => _context.Products.AsNoTracking().ToList();

        public Product? GetById(int id)
            => _context.Products.FirstOrDefault(x => x.Id == id);

        public void Add(Product product)
            => _context.Products.Add(product);

        public void Update(Product product)
            => _context.Products.Update(product);
    }
}