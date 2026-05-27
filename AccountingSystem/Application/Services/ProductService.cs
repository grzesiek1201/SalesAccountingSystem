using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    internal class ProductService
    {
        private readonly AppDbContext _context;
        private readonly ProductValidator _validator;

        public ProductService(AppDbContext context, ProductValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public ProductAddResponse AddProduct(Product product)
        {
            var products = _context.Products.ToList();

            var result = _validator.Validate(product, products);

            if (!result.IsValid)
            {
                return new ProductAddResponse
                {
                    Result = ProductAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _context.Products.Add(product);

            _context.SaveChanges();

            return new ProductAddResponse
            {
                Result = ProductAddResult.Success
            };
        }

        public ProductEditResult EditProduct(Product product)
        {
            var existing = _context.Products
                .FirstOrDefault(x => x.Id == product.Id);

            if (existing == null)
                return ProductEditResult.NotFound;

            if (existing.IsProductArchived)
                return ProductEditResult.ProductArchived;

            var otherProducts = _context.Products
                .Where(x => x.Id != product.Id)
                .ToList();

            var result = _validator.Validate(product, otherProducts);

            if (!result.IsValid)
                return ProductEditResult.InvalidData;

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Category = product.Category;

            _context.SaveChanges();

            return ProductEditResult.Success;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product FindProduct(int id)
        {
            return _context.Products
                .FirstOrDefault(x => x.Id == id);
        }

        public ArchiveProductResult ArchiveProduct(int id)
        {
            var existing = _context.Products
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
            {
                return ArchiveProductResult.NotFound;
            }

            existing.IsProductArchived = true;

            _context.SaveChanges();

            return ArchiveProductResult.Success;
        }
    }
}