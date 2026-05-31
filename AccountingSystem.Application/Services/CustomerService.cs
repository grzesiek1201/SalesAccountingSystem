using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;

namespace AccountingSystem.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(
            ICustomerRepository repository,
            CustomerValidator validator,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public CustomerAddResponse AddCustomer(Customer customer)
        {
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

        public CustomerEditResult EditCustomer(Customer customer)
        {
            var existing = _repository.GetById(customer.Id);

            if (existing == null)
                return CustomerEditResult.NotFound;

            if (existing.IsCustomerArchived)
                return CustomerEditResult.CustomerArchived;

            var others = _repository.GetAll()
                .Where(x => x.Id != customer.Id)
                .ToList();

            var result = _validator.Validate(customer, others);

            if (!result.IsValid)
                return CustomerEditResult.InvalidData;

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.City = customer.City;
            existing.Street = customer.Street;
            existing.ZipCode = customer.ZipCode;

            _repository.Update(existing);
            _unitOfWork.Save();

            return CustomerEditResult.Success;
        }

        public List<Customer> GetAllCustomers()
            => _repository.GetAll();

        public Customer? FindCustomer(int id)
            => _repository.GetById(id);

        public ArchiveCustomerResult ArchiveCustomer(int id)
        {
            var existing = _repository.GetById(id);

            if (existing == null)
                return ArchiveCustomerResult.NotFound;

            existing.IsCustomerArchived = true;

            _repository.Update(existing);
            _unitOfWork.Save();

            return ArchiveCustomerResult.Success;
        }
    }
}