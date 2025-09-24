// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Repository\ProductRepository.cs
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
    /// Implements generic CRUD + helpers with eager loading.
    /// </summary>
    public sealed class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext db;

        public ProductRepository(StoreDbContext db)
            : base()
        {
            this.db = db;
        }

        // ---------- With Includes (required by IProductRepository) ----------

        /// <summary>
        /// Returns all products with related Title and Manufacturer loaded.
        /// </summary>
        public IEnumerable<Product> GetAllWithIncludes()
        {
            return this.db.Products
                .AsNoTracking()
                .Include(p => p.Title)
                .Include(p => p.Manufacturer)
                .AsEnumerable();
        }

        /// <summary>
        /// Returns a product by id with related Title and Manufacturer loaded.
        /// </summary>
        public Product? GetByIdWithIncludes(int id)
        {
            return this.db.Products
                .Include(p => p.Title)
                .Include(p => p.Manufacturer)
                .FirstOrDefault(p => p.Id == id);
        }

        // ---------- Basic CRUD ----------
        public IEnumerable<Product> GetAll()
        {
            return this.db.Products
                .AsNoTracking()
                .AsEnumerable();
        }

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

        public Product? GetById(int id)
        {
            return this.db.Products.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Returns products by category id (through ProductTitle.CategoryId).
        /// </summary>
        public IEnumerable<Product> GetByCategoryId(int categoryId)
        {
            return this.db.Products
                .AsNoTracking()
                .Include(p => p.Title)
                .Where(p => p.Title != null && p.Title.CategoryId == categoryId)
                .AsEnumerable();
        }

        public void Add(Product entity)
        {
            this.db.Products.Add(entity);
        }

        public void Update(Product entity)
        {
            var entry = this.db.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.db.Products.Attach(entity);
                entry = this.db.Entry(entity);
            }

            entry.State = EntityState.Modified;
        }

        public void Delete(Product entity)
        {
            this.db.Products.Remove(entity);
        }

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
