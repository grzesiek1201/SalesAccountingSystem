using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.Infrastructure.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<ProductCategory> GetAll()
    => _context.ProductCategories.AsNoTracking().ToList();

        public ProductCategory? GetById(int id)
            => _context.ProductCategories
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

        public List<ProductCategory> GetByIds(List<int> ids)
            => _context.ProductCategories
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToList();

        public void Add(ProductCategory productCategory)
            => _context.ProductCategories.Add(productCategory);

        public void Update(ProductCategory productCategory)
            => _context.ProductCategories.Update(productCategory);
    }
}
