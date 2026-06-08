using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
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

        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _paymentRepoMock = new Mock<IPaymentRepository>();
            _invoiceRepoMock = new Mock<IInvoiceRepository>();
            _uowMock = new Mock<IUnitOfWork>();

            _invoiceServiceMock = new Mock<InvoiceService>(null, null, null);

            _service = new PaymentService(
                _paymentRepoMock.Object,
                _invoiceRepoMock.Object,
                _uowMock.Object,
                _invoiceServiceMock.Object);
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

        // ---------------- ADD ----------------

        [Fact]
        public void AddPayment_InvoiceNotFound_ShouldReturnInvoiceNotFound()
        {
            var payment = CreateValidPayment();

            _invoiceRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Invoice)null);

            var result = _service.AddPayment(1, payment);

            Assert.Equal(PaymentAddResult.InvoiceNotFound, result);

            _paymentRepoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddPayment_InvoiceArchived_ShouldReturnInvoiceArchived()
        {
            var invoice = CreateValidInvoice();
            invoice.IsInvoiceArchived = true;

            _invoiceRepoMock
                .Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var payment = CreateValidPayment();

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.InvoiceArchived, result);

            _paymentRepoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddPayment_InvalidAmount_ShouldReturnInvalidAmount()
        {
            var invoice = CreateValidInvoice();

            var payment = CreateValidPayment();
            payment.Amount = 0;

            _invoiceRepoMock
                .Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.InvalidAmount, result);

            _paymentRepoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddPayment_AmountExceedsRemaining_ShouldReturnAmountExceedsRemaining()
        {
            var invoice = CreateValidInvoice();

            invoice.Payments.Add(new Payment { Amount = 900m });

            var payment = CreateValidPayment();
            payment.Amount = 200m;

            _invoiceRepoMock
                .Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.AmountExceedsRemaining, result);

            _paymentRepoMock.Verify(r => r.Add(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddPayment_Valid_ShouldReturnSuccess()
        {
            var invoice = CreateValidInvoice();
            var payment = CreateValidPayment();

            _invoiceRepoMock
                .Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.AddPayment(invoice.Id, payment);

            Assert.Equal(PaymentAddResult.Success, result);

            _paymentRepoMock.Verify(r => r.Add(payment), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ---------------- GET PAYMENTS ----------------

        [Fact]
        public void GetPaymentsForInvoice_ShouldReturnInvoicePayments()
        {
            _paymentRepoMock
                .Setup(r => r.GetByInvoiceId(1))
                .Returns(new List<Payment>
                {
                    new Payment { Id = 1, InvoiceId = 1, Amount = 100 },
                    new Payment { Id = 2, InvoiceId = 1, Amount = 200 }
                });

            var result = _service.GetPaymentsForInvoice(1);

            Assert.Equal(2, result.Count());
        }

        // ---------------- DELETE ----------------

        [Fact]
        public void DeletePayment_PaymentNotFound_ShouldDoNothing()
        {
            _paymentRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Payment)null);

            _service.DeletePayment(1);

            _paymentRepoMock.Verify(r => r.Delete(It.IsAny<Payment>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void DeletePayment_Valid_ShouldReturnSuccess()
        {
            var payment = new Payment
            {
                Id = 1,
                InvoiceId = 1,
                Amount = 100
            };

            _paymentRepoMock
                .Setup(r => r.GetById(payment.Id))
                .Returns(payment);

            _invoiceRepoMock
                .Setup(r => r.GetById(payment.InvoiceId))
                .Returns(CreateValidInvoice());

            _service.DeletePayment(payment.Id);

            _paymentRepoMock.Verify(r => r.Delete(payment), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }
    }
}