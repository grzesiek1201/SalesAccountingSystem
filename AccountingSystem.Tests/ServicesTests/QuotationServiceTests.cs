using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Customers;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Application.Validation.Quotations;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Castle.Core.Resource;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class QuotationServiceTests
    {
        private readonly Mock<IQuotationRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly QuotationValidator _validator;

        private readonly QuotationService _service;

        public QuotationServiceTests()
        {
            _repoMock = new Mock<IQuotationRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _validator = new QuotationValidator();

            _service = new QuotationService(
                _repoMock.Object,
                _validator,
                _uowMock.Object
            );
        }

        private Quotation CreateValidQuotation()
        {
            return new Quotation
            {
                Id = 1,
                QuotationNumber = "Q-2026-001",
                Status = QuotationStatus.Draft,
                DateCreated = new DateTime(2026, 1, 1, 10, 0, 0),

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

                IsQuotationArchived = false,

                Items = new List<QuotationItem>
                {
                    new QuotationItem
                    {
                        Id = 1,
                        ProductId = 1,
                        Product = new Product
                        {
                            Id = 1
                        },
                        Position = 1,
                        Quantity = 2,
                        BaseUnitPrice = 100m,
                        DiscountPercent = 0
                    }
                }
            };
        }

        private Quotation CreateInvalidQuotation_NoNumber()
        {
            var quotation = CreateValidQuotation();
            quotation.QuotationNumber = null;
            return quotation;
        }

        private Quotation CreateInvalidQuotation_NoItems()
        {
            var quotation = CreateValidQuotation();
            quotation.Items.Clear();
            return quotation;
        }

        private Quotation CreateInvalidQuotation_Archived()
        {
            var quotation = CreateValidQuotation();
            quotation.IsQuotationArchived = true;
            return quotation;
        }

        // ---------------- ADD ----------------

        [Fact]
        public void AddQuotation_Valid_ShouldReturnSuccess()
        {
            var quotation = CreateValidQuotation();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>());

            var result = _service.AddQuotation(quotation);

            Assert.Equal(QuotationAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Quotation>()), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddQuotation_Invalid_ShouldReturnInvalidData()
        {
            var quotation = CreateValidQuotation();

            quotation.Items.Clear();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Quotation>());

            var result = _service.AddQuotation(quotation);

            Assert.Equal(QuotationAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Quotation>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- EDIT ----------------

        [Fact]
        public void EditQuotation_NotFound_ShouldReturnNotFound()
        {
            var quotation = CreateValidQuotation();

            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Quotation)null);

            var result = _service.EditQuotation(quotation);

            Assert.Equal(QuotationEditResult.NotFound, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Quotation>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditQuotation_Archived_ShouldReturnArchived()
        {
            var quotation = CreateValidQuotation();
            quotation.IsQuotationArchived = true;

            _repoMock.Setup(r => r.GetById(quotation.Id))
                .Returns(quotation);

            var result = _service.EditQuotation(quotation);

            Assert.Equal(QuotationEditResult.QuotationArchived, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Quotation>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditQuotation_Invalid_ShouldReturnInvalidData()
        {
            var quotation = CreateValidQuotation();
            quotation.QuotationNumber = null;

            _repoMock.Setup(r => r.GetById(quotation.Id))
                .Returns(quotation);

            var result = _service.EditQuotation(quotation);

            Assert.Equal(QuotationEditResult.InvalidData, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Quotation>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }


        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveQuotation_ValidQuotation_ShouldSuccess()
        {
            var quotation = CreateValidQuotation();

            _repoMock.Setup(r => r.GetById(quotation.Id))
                .Returns(quotation);

            var result = _service.ArchiveQuotation(quotation.Id);

            Assert.Equal(ArchiveQuotationResult.Success, result);

            _repoMock.Verify(r => r.Update(quotation), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);

        }

        // ---------------- FIND ----------------

        [Fact]
        public void FindQuotation_ExistingId_ShouldReturnQuotation()
        {
            var quotation = CreateValidQuotation();

            _repoMock.Setup(r => r.GetById(quotation.Id))
                .Returns(quotation);

            var result = _service.FindQuotation(quotation.Id);

            Assert.NotNull(result);
            Assert.Equal(quotation.Id, result.Id);
        }

        [Fact]
        public void FindQuotation_NonExistingId_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Quotation)null);

            var result = _service.FindQuotation(1);

            Assert.Null(result);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAllQuotations_ShouldReturnQuotationsFromRepository()
        {
            var quotations = new List<Quotation>
            {
            CreateValidQuotation(),
            CreateValidQuotation()
            };

            _repoMock.Setup(r => r.GetAll())
                .Returns(quotations);

            var result = _service.GetAllQuotations();

            Assert.Equal(2, result.Count);
        }
    }
}
