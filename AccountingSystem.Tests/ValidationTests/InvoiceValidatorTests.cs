using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AccountingSystem.Tests.ValidationTests
{
    public class InvoiceValidatorTests
    {
        private readonly InvoiceValidator _validator = new();

        private Invoice CreateValidInvoice()
        {
            return new Invoice
            {
                Id = 1,
                InvoiceNumber = "F-2026-001",
                Status = InvoiceStatus.Draft,

                DateCreated = new DateTime(2026, 1, 1),
                IssueDate = new DateTime(2026, 1, 1),
                DueDate = new DateTime(2026, 1, 15),

                Customer = new Customer { Id = 1 },

                Items = new List<InvoiceItem>
        {
            new InvoiceItem
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
        public void ValidInvoice_ReturnsNoErrors()
        {
            var result = _validator.Validate(CreateValidInvoice(), new List<Invoice>());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyCustomer_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Customer = null;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NoItems_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Items = new List<InvoiceItem>();

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NullItems_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Items = null;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidQuantity_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Items.First().Quantity = 0;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidPrice_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Items.First().BaseUnitPrice = -10;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDiscount_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.Items.First().DiscountPercent = 150;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void EmptyNumber_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.InvoiceNumber = "";

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDate_ReturnsError()
        {
            var inv = CreateValidInvoice();
            inv.IssueDate = default;

            var result = _validator.Validate(inv, new List<Invoice>());
            Assert.NotEmpty(result.Errors);
        }
    }
}