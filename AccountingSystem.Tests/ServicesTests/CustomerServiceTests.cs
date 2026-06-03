using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly CustomerValidator _validator;

        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repoMock = new Mock<ICustomerRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _validator = new CustomerValidator();

            _service = new CustomerService(
                _repoMock.Object,
                _validator,
                _uowMock.Object
            );
        }

        [Fact]
        public void AddCustomer_ValidCustomer_ReturnsSuccess()
        {
            // Arrange
            var customer = new Customer
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

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            // Act
            var result = _service.AddCustomer(customer);

            // Assert
            Assert.Equal(Domain.Enums.CustomerAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(customer), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddCustomer_InvalidCustomer_ReturnsInvalidData()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "", // invalid
                Email = "bad-email"
                City = "Warszawa",
                Street = "Wąska 12",
                ZipCode = "21222",
                InDebt = false,
                IsCustomerArchived = false
            };

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            // Act
            var result = _service.AddCustomer(customer);

            // Assert
            Assert.Equal(Domain.Enums.CustomerAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddCustomer_DuplicateEmail_ReturnsInvalidData()
        {
            // Arrange
            var customer = new Customer
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

            // Act
            var result = _service.AddCustomer(customer);


            // Assert
            Assert.Equal(Domain.Enums.CustomerAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }
    }
}
