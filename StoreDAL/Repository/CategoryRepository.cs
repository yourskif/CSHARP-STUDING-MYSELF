using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository for managing category entities in the database.
    /// Implements CRUD operations for product categories with immediate persistence.
    /// Each method automatically commits changes to the database.
    /// </summary>
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing category data.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public CategoryRepository(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new category to the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Category entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.context.Categories.Add(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes an existing category from the database and saves changes immediately.
        /// Note: Deletion may fail if category is referenced by products due to foreign key constraints.
        /// </summary>
        /// <param name="entity">Category entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes a category by its identifier.
        /// If the category does not exist, no action is taken (idempotent operation).
        /// Note: Deletion may fail if category is referenced by products due to foreign key constraints.
        /// </summary>
        /// <param name="id">Unique identifier of the category to delete.</param>
        public void DeleteById(int id)
        {
            var category = this.context.Categories.Find(id);
            if (category != null)
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves all categories from the database ordered by Id.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all categories.</returns>
        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToList();
        }

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve (1-based index, minimum value is 1).</param>
        /// <param name="rowCount">Number of rows per page (must be positive).</param>
        /// <returns>Collection of categories for the specified page.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when pageNumber is less than 1 or rowCount is less than 1.</exception>
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");
            }

            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount), "Row count must be at least 1.");
            }

            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the category.</param>
        /// <returns>Category entity if found; otherwise, null.</returns>
        public Category GetById(int id)
        {
            return this.context.Categories.Find(id);
        }

        /// <summary>
        /// Updates an existing category in the database and saves changes immediately.
        /// Marks the entity as modified in the context and persists changes.
        /// </summary>
        /// <param name="entity">Category entity with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.context.Categories.Update(entity);
            this.context.SaveChanges();
        }
    }
}
