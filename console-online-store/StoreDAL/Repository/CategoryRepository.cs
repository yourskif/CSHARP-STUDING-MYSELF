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

        // Р”РѕРґР°С‚Рё РЅРѕРІСѓ РєР°С‚РµРіРѕСЂС–СЋ
        public void Add(Category entity)
        {
            this.context.Categories.Add(entity);
            this.context.SaveChanges();
        }

        // Р’РёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ (РїРѕ СЃСѓС‚РЅРѕСЃС‚С–)
        public void Delete(Category entity)
        {
            this.context.Categories.Remove(entity);
            this.context.SaveChanges();
        }

        // Р’РёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ Р·Р° ID
        public void DeleteById(int id)
        {
            var category = this.context.Categories.Find(id);
            if (category != null)
            {
                this.context.Categories.Remove(category);
                this.context.SaveChanges();
            }
        }

        // РћС‚СЂРёРјР°С‚Рё РІСЃС– РєР°С‚РµРіРѕСЂС–С—
        public IEnumerable<Category> GetAll()
        {
            return this.context.Categories.ToList();
        }

        // РћС‚СЂРёРјР°С‚Рё РєР°С‚РµРіРѕСЂС–С— Р· РїР°РіС–РЅР°С†С–С”СЋ
        public IEnumerable<Category> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Categories
                          .Skip((pageNumber - 1) * rowCount)
                          .Take(rowCount)
                          .ToList();
        }

        // РћС‚СЂРёРјР°С‚Рё РєР°С‚РµРіРѕСЂС–СЋ Р·Р° ID
        public Category GetById(int id)
        {
            return this.context.Categories.Find(id);
        }

        // РћРЅРѕРІРёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ
        public void Update(Category entity)
        {
            this.context.Categories.Update(entity);
            this.context.SaveChanges();
        }
    }
}
