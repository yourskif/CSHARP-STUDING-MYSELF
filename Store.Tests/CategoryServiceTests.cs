// Path: console-online-store/Store.Tests/CategoryServiceTests.cs
using System;
using System.Linq;
using StoreBLL.Services;
using StoreBLL.Models;
using Xunit;

namespace Store.Tests
{
    /// <summary>
    /// Unit tests for CategoryService class.
    /// Tests CRUD operations, search functionality, and business logic for categories.
    /// </summary>
    public class CategoryServiceTests
    {
        /// <summary>
        /// Tests that GetAll returns all categories from database.
        /// Verifies that category list is not null and contains items.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that GetById returns correct category by ID.
        /// Verifies that returned category matches requested ID.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Add creates new category with valid data.
        /// Verifies that category is created with correct name and auto-generated ID.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Update modifies existing category.
        /// Verifies that category name is updated correctly.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Delete removes category from database.
        /// Verifies that category is deleted and cannot be retrieved afterwards.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that FindByName returns categories matching search term.
        /// Verifies case-insensitive partial name matching.
        /// </summary>
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
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that FindByName returns empty list for non-matching search.
        /// Verifies behavior when no categories match the search term.
        /// </summary>
        [Fact]
        public void FindByName_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new CategoryService(ctx);

                // Act
                var results = service.FindByName("NonExistentCategory").ToList();

                // Assert
                Assert.Empty(results);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Update returns false for non-existent category.
        /// Verifies proper handling of invalid update operations.
        /// </summary>
        [Fact]
        public void Update_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new CategoryService(ctx);
                var model = new CategoryModel(99999, "Non Existent");

                // Act
                var result = service.Update(model);

                // Assert
                Assert.False(result);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Delete returns false for non-existent category.
        /// Verifies proper handling of invalid delete operations.
        /// </summary>
        [Fact]
        public void Delete_WithNonExistentId_ReturnsFalse()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new CategoryService(ctx);

                // Act
                var result = service.Delete(99999);

                // Assert
                Assert.False(result);
            }
            finally
            {
                cleanup();
            }
        }
    }
}
