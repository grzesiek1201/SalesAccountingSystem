using AccountingSystem.Application.Validation.Payments;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ValidationTests
{
    public class PaymentValidatorTests
    {
        private readonly Mock<IInvoiceRepository> _repoMock = new();
        private readonly PaymentValidator _validator;

        public PaymentValidatorTests()
        {
            _validator = new PaymentValidator(_repoMock.Object);
        }

        private Invoice CreateValidInvoice(decimal totalAmount = 100m, bool archived = false)
        {
            return new Invoice
            {
                Id = 1,
                TotalAmount = totalAmount,
                IsInvoiceArchived = archived,
                Payments = new List<Payment>()
            };
        }

        private Payment CreateValidPayment(decimal amount = 50m)
        {
            return new Payment
            {
                Id = 1,
                Amount = amount,
                PaymentDate = new DateTime(2026, 1, 1),
                Method = AccountingSystem.Domain.Enums.PaymentMethod.Cash,
                Status = AccountingSystem.Domain.Enums.PaymentStatus.Completed
            };
        }

        [Fact]
        public void ValidPayment_ReturnsNoErrors()
        {
            var invoice = CreateValidInvoice();
            var payment = CreateValidPayment();

            _repoMock.Setup(r => r.GetById(1)).Returns(invoice);

            var result = _validator.Validate(1, payment);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void InvoiceNotFound_ReturnsError()
        {
            _repoMock.Setup(r => r.GetById(1)).Returns((Invoice)null);

            var result = _validator.Validate(1, CreateValidPayment());

            Assert.Contains(PaymentValidationError.InvoiceNotFound, result.Errors);
        }

        [Fact]
        public void ArchivedInvoice_ReturnsError()
        {
            _repoMock.Setup(r => r.GetById(1))
                .Returns(CreateValidInvoice(100m, archived: true));

            var result = _validator.Validate(1, CreateValidPayment());

            Assert.Contains(PaymentValidationError.InvoiceArchived, result.Errors);
        }

        [Fact]
        public void InvalidAmount_ReturnsError()
        {
            _repoMock.Setup(r => r.GetById(1))
                .Returns(CreateValidInvoice());

            var result = _validator.Validate(1, CreateValidPayment(0));

            Assert.Contains(PaymentValidationError.InvalidAmount, result.Errors);
        }

        [Fact]
        public void AmountExceedsRemaining_ReturnsError()
        {
            var invoice = CreateValidInvoice(totalAmount: 100m);
            invoice.Payments = new List<Payment>
            {
                new Payment { Amount = 80m }
            };

            _repoMock.Setup(r => r.GetById(1)).Returns(invoice);

            var payment = CreateValidPayment(30m); 

            var result = _validator.Validate(1, payment);

            Assert.Contains(PaymentValidationError.AmountExceedsRemaining, result.Errors);
        }
    }
}