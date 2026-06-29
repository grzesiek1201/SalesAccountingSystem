using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.ProductCategories
{

    public class ProductCategoryValidator
    {
        public ProductCategoryValidationResult Validate(ProductCategory productCategory, List<ProductCategory> productCategories)
        {
            var result = new ProductCategoryValidationResult();

            if (string.IsNullOrWhiteSpace(productCategory.Name))
            {
                result.Errors.Add(ProductCategoryValidationError.EmptyName);
            }
            else
            {
                if (productCategory.Name.Length > 64)
                    result.Errors.Add(ProductCategoryValidationError.NameTooLong);

                if (productCategories.Exists(x => x.Name == productCategory.Name && x.Id != productCategory.Id))
                    result.Errors.Add(ProductCategoryValidationError.DuplicateName);
            }

            return result;
        }

    }
}