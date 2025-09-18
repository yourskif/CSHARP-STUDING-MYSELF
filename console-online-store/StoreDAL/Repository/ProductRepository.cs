using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class ProductRepository : AbstractRepository, IProductRepository
    {
        public ProductRepository(StoreDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get all products with navigation properties loaded.
        /// </summary>
        public IEnumerable<Product> GetAllWithIncludes()
        {
            return this.context.Products
                .Include(p => p.Title)
                    .ThenInclude(pt => pt.Category)
                .Include(p => p.Manufacturer)
                .ToList();
        }

        /// <summary>
        /// Get product by ID with navigation properties loaded.
        /// </summary>
        public Product? GetByIdWithIncludes(int id)
        {
            return this.context.Products
                .Include(p => p.Title)
                    .ThenInclude(pt => pt.Category)
                .Include(p => p.Manufacturer)
                .FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Get products filtered by category ID with navigation properties.
        /// </summary>
        public IEnumerable<Product> GetByCategoryId(int categoryId)
        {
            return this.context.Products
                .Include(p => p.Title)
                    .ThenInclude(pt => pt.Category)
                .Include(p => p.Manufacturer)
                .Where(p => p.Title.CategoryId == categoryId)
                .ToList();
        }

        // Compatibility methods for ProductService reflection calls
        public IEnumerable<Product> GetAll()
        {
            return GetAllWithIncludes();
        }

        public Product? GetById(int id)
        {
            return GetByIdWithIncludes(id);
        }

        public void Add(Product product)
        {
            this.context.Products.Add(product);
            this.context.SaveChanges();
        }

        public void Update(Product product)
        {
            this.context.Products.Update(product);
            this.context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = this.context.Products.Find(id);
            if (product != null)
            {
                this.context.Products.Remove(product);
                this.context.SaveChanges();
            }
        }

        public void Delete(Product product)
        {
            this.context.Products.Remove(product);
            this.context.SaveChanges();
        }
    }
}