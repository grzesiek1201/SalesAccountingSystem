using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
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

        // ---------------- ADD ----------------

        [Fact]
        public void AddCustomer_ValidCustomer_ShouldReturnSuccess()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.AddCustomer(customer);

            Assert.Equal(CustomerAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(customer), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddCustomer_InvalidCustomer_ShouldReturnInvalidData()
        {
            var customer = CreateValidCustomer();
            customer.Email = "bad-email";

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.AddCustomer(customer);

            Assert.Equal(CustomerAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddCustomer_DuplicateEmail_ShouldReturnInvalidData()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>
                {
                    new Customer { Id = 999, Email = "jan@test.com" }
                });

            var result = _service.AddCustomer(customer);

            Assert.Equal(CustomerAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- EDIT ----------------

        [Fact]
        public void EditCustomer_ValidCustomer_ShouldReturnSuccess()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.EditCustomer(customer);

            Assert.Equal(CustomerEditResult.Success, result);

            _repoMock.Verify(r => r.Update(customer), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void EditCustomer_CustomerNotFound_ShouldReturnNotFound()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns((Customer)null);

            var result = _service.EditCustomer(customer);

            Assert.Equal(CustomerEditResult.NotFound, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditCustomer_ArchivedCustomer_ShouldReturnCustomerArchived()
        {
            var customer = CreateValidCustomer();
            customer.IsCustomerArchived = true;

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            var result = _service.EditCustomer(customer);

            Assert.Equal(CustomerEditResult.CustomerArchived, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditCustomer_InvalidCustomer_ShouldReturnInvalidData()
        {
            var customer = CreateValidCustomer();
            customer.Email = "bad-email";

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.EditCustomer(customer);

            Assert.Equal(CustomerEditResult.InvalidData, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }


        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveCustomer_ValidCustomer_ShouldReturnSuccess()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            var result = _service.ArchiveCustomer(customer.Id);

            Assert.Equal(ArchiveCustomerResult.Success, result);
            Assert.True(customer.IsCustomerArchived);

            _repoMock.Verify(r => r.Update(customer), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void ArchiveCustomer_CustomerNotFound_ShouldReturnNotFound()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Customer)null);

            var result = _service.ArchiveCustomer(1);

            Assert.Equal(ArchiveCustomerResult.NotFound, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- FIND ----------------

        [Fact]
        public void FindCustomer_ExistingId_ShouldReturnCustomer()
        {
            var customer = CreateValidCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            var result = _service.FindCustomer(customer.Id);

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
        }

        [Fact]
        public void FindCustomer_NonExistingId_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Customer)null);

            var result = _service.FindCustomer(1);

            Assert.Null(result);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAllCustomers_ShouldReturnCustomersFromRepository()
        {
            var customers = new List<Customer>
            {
                CreateValidCustomer(),
                CreateValidCustomer()
            };

            _repoMock.Setup(r => r.GetAll())
                .Returns(customers);

            var result = _service.GetAllCustomers();

            Assert.Equal(2, result.Count);
        }
    }
}