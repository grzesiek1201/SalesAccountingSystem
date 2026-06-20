using AccountingSystem.API.DTOs.Customers;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AccountingSystem.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var customers = _customerService.GetAllCustomers();

        var response = customers.Select(c => new CustomerResponse
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            City = c.City,
            Street = c.Street,
            ZipCode = c.ZipCode
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult Find(int id)
    {
        var customer = _customerService.FindCustomer(id);

        if (customer == null)
            return NotFound();

        var response = new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            City = customer.City,
            Street = customer.Street,
            ZipCode = customer.ZipCode
        };

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            City = request.City,
            Street = request.Street,
            ZipCode = request.ZipCode
        };

        var result = _customerService.AddCustomer(customer);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        var response = new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            City = customer.City,
            Street = customer.Street,
            ZipCode = customer.ZipCode
        };

        return CreatedAtAction(
            nameof(Find),
            new { id = customer.Id },
            response
        );
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateCustomerRequest request)
    {
        var customer = new Customer
        {
            Id = id,
            Name = request.Name,
            Email = request.Email,
            City = request.City,
            Street = request.Street,
            ZipCode = request.ZipCode
        };

        var result = _customerService.EditCustomer(customer);

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        var response = new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            City = customer.City,
            Street = customer.Street,
            ZipCode = customer.ZipCode
        };

        return Ok(response);
    }

    [HttpPatch("{id}/archive")]
    public IActionResult Archive(int id)
    {
        var result = _customerService.ArchiveCustomer(id);

        if (result == CustomerArchiveResult.NotFound)
            return NotFound();

        return NoContent();
    }
}