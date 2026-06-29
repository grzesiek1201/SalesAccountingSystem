using AccountingSystem.Application.Interfaces;
using AccountingSystem.Application.Repositories;
using AccountingSystem.Application.Services;
using AccountingSystem.Application.Validation.Products;
using AccountingSystem.Domain.Entities;
using AccountingSystem.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AccountingSystem.Tests.ServicesTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        private readonly ProductValidator _validator;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _validator = new ProductValidator();

            _service = new ProductService(
                _repoMock.Object,
                _validator,
                _uowMock.Object,
                _loggerMock.Object
            );
        }

        private Product CreateValidProduct()
        {
            return new Product
            {
                Id = 1,
                Name = "Chocolate GOLD",
                Price = 100,
                CategoryId = 1,
                IsProductArchived = false,
                Category = CreateValidCategory()
            };
        }

        private Category CreateValidCategory()
        {
            return new Category
            {
                Id = 1,
                Name = "Sweets",
                Products = new List<Product>()
            };
        }

        // ---------------- ADD ----------------

        [Fact]
        public void AddProduct_Valid_ShouldReturnSuccess()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.AddProduct(product);

            Assert.Equal(ProductAddResult.Success, result.Result);

            _repoMock.Verify(r => r.Add(product), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void AddProduct_Invalid_ShouldReturnInvalidData()
        {
            var product = CreateValidProduct();
            product.Price = -10;

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.AddProduct(product);

            Assert.Equal(ProductAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void AddProduct_DuplicateName_ShouldReturnInvalidData()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>
                {
                    new Product
                    {
                        Id = 999,
                        Name = "Chocolate GOLD",
                        Category = new Category
                        {
                            Id = 1,
                            Name = "Sweets",
                            Products = new List<Product>()
                        },
                        Price = 100,
                        CategoryId = 1
                    }
                });

            var result = _service.AddProduct(product);

            Assert.Equal(ProductAddResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- EDIT ----------------

        [Fact]
        public void EditProduct_Valid_ShouldReturnSuccess()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetById(product.Id))
                .Returns(product);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.EditProduct(product);

            Assert.Equal(ProductEditResult.Success, result.Result);

            _repoMock.Verify(r => r.Update(product), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void EditProduct_NotFound_ShouldReturnNotFound()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null);

            var result = _service.EditProduct(product);

            Assert.Equal(ProductEditResult.NotFound, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditProduct_Archived_ShouldReturnProductArchived()
        {
            var product = CreateValidProduct();
            product.IsProductArchived = true;

            _repoMock.Setup(r => r.GetById(product.Id))
                .Returns(product);

            var result = _service.EditProduct(product);

            Assert.Equal(ProductEditResult.ProductArchived, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public void EditProduct_Invalid_ShouldReturnInvalidData()
        {
            var product = CreateValidProduct();
            product.Price = -10;

            _repoMock.Setup(r => r.GetById(product.Id))
                .Returns(product);

            _repoMock.Setup(r => r.GetAll())
                .Returns(new List<Product>());

            var result = _service.EditProduct(product);

            Assert.Equal(ProductEditResult.InvalidData, result.Result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- ARCHIVE ----------------

        [Fact]
        public void ArchiveProduct_Existing_ShouldReturnSuccess()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetById(product.Id))
                .Returns(product);

            var result = _service.ArchiveProduct(product.Id);

            Assert.Equal(ProductArchiveResult.Success, result);

            _repoMock.Verify(r => r.Update(product), Times.Once);
            _uowMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void ArchiveProduct_NotFound_ShouldReturnNotFound()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null);

            var result = _service.ArchiveProduct(1);

            Assert.Equal(ProductArchiveResult.NotFound, result);

            _repoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
            _uowMock.Verify(u => u.Save(), Times.Never);
        }

        // ---------------- FIND ----------------

        [Fact]
        public void FindProduct_Existing_ShouldReturnProduct()
        {
            var product = CreateValidProduct();

            _repoMock.Setup(r => r.GetById(product.Id))
                .Returns(product);

            var result = _service.FindProduct(product.Id);

            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
        }

        [Fact]
        public void FindProduct_NotExisting_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((Product)null);

            var result = _service.FindProduct(1);

            Assert.Null(result);
        }

        // ---------------- GET ALL ----------------

        [Fact]
        public void GetAllProducts_ShouldReturnAllProducts()
        {
            var products = new List<Product>
            {
                CreateValidProduct(),
                CreateValidProduct()
            };

            _repoMock.Setup(r => r.GetAll())
                .Returns(products);

            var result = _service.GetAllProducts();

            Assert.Equal(2, result.Count);
        }
    }
}