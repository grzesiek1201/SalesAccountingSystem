using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Application.Validation.Products
{
    public class ProductValidator
    {
        public ProductValidationResult Validate(Product product, List<Product> products)
        {
            var result = new ProductValidationResult();

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                result.Errors.Add(ProductValidationError.EmptyName);
            }
            else
            {
                if (product.Name.Length > 64)
                    result.Errors.Add(ProductValidationError.NameTooLong);

                if (products.Exists(x => x.Name == product.Name && x.Id != product.Id))
                    result.Errors.Add(ProductValidationError.DuplicateName);
            }

            if (product.Price <= 0)
            {
                result.Errors.Add(ProductValidationError.InvalidPrice);
            }

            if (product.Category == null)
            {
                result.Errors.Add(ProductValidationError.EmptyCategory);
            }

            return result;
        }
    }
}
