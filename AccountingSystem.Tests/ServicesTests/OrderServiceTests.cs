using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<NumberSequenceService> _seqMock;

        private readonly OrderValidator _validator;
        private readonly OrderService _service;


        public OrderServiceTests()
        {
            _repoMock = new Mock<IOrderRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _seqMock = new Mock<NumberSequenceService>();


            _validator = new OrderValidator();

            _service = new OrderService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object,
                _seqMock.Object
            );
        }

        private Order CreateValidOrder()
        {
            return new Order
            {
                Id = 1,
                OrderNumber = "O-2026-001",
                Status = OrderStatus.Draft,
                DateCreated = new DateTime(2026, 1, 1),

                CustomerId = 1,
                Customer = new Customer
                {
                    Id = 1,
                    Name = "Jan Kowalski",
                    Email = "jan@test.com",
                    City = "Warszawa",
                    Street = "Wąska 12",
                    ZipCode = "21222",
                    InDebt = false,
                    IsCustomerArchived = false
                },

                IsOrderArchived = false,

                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = 1,
                        ProductId = 1,
                        Product = new Product { Id = 1 },
                        Position = 1,
                        Quantity = 2,
                        BaseUnitPrice = 100m,
                        DiscountPercent = 0
                    }
                }
            };
        }

        private Order CreateInvalidOrder_NoItems()
        {
            var order = CreateValidOrder();
            order.Items = new List<OrderItem>();
            return order;
        }

        private Order CreateInvalidOrder_BrokenItem()
        {
            var order = CreateValidOrder();

            order.Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = 1,
                    ProductId = 1,
                    Product = new Product { Id = 1 },
                    Position = 1,
                    Quantity = 0,
                    BaseUnitPrice = 100m,
                    DiscountPercent = 0
                }
            };

            return order;
        }

        private Order CreateArchivedOrder()
        {
            var order = CreateValidOrder();
            order.IsOrderArchived = true;
            return order;
        }

        // ---------------- ADD ----------------

        [Fact]
        public void AddOrder_Valid_ShouldReturnSuccess()
        {
            var order = CreateValidOrder();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.AddOrder(order);

            Assert.Equal(OrderAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(order), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddOrder_Invalid_ShouldReturnInvalidData()
        {
            var order = CreateInvalidOrder_NoItems();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.AddOrder(order);

            Assert.Equal(OrderAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- EDIT ----------------

        [Fact]
        public void EditOrder_NotFound_ShouldReturnNotFound()
        {
            var order = CreateValidOrder();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns((Order)null);

            var result = _service.EditOrder(order);

            Assert.Equal(OrderEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditOrder_Archived_ShouldReturnOrderArchived()
        {
            var order = CreateArchivedOrder();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            var result = _service.EditOrder(order);

            Assert.Equal(OrderEditResult.OrderArchived, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditOrder_Invalid_ShouldReturnInvalidData()
        {
            var order = CreateInvalidOrder_BrokenItem();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.EditOrder(order);

            Assert.Equal(OrderEditResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditOrder_Valid_ShouldReturnSuccess()
        {
            var order = CreateValidOrder();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order>());

            var result = _service.EditOrder(order);

            Assert.Equal(OrderEditResult.Success, result.Result);

            _repoMock.Verify(r => r.Update(order), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveOrder_Existing_ShouldReturnSuccess()
        {
            var order = CreateValidOrder();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            var result = _service.ArchiveOrder(order.Id);

            Assert.Equal(ArchiveOrderResult.Success, result);
        }

        // ---------------- FIND ----------------

        [Fact]
        public void FindOrder_Existing_ShouldReturnOrder()
        {
            var order = CreateValidOrder();

            _repoMock.Setup(r => r.GetById(order.Id))
                .Returns(order);

            var result = _service.FindOrder(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAllOrders_ShouldReturnAllOrders()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Order> { CreateValidOrder(), CreateValidOrder() });

            var result = _service.GetAllOrders();

            Assert.Equal(2, result.Count);
        }
    }
}