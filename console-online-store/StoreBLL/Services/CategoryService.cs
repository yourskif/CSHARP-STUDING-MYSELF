// Full path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\CategoryService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoreBLL.Models;

    using StoreDAL.Data;
    using StoreDAL.Entities;

    /// <summary>
    /// Service for working with categories (EF Core).
    /// Provides business logic layer for category management including CRUD operations,
    /// search functionality, and validation. Ensures data consistency and integrity
    /// through proper validation and foreign key constraint handling.
    /// </summary>
    public sealed class CategoryService
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="context">EF Core database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        public CategoryService(StoreDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.context = context;
        }

        /// <summary>
        /// Returns all categories ordered by identifier.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all categories as <see cref="CategoryModel"/> instances.</returns>
        public IEnumerable<CategoryModel> GetAll()
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new CategoryModel(c.Id, c.Name ?? string.Empty))
                .ToList();
        }

        /// <summary>
        /// Returns a category by its unique identifier.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns>
        /// <see cref="CategoryModel"/> instance when found; otherwise, <see langword="null"/>.
        /// </returns>
        public CategoryModel? GetById(int id)
        {
            var entity = this.context.Categories
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            return entity == null ? null : new CategoryModel(entity.Id, entity.Name ?? string.Empty);
        }

        /// <summary>
        /// Adds a new category to the database.
        /// Validates that the category name is not empty and does not already exist.
        /// </summary>
        /// <param name="model">Category model to add.</param>
        /// <returns>Created category model with assigned identifier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when category name is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a category with the same name already exists.</exception>
        public CategoryModel Add(CategoryModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(model));
            }

            // Check for duplicate name (case-insensitive)
            if (this.context.Categories.Any(c => c.Name != null && c.Name.ToLower() == model.Name.ToLower()))
            {
                throw new InvalidOperationException($"Category with name '{model.Name}' already exists.");
            }

            var entity = new Category
            {
                Name = model.Name,
            };

            this.context.Categories.Add(entity);
            this.context.SaveChanges();

            return new CategoryModel(entity.Id, entity.Name ?? string.Empty);
        }

        /// <summary>
        /// Updates an existing category in the database.
        /// Validates that the new name is not empty and does not conflict with existing categories.
        /// </summary>
        /// <param name="model">Category model with the updated data.</param>
        /// <returns><see langword="true"/> if the category was updated; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when category name is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when another category with the same name already exists.</exception>
        public bool Update(CategoryModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = this.context.Categories.FirstOrDefault(c => c.Id == model.Id);
            if (entity is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(model));
            }

            // Check for duplicate name (case-insensitive, excluding current entity)
            if (this.context.Categories.Any(c => c.Id != model.Id && c.Name != null && c.Name.ToLower() == model.Name.ToLower()))
            {
                throw new InvalidOperationException($"Category with name '{model.Name}' already exists.");
            }

            entity.Name = model.Name;
            this.context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes a category by its identifier.
        /// Ensures referential integrity by preventing deletion of categories referenced by products.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns><see langword="true"/> if the category was deleted; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when category has products referencing it.</exception>
        public bool Delete(int id)
        {
            var entity = this.context.Categories.Find(id);
            if (entity is null)
            {
                return false;
            }

            // Check if any product titles reference this category
            var productTitlesCount = this.context.ProductTitles.Count(pt => pt.CategoryId == id);

            if (productTitlesCount > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot delete category '{entity.Name}' (ID: {id}) because it has {productTitlesCount} product title(s) referencing it. Remove or reassign product titles first.");
            }

            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Finds categories by name using case-insensitive partial matching.
        /// Returns empty collection if search term is null or whitespace.
        /// </summary>
        /// <param name="name">A substring to search for within the category name.</param>
        /// <returns>Collection of categories that match the specified <paramref name="name"/>.</returns>
        public IEnumerable<CategoryModel> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Array.Empty<CategoryModel>();
            }

            return this.context.Categories
                .AsNoTracking()
                .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{name}%"))
                .OrderBy(c => c.Id)
                .Select(c => new CategoryModel(c.Id, c.Name ?? string.Empty))
                .ToList();
        }
    }
}
