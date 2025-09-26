// Full path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\CategoryService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoreDAL.Data;
    using StoreDAL.Entities;

    /// <summary>
    /// Service for working with categories (EF Core).
    /// Provides basic CRUD operations and search helpers.
    /// </summary>
    public sealed class CategoryService(StoreDbContext context)
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="context">EF Core database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        private readonly StoreDbContext context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Returns all categories.
        /// </summary>
        /// <returns>Enumeration of categories as <see cref="StoreBLL.Models.CategoryModel"/>.</returns>
        public IEnumerable<StoreBLL.Models.CategoryModel> GetAll()
        {
            return this.context.Categories
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Returns a category by identifier or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns>
        /// <see cref="StoreBLL.Models.CategoryModel"/> instance when found; otherwise, <see langword="null"/>.
        /// </returns>
        public StoreBLL.Models.CategoryModel? GetById(int id)
        {
            return this.context.Categories
                .Where(c => c.Id == id)
                .Select(MapToModel)
                .FirstOrDefault();
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="model">Category model to add.</param>
        /// <returns>Created category model.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        public StoreBLL.Models.CategoryModel Add(StoreBLL.Models.CategoryModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = new Category
            {
                Name = model.Name ?? string.Empty,
            };

            this.context.Categories.Add(entity);
            this.context.SaveChanges();

            return MapToModel(entity);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="model">Category model with the updated data.</param>
        /// <returns><see langword="true"/> if the category was updated; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        public bool Update(StoreBLL.Models.CategoryModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

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
        /// Deletes a category by identifier.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns><see langword="true"/> if the category was deleted; otherwise, <see langword="false"/>.</returns>
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
        /// Finds categories by name (case sensitivity depends on the database provider and collation).
        /// </summary>
        /// <param name="name">A substring to search for within the category name.</param>
        /// <returns>Enumeration of categories that match the specified <paramref name="name"/>.</returns>
        public IEnumerable<StoreBLL.Models.CategoryModel> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Array.Empty<StoreBLL.Models.CategoryModel>();
            }

            return this.context.Categories
                .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{name}%"))
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Maps an EF entity to a BLL model and guarantees a non-null name value.
        /// </summary>
        /// <param name="e">Category entity.</param>
        /// <returns>Mapped <see cref="StoreBLL.Models.CategoryModel"/> instance.</returns>
        private static StoreBLL.Models.CategoryModel MapToModel(Category e) =>
            new StoreBLL.Models.CategoryModel(e.Id, e.Name ?? string.Empty);
    }
}
