using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingSystem.Application.Validation.Products
{
    internal class ProductValidator
    {
        public ProductValidationResult Validate(Product product, List<Product> products)
        {
            var result = new ProductValidationResult();


            if (string.IsNullOrWhiteSpace(product.Name))
                result.Errors.Add(ProductValidationError.EmptyName);
            {
                if (product.Name.Length > 64)
                    result.Errors.Add(ProductValidationError.NameTooLong);

                if (products.Exists(x => x.Name == product.Name && x.Id != product.Id))
                    result.Errors.Add(ProductValidationError.DuplicateName);
            }

            if (product.Price <= 0)
                result.Errors.Add(ProductValidationError.InvalidPrice);

            if (string.IsNullOrWhiteSpace(product.Category.Name))
                result.Errors.Add(ProductValidationError.EmptyCategory);
            {
                if (product.Category.Name.Length > 64)
                    result.Errors.Add(ProductValidationError.CategoryTooLong);

                if (products.Exists(x => x.Category.Name == product.Category.Name && x.Category.Id != product.Category.Id))
                    result.Errors.Add(ProductValidationError.DuplicateCategory);
            }
            return result;
        }
    }
}
