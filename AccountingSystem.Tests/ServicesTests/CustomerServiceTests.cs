using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountingSystem.Tests.ServicesTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;

        private readonly CustomerValidator _validator;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repoMock = new Mock<ICustomerRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _validator = new CustomerValidator();

            _service = new CustomerService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object
            );
        }

        // ================= HELPERS =================

        private CreateCustomerRequest CreateValidRequest()
        {
            return new CreateCustomerRequest
            {
                Name = "Jan Kowalski",
                Email = "jan@test.com",
                City = "Warszawa",
                Street = "Wąska 12",
                ZipCode = "21222"
            };
        }

        private UpdateCustomerRequest CreateValidUpdateRequest(int id = 1)
        {
            return new UpdateCustomerRequest
            {
                Id = id,
                Name = "Jan Kowalski",
                Email = "jan@test.com",
                City = "Warszawa",
                Street = "Wąska 12",
                ZipCode = "21222"
            };
        }

        private Customer CreateDomainCustomer()
        {
            return new Customer
            {
                Id = 1,
                Name = "Jan Kowalski",
                Email = "jan@test.com",
                City = "Warszawa",
                Street = "Wąska 12",
                ZipCode = "21222",
                IsCustomerArchived = false
            };
        }

        // ================= ADD =================

        [Fact]
        public void AddCustomer_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidRequest();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.AddCustomer(request);

            Assert.Equal(CustomerAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddCustomer_Invalid_ShouldReturnInvalidData()
        {
            var request = CreateValidRequest();
            request.Email = "bad-email";

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>());

            var result = _service.AddCustomer(request);

            Assert.Equal(CustomerAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
        }

        // ================= EDIT =================

        [Fact]
        public void EditCustomer_NotFound_ShouldReturnNotFound()
        {
            var request = CreateValidUpdateRequest();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns((Customer)null);

            var result = _service.EditCustomer(request);

            Assert.Equal(CustomerEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditCustomer_Archived_ShouldReturnArchived()
        {
            var request = CreateValidUpdateRequest();

            var customer = CreateDomainCustomer();
            customer.IsCustomerArchived = true;

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(customer);

            var result = _service.EditCustomer(request);

            Assert.Equal(CustomerEditResult.CustomerArchived, result.Result);
        }

        [Fact]
        public void EditCustomer_Valid_ShouldReturnSuccess()
        {
            var request = CreateValidUpdateRequest();

            var customer = CreateDomainCustomer();

            _repoMock.Setup(r => r.GetById(request.Id))
                .Returns(customer);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer> { customer });

            var result = _service.EditCustomer(request);

            Assert.Equal(CustomerEditResult.Success, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Customer>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ================= ARCHIVE =================

        [Fact]
        public void ArchiveCustomer_ShouldReturnSuccess()
        {
            var customer = CreateDomainCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            var result = _service.ArchiveCustomer(customer.Id);

            Assert.Equal(CustomerArchiveResult.Success, result);
            Assert.True(customer.IsCustomerArchived);
        }

        // ================= READ =================

        [Fact]
        public void GetAllCustomers_ShouldReturnList()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Customer>
                {
                    CreateDomainCustomer(),
                    CreateDomainCustomer()
                });

            var result = _service.GetAllCustomers();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetCustomerById_ShouldReturnCustomer()
        {
            var customer = CreateDomainCustomer();

            _repoMock.Setup(r => r.GetById(customer.Id))
                .Returns(customer);

            var result = _service.GetCustomerById(customer.Id);

            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
        }

        [Fact]
        public void GetCustomerById_NotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Customer)null);

            var result = _service.GetCustomerById(1);

            Assert.Null(result);
        }
    }
}