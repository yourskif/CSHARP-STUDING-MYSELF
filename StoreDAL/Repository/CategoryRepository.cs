using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository for managing category entities in the database.
    /// Implements CRUD operations for product categories.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing category data.</param>
        public CategoryRepository(StoreDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a new category to the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Category entity to add.</param>
        public void Add(Category entity)
        {
            this.context.Categories.Add(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes an existing category from the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Category entity to delete.</param>
        public void Delete(Category entity)
        {
            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes a category by its identifier.
        /// If the category does not exist, no action is taken.
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
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>Collection of all categories.</returns>
        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories.ToList();
        }

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve (1-based).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Collection of categories for the specified page.</returns>
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Categories
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
        /// </summary>
        /// <param name="entity">Category entity with updated values.</param>
        public void Update(Category entity)
        {
            this.context.Categories.Update(entity);
            this.context.SaveChanges();
        }
    }
}
