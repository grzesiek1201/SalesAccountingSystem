using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;

namespace AccountingSystem.Application.Services
{
    internal class ProductService
    {
        private List<Product> products = new List<Product>();
        public int nextId;
        private readonly ProductValidator _validator;

        public ProductService(ProductValidator validator)
        {
            _validator = validator;
        }

        public ProductAddResponse AddProduct(Product product)
        {
            var result = _validator.Validate(product, products);

            if (!result.IsValid)
            {
                return new ProductAddResponse
                {
                    Result = ProductAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            product.Id = nextId;
            nextId++;

            products.Add(product);

            return new ProductAddResponse
            {
                Result = ProductAddResult.Success
            };
        }

        public Domain.Enums.ProductEditResult EditProduct(Product product)
        {
            var existing = products.Find(x => x.Id == product.Id);

            if (existing == null)
                return Domain.Enums.ProductEditResult.NotFound;

            if (existing.IsProductArchived)
                return Domain.Enums.ProductEditResult.ProductArchived;

            var otherProducts = products.Where(x => x.Id != product.Id).ToList();
            var result = _validator.Validate(product, otherProducts);

            if (!result.IsValid)
                return Domain.Enums.ProductEditResult.InvalidData;

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Category = product.Category;

            return Domain.Enums.ProductEditResult.Success;
        }

        public List<Product> GetAllProducts()
        {
            return products;
        }

        public Product FindProduct(int Id)
        {
            return products.FirstOrDefault(x => x.Id == Id);
        }

        public Domain.Enums.ArchiveProductResult ArchiveProduct(int Id)
        {
            var existing = products.Find(x => x.Id == Id);
            if (existing == null)
            {
                return Domain.Enums.ArchiveProductResult.NotFound;
            }
            existing.IsProductArchived = true;
            return Domain.Enums.ArchiveProductResult.Success;
        }
    }
}
