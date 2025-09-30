// Path: console-online-store/Store.Tests/CategoryServiceTests.cs
using System;
using System.Linq;
using StoreBLL.Services;
using StoreBLL.Models;
using Xunit;

namespace Store.Tests;

public class CategoryServiceTests
{
    [Fact]
    public void GetAll_ReturnsAllCategories()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);

            // Act
            var categories = service.GetAll().ToList();

            // Assert
            Assert.NotNull(categories);
            Assert.NotEmpty(categories);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void GetById_ReturnsCorrectCategory()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);

            // Act
            var category = service.GetById(1);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(1, category.Id);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Add_CreatesNewCategory()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);
            var model = new CategoryModel(0, "Test Category");

            // Act
            var result = service.Add(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test Category", result.Name);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Update_ModifiesExistingCategory()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);
            var model = new CategoryModel(1, "Updated Category");

            // Act
            var result = service.Update(model);

            // Assert
            Assert.True(result);
            var updated = service.GetById(1);
            Assert.Equal("Updated Category", updated?.Name);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Delete_RemovesCategory()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);
            var newCategory = service.Add(new CategoryModel(0, "To Delete"));

            // Act
            var result = service.Delete(newCategory.Id);

            // Assert
            Assert.True(result);
            Assert.Null(service.GetById(newCategory.Id));
        }
        finally { cleanup(); }
    }

    [Fact]
    public void FindByName_ReturnsMatchingCategories()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new CategoryService(ctx);
            service.Add(new CategoryModel(0, "Fruits Fresh"));
            service.Add(new CategoryModel(0, "Vegetables Fresh"));

            // Act
            var results = service.FindByName("Fresh").ToList();

            // Assert
            Assert.NotEmpty(results);
            Assert.All(results, c => Assert.Contains("Fresh", c.Name));
        }
        finally { cleanup(); }
    }
}
