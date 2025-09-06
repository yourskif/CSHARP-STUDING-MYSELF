// StoreDAL/Repository/CategoryRepository.cs
using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// EF Core implementation of ICategoryRepository.
    /// Keeps logic straightforward and readable; comments are in English only.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext context;

        public CategoryRepository(StoreDbContext context)
        {
            this.context = context;
        }

        /// <summary>Adds a new category and saves changes.</summary>
        public void Add(Category entity)
        {
            this.context.Categories.Add(entity);
            this.context.SaveChanges();
        }

        /// <summary>Deletes the specified category and saves changes.</summary>
        public void Delete(Category entity)
        {
            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
        }

        /// <summary>Deletes a category by its id and saves changes (no-op if not found).</summary>
        public void DeleteById(int id)
        {
            var category = this.context.Categories.Find(id);
            if (category != null)
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
            }
        }

        /// <summary>Returns all categories.</summary>
        public IEnumerable<Category> GetAll()
        {
            // If you prefer no-tracking reads, use: return this.context.Categories.AsNoTracking().ToList();
            return this.context.Categories.ToList();
        }

        /// <summary>Returns a page of categories.</summary>
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (rowCount < 1) rowCount = 10;

            return this.context.Categories
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Returns a category by id or null if not found.
        /// </summary>
        public Category GetById(int id)
        {
            return this.context.Categories.Find(id);
        }

        /// <summary>Updates the specified category and saves changes.</summary>
        public void Update(Category entity)
        {
            this.context.Categories.Update(entity);
            this.context.SaveChanges();
        }
    }
}
