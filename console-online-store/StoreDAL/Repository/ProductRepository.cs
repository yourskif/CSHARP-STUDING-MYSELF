using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreDbContext context;

        public ProductRepository(StoreDbContext context) => this.context = context;

        // === Реалізації, яких вимагає IProductRepository ===

        // У вашій моделі немає навігаційних властивостей, тому "WithIncludes"
        // повертає базову вибірку без Include.
        public IEnumerable<Product> GetAllWithIncludes() => this.GetAll();

        public IEnumerable<Product> GetByCategoryId(int categoryId)
        {
            // Фільтрація через join на ProductTitles -> CategoryId
            // ВАЖЛИВО: використовуємо p.TitleId (а не p.ProductTitleId)
            var query =
                from p in this.context.Products.AsNoTracking()
                join t in this.context.ProductTitles.AsNoTracking()
                    on p.TitleId equals t.Id
                where t.CategoryId == categoryId
                select p;

            return query.ToList();
        }

        public Product? GetByIdWithIncludes(int id) => this.GetById(id);

        // === Додаткові зручні методи (не з інтерфейсу) ===

        public IEnumerable<Product> GetAll() =>
            this.context.Products.AsNoTracking().ToList();

        public Product? GetById(int id) =>
            this.context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
    }
}
