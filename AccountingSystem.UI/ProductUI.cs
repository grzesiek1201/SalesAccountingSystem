using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Application.Validation.Products;
using System;
using System.Collections.Generic;

namespace AccountingSystem.UI
{
    public class ProductUI
    {
        private readonly ProductService _productService;

        public ProductUI(ProductService productService)
        {
            _productService = productService;
        }

        public void AddProductFlow()
        {
            var product = GetProductInput();

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
            Console.WriteLine("Edit product - fill fields below.");

            int id = GetProductId();
            var product = GetProductInput();

            product.Id = id;

            var result = _productService.EditProduct(product);

            switch (result)
            {
                case AccountingSystem.Domain.Enums.ProductEditResult.Success:
                    Console.WriteLine("Product updated successfully");
                    break;

                case AccountingSystem.Domain.Enums.ProductEditResult.NotFound:
                    Console.WriteLine("Product not found");
                    break;

                case AccountingSystem.Domain.Enums.ProductEditResult.ProductArchived:
                    Console.WriteLine("Product is archived");
                    break;

                case AccountingSystem.Domain.Enums.ProductEditResult.InvalidData:
                    Console.WriteLine("Invalid product data");
                    break;
            }
        }

        public void GetAllProductsFlow()
        {
            var products = _productService.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("No products found");
                return;
            }

            foreach (var p in products)
            {
                PrintProduct(p);
            }
        }

        public void FindProductFlow()
        {
            int id = GetProductId();

            var product = _productService.FindProduct(id);

            if (product == null)
            {
                Console.WriteLine("Product not found");
                return;
            }

            PrintProduct(product);
        }

        public void ArchiveProductFlow()
        {
            int id = GetProductId();

            var result = _productService.ArchiveProduct(id);

            switch (result)
            {
                case AccountingSystem.Domain.Enums.ArchiveProductResult.NotFound:
                    Console.WriteLine("Product not found");
                    break;

                case AccountingSystem.Domain.Enums.ArchiveProductResult.Success:
                    Console.WriteLine("Product archived successfully");
                    break;
            }
        }

        private Product GetProductInput()
        {
            Console.Write("Product name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Price: ");
            decimal price;

            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.Write("Invalid price. Try again: ");
            }

            Console.Write("Category name: ");
            string categoryName = Console.ReadLine() ?? "";

            return new Product
            {
                Name = name,
                Price = price,
                Category = new Category
                {
                    Name = categoryName
                }
            };
        }

        private int GetProductId()
        {
            Console.Write("Product ID: ");

            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Invalid ID. Try again: ");
            }

            return id;
        }

        private void PrintProduct(Product p)
        {
            Console.WriteLine(
                $"Id: {p.Id}, Name: {p.Name}, Price: {p.Price}\n" +
                $"Category: {p.Category?.Name}, Archived: {p.IsProductArchived}"
            );
        }

        private string GetProductErrorMessage(ProductValidationError error)
        {
            return error switch
            {
                ProductValidationError.EmptyName => "Name is empty",
                ProductValidationError.NameTooLong => "Name is too long",
                ProductValidationError.DuplicateName => "Name already exists",
                ProductValidationError.InvalidPrice => "Price is invalid",
                ProductValidationError.EmptyPrice => "Price is required",
                ProductValidationError.EmptyCategory => "Category is required",
                ProductValidationError.CategoryTooLong => "Category is too long",
                ProductValidationError.DuplicateCategory => "Category already exists",
                _ => "Unknown error"
            };
        }
    }
}
