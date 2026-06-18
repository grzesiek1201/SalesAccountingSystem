using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ProductValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ================= ADD =================

        public ProductAddResponse AddProduct(Product product)
        {
            _logger.LogInformation("Starting AddProduct. Name: {Name}", product.Name);

            var products = _productRepository.GetAll();
            var result = _validator.Validate(product, products);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddProduct validation failed. Errors: {Errors}", result.Errors);

                return new ProductAddResponse
                {
                    Result = ProductAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _productRepository.Add(product);
            _unitOfWork.Save();

            _logger.LogInformation("Product added successfully. Id: {ProductId}", product.Id);

            return new ProductAddResponse
            {
                Result = ProductAddResult.Success
            };
        }

        // ================= EDIT =================

        public ProductEditResult EditProduct(Product product)
        {
            _logger.LogInformation("Starting EditProduct. Id: {ProductId}", product.Id);

            var existing = _productRepository.GetById(product.Id);

            if (existing == null)
            {
                _logger.LogWarning("Product not found. Id: {ProductId}", product.Id);
                return ProductEditResult.NotFound;
            }

            if (existing.IsProductArchived)
            {
                _logger.LogWarning("Attempt to edit archived product. Id: {ProductId}", product.Id);
                return ProductEditResult.ProductArchived;
            }

            var otherProducts = _productRepository
                .GetAll()
                .Where(x => x.Id != product.Id)
                .ToList();

            var result = _validator.Validate(product, otherProducts);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditProduct validation failed. Id: {ProductId}, Errors: {Errors}",
                    product.Id, result.Errors);

                return ProductEditResult.InvalidData;
            }

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Category = product.Category;

            _productRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product updated successfully. Id: {ProductId}", product.Id);

            return ProductEditResult.Success;
        }

        // ================= READ =================

        public List<Product> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products");
            return _productRepository.GetAll();
        }

        public Product? FindProduct(int id)
        {
            _logger.LogInformation("Finding product. Id: {ProductId}", id);
            return _productRepository.GetById(id);
        }

        // ================= ARCHIVE =================

        public ArchiveProductResult ArchiveProduct(int id)
        {
            _logger.LogInformation("Archiving product. Id: {ProductId}", id);

            var existing = _productRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Product not found for archive. Id: {ProductId}", id);
                return ArchiveProductResult.NotFound;
            }

            existing.IsProductArchived = true;

            _productRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product archived successfully. Id: {ProductId}", id);

            return ArchiveProductResult.Success;
        }
    }
}