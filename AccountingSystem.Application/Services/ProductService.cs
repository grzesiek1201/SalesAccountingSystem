using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            ProductValidator validator,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public ProductAddResponse AddProduct(Product product)
        {
            var products = _productRepository.GetAll();

            var result = _validator.Validate(product, products);

            if (!result.IsValid)
            {
                return new ProductAddResponse
                {
                    Result = ProductAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _productRepository.Add(product);
            _unitOfWork.Save();

            return new ProductAddResponse
            {
                Result = ProductAddResult.Success
            };
        }

        public ProductEditResult EditProduct(Product product)
        {
            var existing = _productRepository.GetById(product.Id);

            if (existing == null)
                return ProductEditResult.NotFound;

            if (existing.IsProductArchived)
                return ProductEditResult.ProductArchived;

            var otherProducts = _productRepository
                .GetAll()
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
            return _productRepository.GetAll();
        }

        public Product? FindProduct(int id)
        {
            return _productRepository.GetById(id);
        }

        public ArchiveProductResult ArchiveProduct(int id)
        {
            var existing = _productRepository.GetById(id);

            if (existing == null)
                return ArchiveProductResult.NotFound;

            existing.IsProductArchived = true;

            _unitOfWork.Save();

            return ArchiveProductResult.Success;
        }
    }
}