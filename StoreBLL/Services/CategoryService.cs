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

        public IEnumerable<CategoryModel> GetAll()
        {
            return this.context.Categories
                .Select(MapToModel)
                .ToList();
        }

        public CategoryModel? GetById(int id)
        {
            return this.context.Categories
                .Where(c => c.Id == id)
                .Select(MapToModel)
                .FirstOrDefault();
        }

        public CategoryModel Add(CategoryModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            var entity = new Category
            {
                // Якщо Id автогенерується БД, не заповнюємо його
                Name = model.Name ?? string.Empty,
            };

            this.context.Categories.Add(entity);
            this.context.SaveChanges(); // отримаємо фактичний Id

            return MapToModel(entity);
        }

        public bool Update(CategoryModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            var entity = this.context.Categories.FirstOrDefault(c => c.Id == model.Id);
            if (entity is null)
            {
                return false;
            }

            entity.Name = model.Name ?? string.Empty;

            this.context.SaveChanges();
            return true;
        }

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

        public IEnumerable<CategoryModel> FindByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Array.Empty<CategoryModel>();
            }

            // EF-пошук по підрядку
            return this.context.Categories
                .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{name}%"))
                .Select(MapToModel)
                .ToList();
        }

        private static CategoryModel MapToModel(Category e) => new CategoryModel(e.Id, e.Name);
    }
}
