using AccountingSystem.Application.DTOs.ProductCategories;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.ProductCategories;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly ProductCategoryValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductCategoryService> _logger;
    

        public ProductCategoryService(
            IProductCategoryRepository productCategoryRepository,
            ProductCategoryValidator validator,
            IUnitOfWork unityOfWork,
            ILogger<ProductCategoryService> logger)
        {
            _productCategoryRepository = productCategoryRepository;
            _validator = validator;
            _unitOfWork = unityOfWork;
            _logger = logger;
        }

        // ================= ADD =================

        public ProductCategoryAddResponse AddProductCategory(CreateProductCategoryRequest request)
        {
            _logger.LogInformation("Starting AddProductCategory. Name: {Name}", request.Name);

            var productCategory = new ProductCategory
            {
                Name = request.Name,
                IsActive = true
            };

            var existing = _productCategoryRepository.GetAll();
            var result = _validator.Validate(productCategory, existing);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddProduct category validation failed. Errors: {Errors}", result.Errors);

                return new ProductCategoryAddResponse
                {
                    Result = ProductCategoryAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _productCategoryRepository.Add(productCategory);
            _unitOfWork.Save();

            return new ProductCategoryAddResponse
            {
                Result = ProductCategoryAddResult.Success
            };
        }

        // ================= EDIT =================

        public ProductCategoryEditResponse EditProductCategory(UpdateProductCategoryRequest request)
        {
            _logger.LogInformation("Starting EditProductCategory. Id: {ProductCategoryId}", request.Id);

            var existing = _productCategoryRepository.GetById(request.Id);

            if (existing == null)
            {
                _logger.LogWarning("Product Category not found. Id: {ProductCategoryId}", request.Id);
                return new ProductCategoryEditResponse
                {
                    Result = ProductCategoryEditResult.NotFound
                };
            }

            if (!existing.IsActive)
            {
                _logger.LogWarning("Attempt to edit active product category. Id: {ProductCategoryId}", request.Id);
                return new ProductCategoryEditResponse
                {
                    Result = ProductCategoryEditResult.ProductCategoryInactive
                };

            }

            existing.Name = request.Name;

            var otherProductCategories = _productCategoryRepository
                .GetAll()
                .Where(x => x.Id != request.Id)
                .ToList();

            var result = _validator.Validate(existing, otherProductCategories);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditProductCategory validation failed. Id: {ProductCategoryId}, Errors: {Errors}",
                    existing.Id, result.Errors);

                return new ProductCategoryEditResponse
                {
                    Result = ProductCategoryEditResult.InvalidData
                };
            }

            _productCategoryRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product Category updated successfully. Id: {ProductCategoryId}", request.Id);

            return new ProductCategoryEditResponse
            {
                Result = ProductCategoryEditResult.Success
            };
        }

        // ================= READ =================

        public List<ProductCategoryResponse> GetAllProductCategories()
        {
            _logger.LogInformation("Fetching all product categories");
            return _productCategoryRepository.GetAll()
            .Select(p => new ProductCategoryResponse
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive
            })
            .ToList();
        }

        public ProductCategoryResponse? GetProductCategoryById(int id)
        {
            _logger.LogInformation("Finding product category. Id: {ProductCategoryId}", id);
            var productCategory = _productCategoryRepository.GetById(id);

            if (productCategory == null)
                return null;

            return new ProductCategoryResponse
            {
                Id = productCategory.Id,
                Name = productCategory.Name,
                IsActive = productCategory.IsActive
            };
        }

        // ================= CHANGE STATUS =================

        public ProductCategoryStatusResult ChangeProductCategoryStatus(int id, bool isActive)
        {
            _logger.LogInformation("Changing product category status. Id: {ProductCategoryId}", id);

            var existing = _productCategoryRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Product category not found for changing status. Id: {ProductCategoryId}", id);
                return ProductCategoryStatusResult.NotFound;
            }

            if (existing.IsActive == isActive)
            {
                _logger.LogInformation("No status change required. Id: {ProductCategoryId}", id);
                return ProductCategoryStatusResult.Success;
            }

            existing.IsActive = isActive;

            _productCategoryRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Product category status changed successfully. Id: {ProductCategoryId}", id);

            return ProductCategoryStatusResult.Success;
        }
    }
}
