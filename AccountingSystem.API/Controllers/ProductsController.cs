using AccountingSystem.API.DTOs.Products;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ProductService productService,
                              ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GET /api/products");

        var products = _productService.GetAllProducts();

        _logger.LogInformation("Products count: {Count}", products.Count);

        return Ok(products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId
        }));
    }

    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        _logger.LogInformation("GET /api/products/{Id}", id);

        var product = _productService.FindProduct(id);

        if (product == null)
        {
            _logger.LogWarning("Product not found: {Id}", id);
            return NotFound();
        }

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        });
    }

    [HttpPost]
    public IActionResult Create(CreateProductRequest request)
    {
        _logger.LogInformation("POST product Name={Name}", request.Name);

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        var result = _productService.AddProduct(product);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Product create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Product created: {Id}", product.Id);

        return CreatedAtAction(nameof(Find), new { id = product.Id }, new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateProductRequest request)
    {
        _logger.LogInformation("PUT product {Id}", id);

        var product = new Product
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId
        };

        var result = _productService.EditProduct(product);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Product update failed {Id}", id);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Product updated: {Id}", id);

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        });
    }
}