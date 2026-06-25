using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;

    private readonly ICustomerService _customerService;

    public CustomersController(
        ICustomerService customerService,
        ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("GET /api/customers");

        var customers = _customerService.GetAllCustomers();

        return Ok(customers.Select(c => new CustomerResponse
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            City = c.City,
            Street = c.Street,
            ZipCode = c.ZipCode
        }));
    }

    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        _logger.LogInformation("GET /api/customers/{Id}", id);

        var customer = _customerService.GetCustomerById(id);

        if (customer == null)
        {
            _logger.LogWarning("Customer not found: {Id}", id);
            return NotFound();
        }

        return Ok(new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            City = customer.City,
            Street = customer.Street,
            ZipCode = customer.ZipCode
        });
    }

    [HttpPost]
    public IActionResult Create(CreateCustomerRequest request)
    {
        _logger.LogInformation("POST customer {Name}", request.Name);

        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            City = request.City,
            Street = request.Street,
            ZipCode = request.ZipCode
        };

        var result = _customerService.AddCustomer(request);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Customer create failed: {@Errors}", result.Errors);
            return BadRequest(result.Errors);
        }

        _logger.LogInformation("Customer created: {Id}", customer.Id);

        return CreatedAtAction(nameof(Find), new { id = customer.Id }, new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            City = customer.City,
            Street = customer.Street,
            ZipCode = customer.ZipCode
        });
    }
}