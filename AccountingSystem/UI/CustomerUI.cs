using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AccountingSystem.UI
{
    internal class CustomerUI
    {
        private CustomerService _customerService;

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
                    Console.WriteLine(error);
                }
                return;
            }

            Console.WriteLine("Customer added successfully");
        }

        public void EditCustomerFlow()
        {
            Console.WriteLine("To edit customer just fill fields below. If there is a customer matching your Id, data will change");

            var customer = GetCustomerInput();
            _customerService.EditCustomer(customer);
        }

        public void GetAllCustomerFlow()
        {
            List<Customer> customers = _customerService.GetAllCustomers();

            foreach (var c in customers)
            {
                Console.WriteLine(
                    $"Name: {c.Name}, Id: {c.Id}, Email: {c.Email},\n" +
                    $"Zip Code: {c.ZipCode}, Street: {c.Street}, City: {c.City},\n" +
                    $"In debt: {c.InDebt}"
                );
            }
        }

        public void FindCustomerFlow()
        {
            int idSearch = GetCustomerId();

            var result = _customerService.FindCustomer(idSearch);

            if (result != null)
            {
                Console.WriteLine(
                    $"Name: {result.Name}, Id: {result.Id}, Email: {result.Email},\n" +
                    $"Zip Code: {result.ZipCode}, Street: {result.Street}, City: {result.City},\n" +
                    $"In debt: {result.InDebt}"
                );
            }
            else
            {
                Console.WriteLine("Customer not found. Try again");
            }
        }

        public void ArchiveCustomerFlow()
        {
            int id = GetCustomerId();

            var result = _customerService.ArchiveCustomer(id);

            if (result == Domain.Enums.ArchiveCustomerResult.NotFound)
            {
                Console.WriteLine("Customer is not found. Try again");
            }
            else if (result == Domain.Enums.ArchiveCustomerResult.CustomerInDebt)
            {
                Console.WriteLine("Customer didn't pay all debts. Only customers without debt can be archived");
            }
            else if (result == Domain.Enums.ArchiveCustomerResult.Success)
            {
                Console.WriteLine("Customer has been archived.");
            }
        }

        public Customer GetCustomerInput()
        {
            Console.Write("add name of the company/client: ");
            string name = Console.ReadLine();

            Console.Write("add zip code: ");
            string zip = Console.ReadLine();

            Console.Write("add city: ");
            string city = Console.ReadLine();

            Console.Write("add street: ");
            string street = Console.ReadLine();

            Console.Write("add email address: ");
            string email = Console.ReadLine();

            return new Customer
            {
                Name = name,
                ZipCode = zip,
                City = city,
                Street = street,
                Email = email
            };
        }

        public int GetCustomerId()
        {
            Console.Write("Type customer ID: ");
            return Convert.ToInt32(Console.ReadLine());
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
                CustomerValidationError.DuplicateName => "Name is duplicated",
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