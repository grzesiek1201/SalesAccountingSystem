using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AccountingSystem.Tests.ValidationTests
{
    public class QuotationValidatorTests
    {
        private readonly QuotationValidator _validator = new();

        private Quotation CreateValidQuotation()
        {
            return new Quotation
            {
                Id = 1,
                QuotationNumber = "Q-2026-001",
                Status = QuotationStatus.Draft,
                DateCreated = new DateTime(2026, 1, 1),

                Customer = new Customer { Id = 1 },

                Items = new List<QuotationItem>
                {
                    new QuotationItem
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
        public void ValidQuotation_ReturnsNoErrors()
        {
            var result = _validator.Validate(CreateValidQuotation(), new List<Quotation>());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyCustomer_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Customer = null;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NoItems_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Items = new List<QuotationItem>();

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void NullItems_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Items = null;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidQuantity_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Items.First().Quantity = 0;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidPrice_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Items.First().BaseUnitPrice = -1;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDiscount_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.Items.First().DiscountPercent = 200;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void EmptyNumber_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.QuotationNumber = "";

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void InvalidDate_ReturnsError()
        {
            var q = CreateValidQuotation();
            q.DateCreated = default;

            var result = _validator.Validate(q, new List<Quotation>());

            Assert.NotEmpty(result.Errors);
        }
    }
}