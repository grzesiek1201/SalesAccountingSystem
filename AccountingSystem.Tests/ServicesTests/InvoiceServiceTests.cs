using AccountingSystem.Application.DTOs.Customers;
using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Mappers;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Invoices;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace AccountingSystem.Tests.ServicesTests
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<InvoiceService>> _loggerMock;
        private readonly Mock<INumberSequenceService> _seqMock;
        private readonly Mock<OrderToInvoiceMapper> _mapperMock;
        private readonly Mock<ICustomerRepository> _customerRepo;
        private readonly Mock<IProductRepository> _productRepo;

        private readonly InvoiceValidator _validator;
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _repoMock = new Mock<IInvoiceRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<InvoiceService>>();
            _seqMock = new Mock<INumberSequenceService>();
            _mapperMock = new Mock<OrderToInvoiceMapper>();
            _customerRepo = new Mock<ICustomerRepository>();
            _productRepo = new Mock<IProductRepository>();

            _seqMock
                .Setup(x => x.GetNext(It.IsAny<DocumentType>()))
                .Returns("I-2026-0001");

            _customerRepo
                .Setup(x => x.GetById(1))
                .Returns(new Customer
                 {
                    Id = 1,
                    Name = "Test"
                 });

            _productRepo
                .Setup(x => x.GetByIds(It.IsAny<List<int>>()))
                .Returns(new List<Product>
                {
                new Product
                {
                    Id = 1,
                    Name = "Test",
                    Price = 100m
                }
                });

            _validator = new InvoiceValidator();

            _service = new InvoiceService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object,
                _seqMock.Object,
                _mapperMock.Object,
                _customerRepo.Object,
                _productRepo.Object
            );
        }

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

                CustomerId = 1,
                Customer = new Customer { Id = 1 },

                TotalAmount = 200m,
                Payments = new List<Payment>(),

                Items = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {

                        ProductId = 1,
                        Product = new Product { Id = 1 },

                        Quantity = 2,
                        BaseUnitPrice = 100m,
                        DiscountPercent = 0,
                        Position = 1
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
        public void AddInvoice_Invalid_ShouldReturnInvalidData()
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

            Assert.Equal(InvoiceEditResult.NotFound, result.Result);
        }

        [Fact]
        public void EditInvoice_Archived_ShouldReturnInvoiceArchived()
        {
            var invoice = CreateValidInvoice();
            invoice.IsInvoiceArchived = true;

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.EditInvoice(invoice);

            Assert.Equal(InvoiceEditResult.InvoiceArchived, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditInvoice_Invalid_ShouldReturnInvalidData()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns((Invoice)null);

            var result = _service.EditInvoice(invoice);

            Assert.Equal(InvoiceEditResult.NotFound, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveInvoice_Existing_ShouldReturnSuccess()
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
        public void FindInvoice_Existing_ShouldReturnInvoice()
        {
            var invoice = CreateValidInvoice();

            _repoMock.Setup(r => r.GetById(invoice.Id))
                .Returns(invoice);

            var result = _service.FindInvoice(invoice.Id);

            Assert.NotNull(result);
            Assert.Equal(invoice.Id, result.Id);
        }

        [Fact]
        public void FindInvoice_NotExisting_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Invoice)null);

            var result = _service.FindInvoice(1);

            Assert.Null(result);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAllInvoices_ShouldReturnAllInvoices()
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