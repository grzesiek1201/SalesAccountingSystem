using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly InvoiceValidator _validator;

        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _repoMock = new Mock<IInvoiceRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _validator = new InvoiceValidator();

            _service = new InvoiceService(
                _repoMock.Object,
                _validator,
                _uowMock.Object
            );
        }

        private Invoice CreateValidInvoice()
        {
            return new Invoice
            {
                Id = 1,
                InvoiceNumber = "INV-2026-001",
                Status = InvoiceStatus.Draft,
                DateCreated = new DateTime(2026, 1, 1),
                IssueDate = new DateTime(2026, 1, 1),
                DueDate = new DateTime(2026, 1, 15),
                TotalAmount = 200m,

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

        private Invoice CreateInvalidInvoice_NoItems()
        {
            var invoice = CreateValidInvoice();
            invoice.Items.Clear();
            return invoice;
        }

        private Invoice CreateInvalidInvoice_NoNumber()
        {
            var invoice = CreateValidInvoice();
            invoice.InvoiceNumber = null;
            return invoice;
        }

        // ---------------- ADD ----------------

        [Fact]
        public void AddInvoice_Valid_ShouldReturnSuccess()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>());

            var result = _service.AddInvoice(invoice);

            Assert.Equal(InvoiceAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(invoice), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddInvoice_Invalid_ShouldReturnInvalid()
        {
            var invoice = CreateInvalidInvoice_NoItems();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>());

            var result = _service.AddInvoice(invoice);

            Assert.Equal(InvoiceAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- EDIT ----------------

        [Fact]
        public void EditInvoice_NotFound_ShouldReturnNotFound()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns((Invoice)null);

            var result = _service.EditInvoice(invoice);

            Assert.Equal(InvoiceEditResult.NotFound, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditInvoice_Archived_ShouldReturnArchived()
        {
            var invoice = CreateValidInvoice();
            invoice.IsInvoiceArchived = true;

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.EditInvoice(invoice);

            Assert.Equal(InvoiceEditResult.InvoiceArchived, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditInvoice_Invalid_ShouldReturnInvalid()
        {
            var invoice = CreateInvalidInvoice_NoNumber();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.EditInvoice(invoice);

            Assert.Equal(InvoiceEditResult.InvalidData, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveInvoice_Valid_ShouldSuccess()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.ArchiveInvoice(invoice.Id);

            Assert.Equal(ArchiveInvoiceResult.Success, result);

            _repoMock.Verify(r => r.Update(invoice), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        // ---------------- FIND ----------------

        [Fact]
        public void FindInvoice_ShouldReturnEntity()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.FindInvoice(invoice.Id);

            Assert.NotNull(result);
            Assert.Equal(invoice.Id, result.Id);
        }

        [Fact]
        public void FindInvoice_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Invoice)null);

            var result = _service.FindInvoice(1);

            Assert.Null(result);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAll_ShouldReturnData()
        {
            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Invoice>
                {
                    CreateValidInvoice(),
                    CreateValidInvoice()
                });

            var result = _service.GetAllInvoices();

            Assert.Equal(2, result.Count);
        }
    }
}