using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Xunit;
using System.Collections.Generic;

namespace AccountingSystem.Tests.ValidationTests
{
    public class CustomerValidatorTests
    {
        private readonly CustomerValidator _validator = new();

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
        public void Validate_ValidCustomer_ShouldReturnValidResult()
        {
            var customer = CreateValidCustomer();
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_EmptyName_ShouldContainEmptyNameError()
        {
            var customer = CreateValidCustomer();
            customer.Name = "";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyName, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_NameTooLong_ShouldContainNameTooLongError()
        {
            var customer = CreateValidCustomer();
            customer.Name = new string('A', 70);
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.NameTooLong, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_DuplicateName_ShouldContainDuplicateNameError()
        {
            var existing = CreateValidCustomer();
            var customers = new List<Customer> { existing };

            var customer = CreateValidCustomer();
            customer.Id = 2;

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.DuplicateName, result.Errors);
        }

        [Fact]
        public void Validate_EmptyZipCode_ShouldContainEmptyZipCodeError()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyZipCode, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_InvalidZipCode_ShouldContainInvalidZipCodeError()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "1000009";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.InvalidZipCode, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_NotDigitsZipCode_ShouldContainNotDigitsZipCodeError()
        {
            var customer = CreateValidCustomer();
            customer.ZipCode = "zip code";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.NotDigitsZipCode, result.Errors);
        }

        [Fact]
        public void Validate_EmptyCity_ShouldContainEmptyCityError()
        {
            var customer = CreateValidCustomer();
            customer.City = "";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyCity, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_CityTooLong_ShouldContainCityTooLongError()
        {
            var customer = CreateValidCustomer();
            customer.City = new string('A', 70);
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.CityTooLong, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_InvalidEmail_ShouldContainInvalidEmailError()
        {
            var customer = CreateValidCustomer();
            customer.Email = "email.email.com";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.InvalidEmail, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_EmptyEmail_ShouldContainEmptyEmailError()
        {
            var customer = CreateValidCustomer();
            customer.Email = "";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyEmail, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_EmailTooLong_ShouldContainEmailTooLongError()
        {
            var customer = CreateValidCustomer();
            customer.Email = new string('A', 70);
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmailTooLong, result.Errors);
        }

        [Fact]
        public void Validate_DuplicateEmail_ShouldContainDuplicateEmailError()
        {
            var existing = CreateValidCustomer();
            var customers = new List<Customer> { existing };

            var customer = CreateValidCustomer();
            customer.Id = 2;

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.DuplicateEmail, result.Errors);
        }

        [Fact]
        public void Validate_EmptyStreet_ShouldContainEmptyStreetError()
        {
            var customer = CreateValidCustomer();
            customer.Street = "";
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.EmptyStreet, result.Errors);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Validate_StreetTooLong_ShouldContainStreetTooLongError()
        {
            var customer = CreateValidCustomer();
            customer.Street = new string('A', 70);
            var customers = new List<Customer>();

            var result = _validator.Validate(customer, customers);

            Assert.Contains(CustomerValidationError.StreetTooLong, result.Errors);
            Assert.Single(result.Errors);
        }
    }
}