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

        public CustomerAddResponse AddCustomer(CreateCustomerRequest request)
        {
            _logger.LogInformation("Starting AddCustomer. Email: {Email}", request.Email);

            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                City = request.City,
                Street = request.Street,
                ZipCode = request.ZipCode
            };

            var existing = _repository.GetAll();
            var result = _validator.Validate(customer, existing);

            if (!result.IsValid)
            {
                return new CustomerAddResponse
                {
                    Result = CustomerAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _repository.Add(customer);
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

            var existing = _repository.GetById(request.Id);

            if (existing == null)
            {
                return new CustomerEditResponse
                {
                    Result = CustomerEditResult.NotFound
                };
            }

            if (existing.IsCustomerArchived)
            {
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

            var otherCustomers = _repository
                .GetAll()
                .Where(x => x.Id != existing.Id)
                .ToList();

            var validation = _validator.Validate(existing, otherCustomers);

            if (!validation.IsValid)
            {
                return new CustomerEditResponse
                {
                    Result = CustomerEditResult.InvalidData,
                    Errors = validation.Errors
                };
            }

            _repository.Update(existing);
            _unitOfWork.Save();

            return new CustomerEditResponse
            {
                Result = CustomerEditResult.Success
            };
        }

        // ================= READ =================

        public List<CustomerResponse> GetAllCustomers()
        {
            return _repository.GetAll()
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
            var customer = _repository.GetById(id);

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
            var existing = _repository.GetById(id);

            if (existing == null)
            {
                return CustomerArchiveResult.NotFound;
            }

            existing.IsCustomerArchived = true;

            _repository.Update(existing);
            _unitOfWork.Save();

            return CustomerArchiveResult.Success;
        }
    }
}