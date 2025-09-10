using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreDbContext context;

        public CategoryRepository(StoreDbContext context)
        {
            this.context = context;
        }

        // Додати нову категорію
        public void Add(Category entity)
        {
            this.context.Categories.Add(entity);
            this.context.SaveChanges();
        }

        // Видалити категорію (по сутності)
        public void Delete(Category entity)
        {
            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
        }

        // Видалити категорію за ID
        public void DeleteById(int id)
        {
            var category = this.context.Categories.Find(id);
            if (category != null)
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
            }
        }

        // Отримати всі категорії
        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories.ToList();
        }

        // Отримати категорії з пагінацією
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Categories
                          .Skip((pageNumber - 1) * rowCount)
                          .Take(rowCount)
                          .ToList();
        }

        // Отримати категорію за ID
        public Category GetById(int id)
        {
            return this.context.Categories.Find(id);
        }

        // Оновити категорію
        public void Update(Category entity)
        {
            this.context.Categories.Update(entity);
            this.context.SaveChanges();
        }
    }
}
