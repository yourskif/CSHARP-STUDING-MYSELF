// Path: console-online-store/Store.Tests/ProductServiceTests.cs
using System;
using System.Linq;

using StoreBLL.Services;

using Xunit;

namespace Store.Tests;

public class ProductServiceTests
{
    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));

            // Act
            var products = service.GetAll();

            // Assert
            Assert.NotNull(products);
            Assert.NotEmpty(products);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void GetById_ReturnsCorrectProduct()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));

            // Act
            var product = service.GetById(1);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Add_CreatesNewProduct()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));

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
        finally { cleanup(); }
    }

    [Fact]
    public void Update_ModifiesExistingProduct()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));

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
        finally { cleanup(); }
    }

    [Fact]
    public void Delete_RemovesProduct()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));
            var newProduct = service.Add("Delete Me", "fruits", "GreenFarm", "DEL-001", "To delete", 5.0m, 10);

            // Act
            var result = service.Delete(newProduct.Id);

            // Assert
            Assert.True(result);
            Assert.Null(service.GetById(newProduct.Id));
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Add_WithNegativePrice_ThrowsException()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ProductService(new StoreDAL.Repository.ProductRepository(ctx));

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                service.Add("Invalid", "fruits", "GreenFarm", "INV-001", "Invalid price", -10.0m, 10));
        }
        finally { cleanup(); }
    }
}
