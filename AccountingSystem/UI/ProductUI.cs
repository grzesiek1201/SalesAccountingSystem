using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.UI
{
    internal class ProductUI
    {
        private ProductService _productService;

        public ProductUI(ProductService ProductService)
        {
            _productService = ProductService;
        }

        public void AddProductFlow()
        {
            var product = GetProductInput();

            var response = _productService.AddProduct(product);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine("Customer added successfully");
        }

        public void EditProductFlow()
        {
            Console.WriteLine("To edit prodiuct just fill fields below. If there is a product matching your Id, data will change");

            var product = GetProductInput();
            _productService.EditProduct(product);
        }

        public void GetAllProductsFlow()
        {
            List<Product> products = _productService.GetAllProducts();

            foreach (var c in products)
            {
                Console.WriteLine(
                    $"Name: {c.Name}, Id: {c.Id}, Price: {c.Price},\n" +
                    $"Category Id: {c.Category.Id}, Name: {c.Category.Name}"
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
                    $"Name: {result.Name}, Id: {result.Id}, Price: {result.Price},\n" +
                    $"Category Id: {result.Category.Id}, Category name: {result.Category.Name},\n" 
                );
            }
            else
            {
                Console.WriteLine("Product not found. Try again");
            }
        }

        public void ArchiveProductFlow()
        {
            int id = GetProductId();

            var result = _productService.ArchiveProduct(id);

            if (result == Domain.Enums.ArchiveProductResult.NotFound)
            {
                Console.WriteLine("Product is not found. Try again");
            }
            else if (result == Domain.Enums.ArchiveProductResult.Success)
            {
                Console.WriteLine("Product has been archived.");
            }
        }

        public Product GetProductInput()
        {
            Console.Write("add name of the product: ");
            string name = Console.ReadLine();

            Console.Write("add price: ");
            string price = Console.ReadLine();
            if (decimal.TryParse(price, out var priceValue))

            Console.Write("add category: ");
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
            return Convert.ToInt32(Console.ReadLine());
        }

        private string GetPrductErrorMessage(ProductValidationError error)
        {
            return error switch
            {
                ProductValidationError.EmptyName => "Name is empty",
                ProductValidationError.NameTooLong => "Name is too long",
                ProductValidationError.DuplicateName => "Name is duplicated",
                ProductValidationError.InvalidPrice => "Price is invalid",
                ProductValidationError.EmptyCategory => "Category name is empty",
                ProductValidationError.CategoryTooLong => "Category name is too long",
                ProductValidationError.DuplicateCategory => "Category name is duplicated",

                _ => "Unknown error"
            };
        }

    }
}
