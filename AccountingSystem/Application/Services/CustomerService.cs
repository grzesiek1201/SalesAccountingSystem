using AccountingSystem.Application.DTOs;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;


namespace AccountingSystem.Application.Services
{
    internal class CustomerService
    {
        private List<Customer> customers = new List<Customer>();
        public int nextId;
        private readonly CustomerValidator _validator;
        public CustomerService(CustomerValidator validator)
        {
           _validator = validator;
        }
        public CustomerAddResponse AddCustomer(Customer customer)
        {
            var result = _validator.Validate(customer, customers);

            if (!result.IsValid)
            {
                return new CustomerAddResponse
                {
                    Result = CustomerAddResult.InvalidData,
                    Errors = result.Errors
                };
            }

            customer.Id = nextId;
            nextId++;

            customers.Add(customer);

            return new CustomerAddResponse
            {
                Result = CustomerAddResult.Success
            };
        }

        public Domain.Enums.CustomerEditResult EditCustomer(Customer customer)
        {
            var existing = customers.Find(x => x.Id == customer.Id);

            if (existing == null)
                return Domain.Enums.CustomerEditResult.NotFound;

            if (existing.IsCustomerArchived)
                return Domain.Enums.CustomerEditResult.CustomerArchived;

            var otherCustomers = customers.Where(x => x.Id != customer.Id).ToList();
            var result = _validator.Validate(customer, otherCustomers);

            if (!result.IsValid)
                return Domain.Enums.CustomerEditResult.InvalidData;

            existing.Name = customer.Name;
            existing.Street = customer.Street;
            existing.ZipCode = customer.ZipCode;
            existing.City = customer.City;
            existing.Email = customer.Email;

            return Domain.Enums.CustomerEditResult.Success;
        }

        public List<Customer> GetAllCustomers()
        {
            return customers;
        }

        public Customer? FindCustomer(int Id)  
        {
            return customers.FirstOrDefault(x => x.Id == Id);
        }

        public Domain.Enums.ArchiveCustomerResult ArchiveCustomer(int Id)
        {
            var existing = customers.Find(x => x.Id == Id);
            if (existing == null)
            {
                return Domain.Enums.ArchiveCustomerResult.NotFound;
            }

            if (existing.InDebt == true)
            {
                return Domain.Enums.ArchiveCustomerResult.CustomerInDebt;
            }
            existing.IsCustomerArchived = true;
            return Domain.Enums.ArchiveCustomerResult.Success;
        }
    }
}
