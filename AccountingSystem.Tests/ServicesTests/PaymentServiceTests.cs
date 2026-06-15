using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepoMock;
        private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
        private readonly Mock<IUnitOfWork> _uowMock;

        private readonly Mock<InvoiceService> _invoiceServiceMock;

        private readonly Mock<ILogger<PaymentService>> _loggerMock;

        private readonly PaymentService _service;


        public PaymentServiceTests()
        {
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _invoiceRepoMock = new Mock<IInvoiceRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            var invoiceLoggerMock = new Mock<ILogger<InvoiceService>>();

            var invoiceValidator = new InvoiceValidator();


            _invoiceServiceMock = new Mock<InvoiceService>(
                _invoiceRepoMock.Object,
                invoiceValidator,
                _uowMock.Object,
                invoiceLoggerMock.Object
            );


            _loggerMock = new Mock<ILogger<PaymentService>>();


            _service = new PaymentService(
                _paymentRepoMock.Object,
                _invoiceRepoMock.Object,
                _uowMock.Object,
                _invoiceServiceMock.Object,
                _loggerMock.Object
            );
        }

        private Invoice CreateValidInvoice()
        {
            return new Invoice
            {
                Id = 1,
                InvoiceNumber = "FV-2026-001",
                TotalAmount = 1000m,
                IsInvoiceArchived = false,
                Payments = new List<Payment>()
            };
        }

        private Payment CreateValidPayment()
        {
            return new Payment
            {
                Id = 1,
                Amount = 200m
            };
        }

        [Fact]
        public void AddPayment_InvoiceNotFound_ShouldReturnInvoiceNotFound()
        {
            _invoiceRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Invoice)null);

            var result = _service.AddPayment(1, CreateValidPayment());

            Assert.Equal(PaymentAddResult.InvoiceNotFound, result);

            _paymentRepoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddPayment_InvoiceArchived_ShouldReturnInvoiceArchived()
        {
            var invoice = CreateValidInvoice();
            invoice.IsInvoiceArchived = true;

            _invoiceRepoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.AddPayment(invoice.Id, CreateValidPayment());

            Assert.Equal(PaymentAddResult.InvoiceArchived, result);
        }

        [Fact]
        public void AddPayment_InvalidAmount_ShouldReturnInvalidAmount()
        {
            var invoice = CreateValidInvoice();

            _invoiceRepoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var payment = CreateValidPayment();
            payment.Amount = 0;

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.InvalidAmount, result);
        }

        [Fact]
        public void AddPayment_AmountExceedsRemaining_ShouldReturnAmountExceedsRemaining()
        {
            var invoice = CreateValidInvoice();
            invoice.Payments.Add(new Payment { Amount = 900m });

            _invoiceRepoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var payment = CreateValidPayment();
            payment.Amount = 200m;

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.AmountExceedsRemaining, result);
        }

        [Fact]
        public void AddPayment_Valid_ShouldReturnSuccess()
        {
            var invoice = CreateValidInvoice();

            _invoiceRepoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var payment = CreateValidPayment();

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.Success, result);

            _paymentRepoMock.Verify(r => r.Add(payment), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void GetPaymentsForInvoice_ShouldReturnPayments()
        {
            _paymentRepoMock.Setup(r => r.GetByInvoiceId(1))
                .Returns(new List<Payment>
                {
                    new Payment { Id = 1, InvoiceId = 1, Amount = 100 },
                    new Payment { Id = 2, InvoiceId = 1, Amount = 200 }
                });

            var result = _service.GetPaymentsForInvoice(1);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void DeletePayment_NotFound_ShouldDoNothing()
        {
            _paymentRepoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Payment)null);

            _service.DeletePayment(1);

            _paymentRepoMock.Verify(r => r.Delete(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void DeletePayment_Valid_ShouldDelete()
        {
            var payment = new Payment
            {
                Id = 1,
                InvoiceId = 1,
                Amount = 100
            };

            _paymentRepoMock.Setup(r => r.GetById(payment.Id))
                .Returns(payment);

            _invoiceRepoMock.Setup(r => r.GetById(payment.InvoiceId))
                .Returns(CreateValidInvoice());

            _service.DeletePayment(payment.Id);

            _paymentRepoMock.Verify(r => r.Delete(payment), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }
    }
}