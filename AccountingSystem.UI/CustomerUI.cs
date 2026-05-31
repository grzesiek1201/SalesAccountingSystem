using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AccountingSystem.UI
{
    public class CustomerUI
    {
        private readonly CustomerService _customerService;

        public CustomerUI(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public void AddCustomerFlow()
        {
            var customer = GetCustomerInput();

            var response = _customerService.AddCustomer(customer);

            if (!response.IsSuccess)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine(GetCustomerErrorMessage(error));
                }
                return;
            }

            Console.WriteLine("Customer added successfully");
        }

        public void EditCustomerFlow()
        {
            Console.WriteLine("Edit customer - fill fields. Id must match existing record.");

            var customer = GetCustomerInput();

            var result = _customerService.EditCustomer(customer);

            switch (result)
            {
                case AccountingSystem.Domain.Enums.CustomerEditResult.Success:
                    Console.WriteLine("Customer updated successfully");
                    break;

                case AccountingSystem.Domain.Enums.CustomerEditResult.NotFound:
                    Console.WriteLine("Customer not found");
                    break;

                case AccountingSystem.Domain.Enums.CustomerEditResult.CustomerArchived:
                    Console.WriteLine("Customer is archived");
                    break;

                case AccountingSystem.Domain.Enums.CustomerEditResult.InvalidData:
                    Console.WriteLine("Invalid data");
                    break;
            }
        }

        public void GetAllCustomerFlow()
        {
            var customers = _customerService.GetAllCustomers();

            foreach (var c in customers)
            {
                PrintCustomer(c);
            }
        }

        public void FindCustomerFlow()
        {
            int id = GetCustomerId();

            var customer = _customerService.FindCustomer(id);

            if (customer == null)
            {
                Console.WriteLine("Customer not found");
                return;
            }

            PrintCustomer(customer);
        }

        public void ArchiveCustomerFlow()
        {
            int id = GetCustomerId();

            var result = _customerService.ArchiveCustomer(id);

            switch (result)
            {
                case AccountingSystem.Domain.Enums.ArchiveCustomerResult.NotFound:
                    Console.WriteLine("Customer not found");
                    break;

                case AccountingSystem.Domain.Enums.ArchiveCustomerResult.CustomerInDebt:
                    Console.WriteLine("Customer has unpaid debt");
                    break;

                case AccountingSystem.Domain.Enums.ArchiveCustomerResult.Success:
                    Console.WriteLine("Customer archived");
                    break;
            }
        }

        private Customer GetCustomerInput()
        {
            Console.Write("Company/client name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Zip code: ");
            string zip = Console.ReadLine() ?? "";

            Console.Write("City: ");
            string city = Console.ReadLine() ?? "";

            Console.Write("Street: ");
            string street = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";

            return new Customer
            {
                Name = name,
                ZipCode = zip,
                City = city,
                Street = street,
                Email = email
            };
        }

        private int GetCustomerId()
        {
            Console.Write("Customer ID: ");

            int id;

            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid number. Try again:");
            }

            return id;
        }

        private void PrintCustomer(Customer c)
        {
            Console.WriteLine(
                $"Id: {c.Id}, Name: {c.Name}, Email: {c.Email}\n" +
                $"City: {c.City}, Street: {c.Street}, Zip: {c.ZipCode}\n" +
                $"Archived: {c.IsCustomerArchived}"
            );
        }

        private string GetCustomerErrorMessage(CustomerValidationError error)
        {
            return error switch
            {
                CustomerValidationError.InvalidEmail => "Email format is invalid",
                CustomerValidationError.InvalidZipCode => "Zip code is invalid",
                CustomerValidationError.EmptyEmail => "Email is required",
                CustomerValidationError.EmptyName => "Name is empty",
                CustomerValidationError.NameTooLong => "Name is too long",
                CustomerValidationError.DuplicateName => "Name already exists",
                CustomerValidationError.EmptyZipCode => "Zip code is empty",
                CustomerValidationError.NotDigitsZipCode => "Zip code must contain only digits",
                CustomerValidationError.EmptyCity => "City is empty",
                CustomerValidationError.CityTooLong => "City is too long",
                CustomerValidationError.EmailTooLong => "Email is too long",
                CustomerValidationError.EmptyStreet => "Street is empty",
                CustomerValidationError.StreetTooLong => "Street is too long",
                _ => "Unknown error"
            };
        }
    }
}
