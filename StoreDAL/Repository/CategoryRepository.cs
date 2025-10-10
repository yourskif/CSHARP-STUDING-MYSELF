// Path: console-online-store/StoreDAL/Repository/CategoryRepository.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository implementation for category entities.
    /// Uses Unit of Work pattern - does not call SaveChanges internally.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public CategoryRepository(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets all categories ordered by name.
        /// </summary>
        /// <returns>Collection of all categories.</returns>
        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Gets paginated categories ordered by name.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Paginated collection of categories.</returns>
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Gets a category by its identifier.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns>Category entity.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when category with specified id is not found.</exception>
        public Category GetById(int id)
        {
            return this.context.Categories.Find(id)
                ?? throw new KeyNotFoundException($"Category with id {id} not found.");
        }

        /// <summary>
        /// Adds a new category. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Category entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(Category entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Categories.Add(entity);
        }

        /// <summary>
        /// Deletes a category. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Category entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(Category entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Categories.Remove(entity);
        }

        /// <summary>
        /// Deletes a category by ID. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="id">Category identifier to delete.</param>
        public void DeleteById(int id)
        {
            var entity = this.context.Categories.Find(id);
            if (entity != null)
            {
                this.context.Categories.Remove(entity);
            }
        }

        /// <summary>
        /// Updates a category. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Category entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(Category entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Categories.Update(entity);
        }

        /// <summary>
        /// Searches categories by name (case-insensitive contains).
        /// Uses ordinal case-insensitive comparison for culture-independent matching.
        /// </summary>
        /// <param name="namePart">Partial name to search for.</param>
        /// <returns>Collection of matching categories ordered by name.</returns>
        public IEnumerable<Category> SearchByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
            {
                return this.GetAll();
            }

            var upperSearch = namePart.ToUpperInvariant();

            return this.context.Categories
                .AsNoTracking()
                .Where(c => c.Name != null && c.Name.ToUpper(CultureInfo.InvariantCulture).Contains(upperSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Gets total stock quantity across all products in a category.
        /// </summary>
        /// <param name="categoryId">Category identifier.</param>
        /// <returns>Total stock quantity.</returns>
        public int GetTotalStock(int categoryId)
        {
            return this.context.ProductTitles
                .Where(pt => pt.CategoryId == categoryId)
                .SelectMany(pt => pt.Products)
                .Sum(p => p.StockQuantity);
        }
    }
}
