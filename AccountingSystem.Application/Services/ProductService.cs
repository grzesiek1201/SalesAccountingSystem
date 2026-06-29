using AccountingSystem.Application.DTOs.Products;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class ProductService : IProductService
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

        public ProductAddResponse AddProduct(CreateProductRequest request)
        {
            _logger.LogInformation("Starting AddProduct. Name: {Name}", request.Name);

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId
            };

            var existing = _productRepository.GetAll();
            var result = _validator.Validate(product, existing);

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

            return new ProductAddResponse
            {
                Result = ProductAddResult.Success
            };
        }

        // ================= EDIT =================

        public ProductEditResponse EditProduct(UpdateProductRequest request)
        {
            _logger.LogInformation("Starting EditProduct. Id: {ProductId}", request.Id);

            var existing = _productRepository.GetById(request.Id);

            if (existing == null)
            {
                _logger.LogWarning("Product not found. Id: {ProductId}", request.Id);
                return new ProductEditResponse
                {
                    Result = ProductEditResult.NotFound
                };
            }

            if (existing.IsProductArchived)
            {
                _logger.LogWarning("Attempt to edit archived product. Id: {ProductId}", request.Id);
                return new ProductEditResponse
                {
                    Result = ProductEditResult.ProductArchived
                };
                
            }

            existing.Name = request.Name;
            existing.Price = request.Price;
            existing.CategoryId = request.CategoryId;

            var otherProducts = _productRepository
                .GetAll()
                .Where(x => x.Id != request.Id)
                .ToList();

            var result = _validator.Validate(existing, otherProducts);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditProduct validation failed. Id: {ProductId}, Errors: {Errors}",
                    existing.Id, result.Errors);

                return new ProductEditResponse
                {
                    Result = ProductEditResult.InvalidData
                };
            }

            _productRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product updated successfully. Id: {ProductId}", request.Id);

            return new ProductEditResponse
            {
                Result = ProductEditResult.Success
            };
        }

        // ================= READ =================

        public List<ProductResponse> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products");
            return _productRepository.GetAll()
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId
            })
            .ToList();
        }

        public ProductResponse? GetProductById(int id)
        {
            _logger.LogInformation("Finding product. Id: {ProductId}", id);
            var product = _productRepository.GetById(id);

            if (product == null)
                return null;

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId
            };    
        }

        // ================= ARCHIVE =================

        public ProductArchiveResult ArchiveProduct(int id)
        {
            _logger.LogInformation("Archiving product. Id: {ProductId}", id);

            var existing = _productRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Product not found for archive. Id: {ProductId}", id);
                return ProductArchiveResult.NotFound;
            }

            existing.IsProductArchived = true;

            _productRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product archived successfully. Id: {ProductId}", id);

            return ProductArchiveResult.Success;
        }
    }
}