// Path: console-online-store/Store.Tests/ProductServiceTests.cs
using System;
using System.Linq;

using StoreBLL.Services;

using Xunit;

namespace Store.Tests
{
    /// <summary>
    /// Unit tests for ProductService class.
    /// Tests CRUD operations, validation, and business logic for products.
    /// </summary>
    public class ProductServiceTests
    {
        /// <summary>
        /// Tests that GetAll returns all products from database.
        /// Verifies that product list is not null and contains items.
        /// </summary>
        [Fact]
        public void GetAll_ReturnsAllProducts()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act
                var products = service.GetAll();

                // Assert
                Assert.NotNull(products);
                Assert.NotEmpty(products);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that GetById returns correct product by ID.
        /// Verifies that returned product matches requested ID.
        /// </summary>
        [Fact]
        public void GetById_ReturnsCorrectProduct()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act
                var product = service.GetById(1);

                // Assert
                Assert.NotNull(product);
                Assert.Equal(1, product.Id);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Add creates new product with valid data.
        /// Verifies that product is created with correct properties and auto-generated ID.
        /// </summary>
        [Fact]
        public void Add_CreatesNewProduct()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act
                var newProduct = service.Add(
                    title: "Test Product",
                    category: "fruits",
                    manufacturer: "GreenFarm",
                    sku: "TEST-001",
                    description: "Test description",
                    price: 10.50m,
                    stock: 100);

                // Assert
                Assert.NotNull(newProduct);
                Assert.True(newProduct.Id > 0);
                Assert.Equal("Test Product", newProduct.Title);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Update modifies existing product.
        /// Verifies that product properties are updated correctly.
        /// </summary>
        [Fact]
        public void Update_ModifiesExistingProduct()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act
                var updated = service.Update(
                    id: 1,
                    title: "Updated Product",
                    category: "fruits",
                    manufacturer: "GreenFarm",
                    sku: "UPD-001",
                    description: "Updated description",
                    price: 15.75m,
                    stock: 150);

                // Assert
                Assert.NotNull(updated);
                Assert.Equal("Updated Product", updated.Title);
                Assert.Equal(15.75m, updated.Price);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Delete removes product from database.
        /// Verifies that product is deleted and cannot be retrieved afterwards.
        /// </summary>
        [Fact]
        public void Delete_RemovesProduct()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);
                var newProduct = service.Add("Delete Me", "fruits", "GreenFarm", "DEL-001", "To delete", 5.0m, 10);

                // Act
                var result = service.Delete(newProduct.Id);

                // Assert
                Assert.True(result);
                Assert.Null(service.GetById(newProduct.Id));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Add throws ArgumentOutOfRangeException for negative price.
        /// Verifies validation logic for product price.
        /// </summary>
        [Fact]
        public void Add_WithNegativePrice_ThrowsException()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    service.Add("Invalid", "fruits", "GreenFarm", "INV-001", "Invalid price", -10.0m, 10));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Add with negative stock throws ArgumentOutOfRangeException.
        /// Verifies validation logic for product stock quantity.
        /// </summary>
        [Fact]
        public void Add_WithNegativeStock_ThrowsException()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    service.Add("Invalid", "fruits", "GreenFarm", "INV-002", "Invalid stock", 10.0m, -5));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that newly created product has reserved quantity set to zero.
        /// Verifies proper initialization of inventory counters.
        /// </summary>
        [Fact]
        public void Add_SetsReservedQuantityToZero()
        {
            // Arrange
            var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
            try
            {
                var service = new ProductService(unitOfWork);

                // Act
                var product = service.Add("Test", "fruits", "GreenFarm", "T-001", "Desc", 10m, 50);

                // Assert
                Assert.Equal(0, product.Reserved);
                Assert.Equal(50, product.Available);
            }
            finally
            {
                cleanup();
            }
        }
    }
}
