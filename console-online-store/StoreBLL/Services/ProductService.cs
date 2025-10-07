namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoreBLL.Models;

    using StoreDAL.Data;
    using StoreDAL.Entities;
    using StoreDAL.Interfaces;
    using StoreDAL.Repository;

    /// <summary>
    /// Product business logic with EF Core context for write operations.
    /// </summary>
    public sealed class ProductService
    {
        private readonly IProductRepository repository;
        private readonly StoreDbContext context;

        public ProductService(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

            // Extract context from repository using reflection
            var contextField = repository.GetType().GetField("db", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (contextField == null)
            {
                contextField = repository.GetType().GetField("context", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }

            this.context = contextField?.GetValue(repository) as StoreDbContext
                ?? throw new InvalidOperationException("Cannot access DbContext from repository");
        }

        public ProductService()
            : this(new ProductRepository(StoreDbFactory.Create()))
        {
        }

        // ===== Public API =====
        public List<ProductModel> GetAll()
        {
            var list = this.repository.GetAllWithIncludes() ?? Array.Empty<Product>();
            return list.Select(MapToModel).ToList();
        }

        public ProductModel? GetById(int id)
        {
            var entity = this.repository.GetByIdWithIncludes(id);
            return entity is null ? null : MapToModel(entity);
        }

        public ProductModel Add(
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
        {
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price));
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock));
            }

            // Find or create ProductTitle
            var productTitle = this.context.ProductTitles
                .FirstOrDefault(pt => pt.Title == title);

            if (productTitle == null)
            {
                var cat = this.context.Categories.FirstOrDefault(c => c.Name == category);
                if (cat == null)
                {
                    cat = new Category { Name = category };
                    this.context.Categories.Add(cat);
                    this.context.SaveChanges();
                }

                productTitle = new ProductTitle
                {
                    Title = title,
                    CategoryId = cat.Id,
                };
                this.context.ProductTitles.Add(productTitle);
                this.context.SaveChanges();
            }

            // Find or create Manufacturer
            var manu = this.context.Manufacturers.FirstOrDefault(m => m.Name == manufacturer);
            if (manu == null)
            {
                manu = new Manufacturer { Name = manufacturer };
                this.context.Manufacturers.Add(manu);
                this.context.SaveChanges();
            }

            var p = new Product
            {
                ProductTitleId = productTitle.Id,
                ManufacturerId = manu.Id,
                Description = description ?? string.Empty,
                UnitPrice = price,
                StockQuantity = stock,
                ReservedQuantity = 0,
            };

            this.context.Products.Add(p); // ⚠️ Використовуємо context замість repository
            this.context.SaveChanges();

            return MapToModel(p);
        }

        public ProductModel? Update(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
        {
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price));
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock));
            }

            var p = this.context.Products
                .Include(p => p.Title)
                .Include(p => p.Manufacturer)
                .FirstOrDefault(p => p.Id == id);

            if (p is null)
            {
                return null;
            }

            p.Title ??= new ProductTitle();
            p.Title.Title = string.IsNullOrWhiteSpace(title) ? p.Title.Title ?? string.Empty : title.Trim();
            p.UnitPrice = price;
            p.StockQuantity = stock;
            p.Description = description ?? string.Empty;

            this.context.SaveChanges();

            return MapToModel(p);
        }

        public bool Delete(int id)
        {
            var p = this.context.Products.FirstOrDefault(p => p.Id == id);
            if (p is null)
            {
                return false;
            }

            this.context.Products.Remove(p); // ⚠️ Використовуємо context замість repository
            this.context.SaveChanges();
            return true;
        }

        // ===== Mapping =====
        private static ProductModel MapToModel(Product p)
        {
            var titleText = p.Title?.Title ?? $"Product {p.Id}";
            var categoryName = p.Title?.Category?.Name ?? "unknown";
            var manufacturerName = p.Manufacturer?.Name ?? "unknown";
            var price = p.UnitPrice;
            var stock = p.StockQuantity;
            var reserved = p.ReservedQuantity;

            const string sku = "";
            const string description = "";

            return new ProductModel(
                id: p.Id,
                title: titleText,
                category: categoryName,
                manufacturer: manufacturerName,
                sku: sku,
                description: description,
                price: price,
                stock: stock,
                reserved: reserved);
        }
    }
}
