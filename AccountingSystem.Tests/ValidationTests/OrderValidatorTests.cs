using AccountingSystem.Application.Validation.Orders;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AccountingSystem.Tests.ValidationTests
{
    public class OrderValidatorTests
    {
        private readonly OrderValidator _validator = new();

        private Order CreateValidOrder()
        {
            return new Order
            {
                Id = 1,
                OrderNumber = "O-2026-001",
                Status = OrderStatus.Draft,
                DateCreated = new DateTime(2026, 1, 1),

                Customer = new Customer { Id = 1 },

                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Product = new Product { Id = 1 },
                        Quantity = 2,
                        BaseUnitPrice = 100m,
                        DiscountPercent = 0
                    }
                }
            };
        }

        [Fact]
        public void ValidOrder_ReturnsNoErrors()
        {
            var result = _validator.Validate(CreateValidOrder(), new List<Order>());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyCustomer_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Customer = null;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NoItems_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Items = new List<OrderItem>();

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NullItems_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Items = null;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidQuantity_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Items.First().Quantity = 0;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidPrice_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Items.First().BaseUnitPrice = -5;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDiscount_ReturnsError()
        {
            var order = CreateValidOrder();
            order.Items.First().DiscountPercent = 120;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void EmptyNumber_ReturnsError()
        {
            var order = CreateValidOrder();
            order.OrderNumber = "";

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDate_ReturnsError()
        {
            var order = CreateValidOrder();
            order.DateCreated = default;

            var result = _validator.Validate(order, new List<Order>());
            Assert.NotEmpty(result.Errors);
        }
    }
}