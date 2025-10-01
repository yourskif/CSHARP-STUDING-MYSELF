// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Repository\ProductRepository.cs
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
    /// EF Core-backed repository for products.
    /// Implements generic CRUD operations with eager loading support for related entities (Title, Manufacturer).
    /// Provides both basic operations and extended queries with navigation properties.
    /// </summary>
    public sealed class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="db">EF Core database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
        public ProductRepository(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // ---------- With Includes (required by IProductRepository) ----------

        /// <summary>
        /// Returns all products with related Title and Manufacturer entities eagerly loaded.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of products with navigation properties populated.</returns>
        public IEnumerable<Product> GetAllWithIncludes()
        {
            return this.db.Products
                .AsNoTracking()
                .Include(p => p.Title)
                    .ThenInclude(t => t!.Category)
                .Include(p => p.Manufacturer)
                .OrderBy(p => p.Id)
                .AsEnumerable();
        }

        /// <summary>
        /// Returns a single product by id with related Title and Manufacturer entities eagerly loaded.
        /// Returns null if product is not found.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>Product entity with navigation properties, or null if not found.</returns>
        public Product? GetByIdWithIncludes(int id)
        {
            return this.db.Products
                .Include(p => p.Title)
                    .ThenInclude(t => t!.Category)
                .Include(p => p.Manufacturer)
                .FirstOrDefault(p => p.Id == id);
        }

        // ---------- Basic CRUD ----------

        /// <summary>
        /// Returns all products without navigation properties.
        /// Uses AsNoTracking for read-only operations.
        /// </summary>
        /// <returns>Collection of all products.</returns>
        public IEnumerable<Product> GetAll()
        {
            return this.db.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .AsEnumerable();
        }

        /// <summary>
        /// Returns a paginated subset of products.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based index).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Paginated collection of products.</returns>
        public IEnumerable<Product> GetAll(int pageNumber, int rowCount)
        {
            var skip = pageNumber <= 1 ? 0 : (pageNumber - 1) * rowCount;

            return this.db.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(rowCount)
                .AsEnumerable();
        }

        /// <summary>
        /// Returns a single product by id without navigation properties.
        /// Returns null if product is not found.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>Product entity or null if not found.</returns>
        public Product? GetById(int id)
        {
            return this.db.Products.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Returns all products belonging to a specific category.
        /// Filters through ProductTitle.CategoryId relationship.
        /// </summary>
        /// <param name="categoryId">Category identifier to filter by.</param>
        /// <returns>Collection of products in the specified category.</returns>
        public IEnumerable<Product> GetByCategoryId(int categoryId)
        {
            return this.db.Products
                .AsNoTracking()
                .Include(p => p.Title)
                .Where(p => p.Title != null && p.Title.CategoryId == categoryId)
                .OrderBy(p => p.Id)
                .AsEnumerable();
        }

        /// <summary>
        /// Adds a new product to the database context.
        /// Note: Changes are not persisted until SaveChanges is called on the context.
        /// </summary>
        /// <param name="entity">Product entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.db.Products.Add(entity);
        }

        /// <summary>
        /// Marks an existing product entity as modified in the database context.
        /// Handles both attached and detached entities.
        /// Note: Changes are not persisted until SaveChanges is called on the context.
        /// </summary>
        /// <param name="entity">Product entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entry = this.db.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.db.Products.Attach(entity);
                entry = this.db.Entry(entity);
            }

            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// Removes a product from the database context.
        /// Note: Changes are not persisted until SaveChanges is called on the context.
        /// </summary>
        /// <param name="entity">Product entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(Product entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.db.Products.Remove(entity);
        }

        /// <summary>
        /// Removes a product by id from the database context.
        /// Idempotent operation - no error if product doesn't exist.
        /// Note: Changes are not persisted until SaveChanges is called on the context.
        /// </summary>
        /// <param name="id">Product identifier to delete.</param>
        public void DeleteById(int id)
        {
            var entity = this.db.Products.FirstOrDefault(p => p.Id == id);
            if (entity != null)
            {
                this.db.Products.Remove(entity);
            }
        }
    }
}
