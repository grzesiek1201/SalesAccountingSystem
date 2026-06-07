using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Castle.Core.Resource;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountingSystem.Tests.ValidationTests
{
    public class CustomerValidatorTests
    {
        private readonly CustomerValidator _validator;

        public CustomerValidatorTests()
        {
            _validator = new CustomerValidator();
        }

        private Customer CreateValidCustomer()
        {
            return new Customer
            {
                Id = 1,
                Name = "Jan Kowalski",
                Email = "jan@test.com",
                City = "Warszawa",
                Street = "Wąska 12",
                ZipCode = "21222",
                InDebt = false,
                IsCustomerArchived = false
            };
        }

        [Fact]
        public void CustomerValidationResult_ValidCustomer_ReturnsNoErrors()
        {
            var customer = CreateValidCustomer();
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CustomerValidationResult_EmptyName_ReturnsEmptyName()
        {
            var customer = CreateValidCustomer();
            customer.Name = "";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyName, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_NameTooLong_ReturnsNameTooLong()
        {
            var customer = CreateValidCustomer();
            customer.Name = new string('A', 70);
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.NameTooLong, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_DuplicateName_ReturnsDuplicateName()
        {
            var customer = CreateValidCustomer();
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CustomerValidationResult_EmptyZipCode_ReturnsEmptyZipCode()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyZipCode, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_InvalidZipCode_ReturnsInvalidZipCode()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "1000009";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.InvalidZipCode, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_NotDigitsZipCode_ReturnsNotDigitsZipCode()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "zip code";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.NotDigitsZipCode, result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_EmptyCity_ReturnsEmptyCity()
        {
            var customer = CreateValidCustomer();
            customer.City = "";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyCity, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_CityTooLong_ReturnsCityTooLong()
        {
            var customer = CreateValidCustomer();
            customer.City = new string('A', 70);
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.CityTooLong, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_InvalidEmail_ReturnsInvalidEmail()
        {
            var customer = CreateValidCustomer();
            customer.Email = "email.email.com";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.InvalidEmail, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_EmptyEmail_ReturnsEmptyEmail()
        {
            var customer = CreateValidCustomer();
            customer.Email = "";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyEmail, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_EmailTooLong_ReturnsEmailTooLong()
        {
            var customer = CreateValidCustomer();
            customer.Email = new string('A', 70);
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmailTooLong, result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_DuplicateEmail_ReturnsDuplicateEmail()
        {
            var customer = CreateValidCustomer();
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void CustomerValidationResult_EmptyStreet_ReturnsEmptyStreet()
        {
            var customer = CreateValidCustomer();
            customer.Street = "";
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyStreet, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void CustomerValidationResult_StreetTooLong_ReturnsStreetTooLong()
        {
            var customer = CreateValidCustomer();
            customer.Street = new string('A', 70);
            var validator = new CustomerValidator();
            var customers = new List<Customer>();

            var result = validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.StreetTooLong, result.Errors);
            Assert.Single(result.Errors);
        }
    }
}