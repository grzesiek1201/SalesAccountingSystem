using AccountingSystem.AccountingSystem.Application.DTOs;
using AccountingSystem.AccountingSystem.Application.Validation.Products;
using AccountingSystem.AccountingSystem.Domain.Entities;
using AccountingSystem.AccountingSystem.Domain.Enums;
using AccountingSystem.AccountingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.AccountingSystem.Application.Services
{
    internal class ProductService
    {
        private readonly AppDbContext _context;
        private readonly ProductValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            AppDbContext context,
            ProductValidator validator,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _validator = validator;
            _unitOfWork = unitOfWork;
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

            _unitOfWork.Save();

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

            _unitOfWork.Save();

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
                return ArchiveProductResult.NotFound;

            existing.IsProductArchived = true;

            _unitOfWork.Save();

            return ArchiveProductResult.Success;
        }
    }
}