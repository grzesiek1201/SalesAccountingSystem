using AccountingSystem.AccountingSystem.Application.DTOs;
using AccountingSystem.AccountingSystem.Application.Validation.Customers;
using AccountingSystem.AccountingSystem.Domain.Enums;
using AccountingSystem.AccountingSystem.Infrastructure.Data;
using AccountingSystem.AccountingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountingSystem.AccountingSystem.Application.Services
{
    internal class CustomerService
    {
        private readonly AppDbContext _context;
        private readonly CustomerValidator _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(
            AppDbContext context,
            CustomerValidator validator,
            IUnitOfWork unitOfWork)
        {
            _context = context;
            _validator = validator;
            _unitOfWork = unitOfWork;
        }

        public CustomerAddResponse AddCustomer(Customer customer)
        {
            var existingCustomers = _context.Customers.AsNoTracking().ToList();

            var result = _validator.Validate(customer, existingCustomers);

            if (!result.IsValid)
            {
                return new CustomerAddResponse
                {
                    Result = CustomerAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            _context.Customers.Add(customer);

            _unitOfWork.Save();

            return new CustomerAddResponse
            {
                Result = CustomerAddResult.Success
            };
        }

        public CustomerEditResult EditCustomer(Customer customer)
        {
            var existing = _context.Customers
                .FirstOrDefault(x => x.Id == customer.Id);

            if (existing == null)
                return CustomerEditResult.NotFound;

            if (existing.IsCustomerArchived)
                return CustomerEditResult.CustomerArchived;

            var otherCustomers = _context.Customers
                .AsNoTracking()
                .Where(x => x.Id != customer.Id)
                .ToList();

            var result = _validator.Validate(customer, otherCustomers);

            if (!result.IsValid)
                return CustomerEditResult.InvalidData;

            existing.Name = customer.Name;
            existing.Email = customer.Email;
            existing.Street = customer.Street;
            existing.City = customer.City;
            existing.ZipCode = customer.ZipCode;

            _unitOfWork.Save();

            return CustomerEditResult.Success;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers
                .AsNoTracking()
                .ToList();
        }

        public Customer? FindCustomer(int id)
        {
            return _context.Customers
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public ArchiveCustomerResult ArchiveCustomer(int id)
        {
            var existing = _context.Customers
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return ArchiveCustomerResult.NotFound;

            existing.IsCustomerArchived = true;

            _unitOfWork.Save();

            return ArchiveCustomerResult.Success;
        }
    }
}