using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AccountingSystem.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository repository,
            CustomerValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<CustomerService> logger)
        {
            _repository = repository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ================= ADD =================

        public CustomerAddResponse AddCustomer(Customer customer)
        {
            _logger.LogInformation("Starting AddCustomer. Email: {Email}", customer.Email);

            var existing = _repository.GetAll();
            var result = _validator.Validate(customer, existing);

            if (!result.IsValid)
            {
                _logger.LogWarning("AddCustomer validation failed. Errors: {Errors}", result.Errors);

                return new CustomerAddResponse
                {
                    Result = CustomerAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _repository.Add(customer);
            _unitOfWork.Save();

            _logger.LogInformation("Customer added successfully. Id: {CustomerId}", customer.Id);

            return new CustomerAddResponse
            {
                Result = CustomerAddResult.Success
            };
        }

        // ================= EDIT =================

        public CustomerEditResult EditCustomer(Customer customer)
        {
            _logger.LogInformation("Starting EditCustomer. Id: {CustomerId}", customer.Id);

            var existing = _repository.GetById(customer.Id);

            if (existing == null)
            {
                _logger.LogWarning("Customer not found. Id: {CustomerId}", customer.Id);
                return CustomerEditResult.NotFound;
            }

            if (existing.IsCustomerArchived)
            {
                _logger.LogWarning("Attempt to edit archived customer. Id: {CustomerId}", customer.Id);
                return CustomerEditResult.CustomerArchived;
            }

            var otherCustomers = _repository
                .GetAll()
                .Where(x => x.Id != customer.Id)
                .ToList();

            var result = _validator.Validate(customer, otherCustomers);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditCustomer validation failed. Id: {CustomerId}, Errors: {Errors}",
                    customer.Id, result.Errors);

                return CustomerEditResult.InvalidData;
            }

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.City = customer.City;
            existing.Street = customer.Street;
            existing.ZipCode = customer.ZipCode;

            _repository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Customer updated successfully. Id: {CustomerId}", customer.Id);

            return CustomerEditResult.Success;
        }

        // ================= READ =================

        public List<Customer> GetAllCustomers()
        {
            _logger.LogInformation("Fetching all customers");
            return _repository.GetAll();
        }

        public Customer? FindCustomer(int id)
        {
            _logger.LogInformation("Finding customer. Id: {CustomerId}", id);
            return _repository.GetById(id);
        }

        // ================= ARCHIVE =================

        public ArchiveCustomerResult ArchiveCustomer(int id)
        {
            _logger.LogInformation("Archiving customer. Id: {CustomerId}", id);

            var existing = _repository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Customer not found for archive. Id: {CustomerId}", id);
                return ArchiveCustomerResult.NotFound;
            }

            existing.IsCustomerArchived = true;

            _repository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Customer archived successfully. Id: {CustomerId}", id);

            return ArchiveCustomerResult.Success;
        }
    }
}