using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccountingSystem.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerValidator _validator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepository,
            CustomerValidator validator,
            IUnitOfWork unitOfWork,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _validator = validator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // ================= ADD =================

        public CustomerAddResponse AddCustomer(CreateCustomerRequest request)
        {
            _logger.LogInformation("Starting AddCustomer. Name: {Name}", request.Name);

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                City = request.City,
                Street = request.Street,
                ZipCode = request.ZipCode
            };

            var existing = _customerRepository.GetAll();
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

            _customerRepository.Add(customer);
            _unitOfWork.Save();

            return new CustomerAddResponse
            {
                Result = CustomerAddResult.Success
            };
        }

        // ================= EDIT =================

        public CustomerEditResponse EditCustomer(UpdateCustomerRequest request)
        {
            _logger.LogInformation("Starting EditCustomer. Id: {CustomerId}", request.Id);

            var existing = _customerRepository.GetById(request.Id);

            if (existing == null)
            {
                _logger.LogWarning("Customer not found. Id: {CustomerId}", request.Id);
                return new CustomerEditResponse
                {
                    Result = CustomerEditResult.NotFound
                };
            }

            if (existing.IsCustomerArchived)
            {
                _logger.LogWarning("Attempt to edit archived product. Id: {ProductId}", request.Id);
                return new CustomerEditResponse
                {
                    Result = CustomerEditResult.CustomerArchived
                };
            }

            existing.Name = request.Name;
            existing.Email = request.Email;
            existing.City = request.City;
            existing.Street = request.Street;
            existing.ZipCode = request.ZipCode;

            var otherCustomers = _customerRepository
                .GetAll()
                .Where(x => x.Id != existing.Id)
                .ToList();

            var result = _validator.Validate(existing, otherCustomers);

            if (!result.IsValid)
            {
                _logger.LogWarning("EditProduct validation failed. Id: {CustomerId}, Errors: {Errors}",
                    existing.Id, result.Errors);

                return new CustomerEditResponse
                {
                    Result = CustomerEditResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _customerRepository.Update(existing);
            _unitOfWork.Save();


            _logger.LogInformation("Customer updated successfully. Id: {CustomerId}", request.Id);


            return new CustomerEditResponse
            {
                Result = CustomerEditResult.Success
            };
        }

        // ================= READ =================

        public List<CustomerResponse> GetAllCustomers()
        {
            _logger.LogInformation("Fetching all customers");
            return _customerRepository.GetAll()
                .Select(c => new CustomerResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    City = c.City,
                    Street = c.Street,
                    ZipCode = c.ZipCode
                })
                .ToList();
        }

        public CustomerResponse? GetCustomerById(int id)
        {
            _logger.LogInformation("Finding customer. Id: {CustomerId}", id);
            var customer = _customerRepository.GetById(id);

            if (customer == null)
                return null;

            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                City = customer.City,
                Street = customer.Street,
                ZipCode = customer.ZipCode
            };
        }

        // ================= ARCHIVE =================

        public CustomerArchiveResult ArchiveCustomer(int id)
        {
            _logger.LogInformation("Archiving customer. Id: {CustomerId}", id);

            var existing = _customerRepository.GetById(id);

            if (existing == null)
            {
                _logger.LogWarning("Customer not found for archive. Id: {CustomerId}", id);
                return CustomerArchiveResult.NotFound;
            }

            existing.IsCustomerArchived = true;

            _customerRepository.Update(existing);
            _unitOfWork.Save();

            _logger.LogInformation("Customer archived successfully. Id: {CustomerId}", id);

            return CustomerArchiveResult.Success;
        }
    }
}