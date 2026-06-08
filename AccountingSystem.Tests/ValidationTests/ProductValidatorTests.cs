using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using Xunit;
using System.Collections.Generic;

public class ProductValidatorTests
{
    private readonly ProductValidator _validator = new();

    private Product CreateValidProduct()
    {
        return new Product
        {
            Id = 2,
            Name = "Chocolate GOLD",
            Price = 100,
            CategoryId = 1,
            IsProductArchived = false,
            Category = CreateValidCategory()
        };
    }

    private Category CreateValidCategory()
    {
        return new Category
        {
            Id = 1,
            Name = "Sweets",
            Products = new List<Product>()
        };
    }

    [Fact]
    public void Validate_ValidProduct_ShouldReturnValidResult()
    {
        var product = CreateValidProduct();
        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_EmptyName_ShouldContainEmptyNameError()
    {
        var product = CreateValidProduct();
        product.Name = "";
        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.EmptyName, result.Errors);
    }

    [Fact]
    public void Validate_NameTooLong_ShouldContainNameTooLongError()
    {
        var product = CreateValidProduct();
        product.Name = new string('A', 70);
        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.NameTooLong, result.Errors);
    }

    [Fact]
    public void Validate_DuplicateName_ShouldContainDuplicateNameError()
    {
        var existing = CreateValidProduct();
        existing.Id = 1;

        var products = new List<Product> { existing };

        var product = CreateValidProduct();
        product.Id = 2;
        product.Name = existing.Name;

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.DuplicateName, result.Errors);
    }

    [Fact]
    public void Validate_InvalidPrice_ShouldContainInvalidPriceError()
    {
        var product = CreateValidProduct();
        product.Price = -10;

        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.InvalidPrice, result.Errors);
    }

    [Fact]
    public void Validate_EmptyCategory_ShouldContainEmptyCategoryError()
    {
        var product = CreateValidProduct();
        product.Category = null;

        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.EmptyCategory, result.Errors);
    }

    [Fact]
    public void Validate_CategoryTooLong_ShouldContainCategoryTooLongError()
    {
        var product = CreateValidProduct();
        product.Category = new Category
        {
            Id = 1,
            Name = new string('A', 70),
            Products = new List<Product>()
        };

        var products = new List<Product>();

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.CategoryTooLong, result.Errors);
    }

    [Fact]
    public void Validate_DuplicateCategory_ShouldContainDuplicateCategoryError()
    {
        var existingCategory = new Category
        {
            Id = 1,
            Name = "Sweets",
            Products = new List<Product>()
        };

        var existingProduct = new Product
        {
            Id = 1,
            Name = "Existing",
            Price = 10,
            Category = existingCategory,
            CategoryId = 1,
            IsProductArchived = false
        };

        var products = new List<Product> { existingProduct };

        var product = CreateValidProduct();
        product.Id = 2;
        product.Category = existingCategory;
        product.CategoryId = 1;

        var result = _validator.Validate(product, products);

        Assert.Contains(ProductValidationError.DuplicateCategory, result.Errors);
    }
}