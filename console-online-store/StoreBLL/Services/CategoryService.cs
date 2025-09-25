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
    /// </summary>
    public sealed class CategoryService
    {
        private readonly StoreDbContext context;

        public CategoryService(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Returns all categories.
        /// </summary>
        public IEnumerable<CategoryModel> GetAll()
        {
            return this.context.Categories
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Returns a category by id or null if not found.
        /// </summary>
        public CategoryModel? GetById(int id)
        {
            return this.context.Categories
                .Where(c => c.Id == id)
                .Select(MapToModel)
                .FirstOrDefault();
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        public CategoryModel Add(CategoryModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = new Category
            {
                // Persist non-null name to avoid nullability warnings downstream.
                Name = model.Name ?? string.Empty,
            };

            this.context.Categories.Add(entity);
            this.context.SaveChanges();

            return MapToModel(entity);
        }

        /// <summary>
        /// Updates an existing category. Returns true if updated, false if not found.
        /// </summary>
        public bool Update(CategoryModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var entity = this.context.Categories.FirstOrDefault(c => c.Id == model.Id);
            if (entity is null)
            {
                return false;
            }

            entity.Name = model.Name ?? string.Empty;

            this.context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes a category by id. Returns true if deleted, false if not found.
        /// </summary>
        public bool Delete(int id)
        {
            var entity = this.context.Categories.Find(id);
            if (entity is null)
            {
                return false;
            }

            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Finds categories by name (case sensitivity depends on provider/config).
        /// </summary>
        public IEnumerable<CategoryModel> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Array.Empty<CategoryModel>();
            }

            return this.context.Categories
                .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{name}%"))
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Maps an entity to a model. Guarantees non-null name.
        /// </summary>
        private static CategoryModel MapToModel(Category e) =>
            new CategoryModel(e.Id, e.Name ?? string.Empty);
    }
}
