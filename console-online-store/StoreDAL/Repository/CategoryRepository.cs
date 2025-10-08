// Path: console-online-store/StoreDAL/Repository/CategoryRepository.cs
using System.Collections.Generic;
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

        public CategoryRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToList();
        }

        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public Category GetById(int id)
        {
            return this.context.Categories.Find(id);
        }

        /// <summary>
        /// Adds a new category. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Add(Category entity)
        {
            this.context.Categories.Add(entity);
        }

        /// <summary>
        /// Deletes a category. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Delete(Category entity)
        {
            this.context.Categories.Remove(entity);
        }

        /// <summary>
        /// Deletes a category by ID. Changes are not persisted until SaveChanges is called.
        /// </summary>
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
        public void Update(Category entity)
        {
            this.context.Categories.Update(entity);
        }

        /// <summary>
        /// Searches categories by name (case-insensitive contains).
        /// </summary>
        public IEnumerable<Category> SearchByName(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
            {
                return this.GetAll();
            }

            var lower = namePart.ToLowerInvariant();
            return this.context.Categories
                .AsNoTracking()
                .Where(c => c.Name.ToLower().Contains(lower))
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Gets total stock quantity across all products in a category.
        /// </summary>
        public int GetTotalStock(int categoryId)
        {
            return this.context.ProductTitles
                .Where(pt => pt.CategoryId == categoryId)
                .SelectMany(pt => pt.Products)
                .Sum(p => p.StockQuantity);
        }
    }
}
