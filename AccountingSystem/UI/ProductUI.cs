using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AccountingSystem.UI
{
    internal class ProductUI
    {
        private readonly ProductService _productService;

        public ProductUI(ProductService productService)
        {
            _productService = productService;
        }

        public void AddProductFlow()
        {
            var product = GetProductInput();

            if (product == null)
                return;

            var response = _productService.AddProduct(product);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(GetProductErrorMessage(error));
                }

                return;
            }

            Console.WriteLine("Product added successfully");
        }

        public void EditProductFlow()
        {
            Console.WriteLine("To edit product fill fields below.");

            int id = GetProductId();

            var product = GetProductInput();

            if (product == null)
                return;

            product.Id = id;

            var result = _productService.EditProduct(product);

            if (result == Domain.Enums.ProductEditResult.NotFound)
            {
                Console.WriteLine("Product not found.");
            }
            else if (result == Domain.Enums.ProductEditResult.ProductArchived)
            {
                Console.WriteLine("Archived product cannot be edited.");
            }
            else if (result == Domain.Enums.ProductEditResult.InvalidData)
            {
                Console.WriteLine("Product data is invalid.");
            }
            else if (result == Domain.Enums.ProductEditResult.Success)
            {
                Console.WriteLine("Product updated successfully.");
            }
        }

        public void GetAllProductsFlow()
        {
            List<Product> products = _productService.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("Products list is empty.");
                return;
            }

            foreach (var p in products)
            {
                Console.WriteLine(
                    $"Name: {p.Name}, Id: {p.Id}, Price: {p.Price}\n" +
                    $"Category: {p.Category?.Name}, Archived: {p.IsProductArchived}"
                );
            }
        }

        public void FindProductFlow()
        {
            int idSearch = GetProductId();

            var result = _productService.FindProduct(idSearch);

            if (result != null)
            {
                Console.WriteLine(
                    $"Name: {result.Name}, Id: {result.Id}, Price: {result.Price}\n" +
                    $"Category: {result.Category?.Name}, Archived: {result.IsProductArchived}"
                );
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }

        public void ArchiveProductFlow()
        {
            int id = GetProductId();

            var result = _productService.ArchiveProduct(id);

            if (result == Domain.Enums.ArchiveProductResult.NotFound)
            {
                Console.WriteLine("Product not found.");
            }
            else if (result == Domain.Enums.ArchiveProductResult.Success)
            {
                Console.WriteLine("Product archived successfully.");
            }
        }

        public Product GetProductInput()
        {
            Console.Write("Add product name: ");
            string name = Console.ReadLine();

            Console.Write("Add price: ");
            string priceInput = Console.ReadLine();

            if (!decimal.TryParse(priceInput, out decimal priceValue))
            {
                Console.WriteLine("Invalid price format.");
                return null;
            }

            Console.Write("Add category: ");
            string group = Console.ReadLine();

            var category = new Category
            {
                Name = group
            };

            return new Product
            {
                Name = name,
                Price = priceValue,
                Category = category
            };
        }

        public int GetProductId()
        {
            Console.Write("Type product ID: ");

            int id;

            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Invalid ID. Type number: ");
            }

            return id;
        }

        private string GetProductErrorMessage(ProductValidationError error)
        {
            return error switch
            {
                ProductValidationError.EmptyName => "Name is empty",
                ProductValidationError.NameTooLong => "Name is too long",
                ProductValidationError.DuplicateName => "Name is duplicated",
                ProductValidationError.InvalidPrice => "Price is invalid",
                ProductValidationError.EmptyPrice => "Price is empty",
                ProductValidationError.EmptyCategory => "Category name is empty",
                ProductValidationError.CategoryTooLong => "Category name is too long",
                ProductValidationError.DuplicateCategory => "Category name is duplicated",
                _ => "Unknown error"
            };
        }
    }
}