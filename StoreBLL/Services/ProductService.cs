// Path: console-online-store/StoreBLL/Services/ProductService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoreBLL.Models;

    using StoreDAL.Entities;
    using StoreDAL.UnitOfWork;

    /// <summary>
    /// Product business logic with Unit of Work for transaction management.
    /// </summary>
    public sealed class ProductService
    {
        private readonly IStoreUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
        public ProductService(IStoreUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public List<ProductModel> GetAll()
        {
            var list = this.unitOfWork.Products.GetAllWithIncludes() ?? Array.Empty<Product>();
            return list.Select(MapToModel).ToList();
        }

        public ProductModel? GetById(int id)
        {
            var entity = this.unitOfWork.Products.GetByIdWithIncludes(id);
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

            var productTitle = this.unitOfWork.Context.ProductTitles
                .FirstOrDefault(pt => pt.Title == title);

            if (productTitle == null)
            {
                var cat = this.unitOfWork.Context.Categories.FirstOrDefault(c => c.Name == category);
                if (cat == null)
                {
                    cat = new Category { Name = category };
                    this.unitOfWork.Context.Categories.Add(cat);
                    this.unitOfWork.SaveChanges();
                }

                productTitle = new ProductTitle
                {
                    Title = title,
                    CategoryId = cat.Id,
                };
                this.unitOfWork.Context.ProductTitles.Add(productTitle);
                this.unitOfWork.SaveChanges();
            }

            var manu = this.unitOfWork.Context.Manufacturers.FirstOrDefault(m => m.Name == manufacturer);
            if (manu == null)
            {
                manu = new Manufacturer { Name = manufacturer };
                this.unitOfWork.Context.Manufacturers.Add(manu);
                this.unitOfWork.SaveChanges();
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

            this.unitOfWork.Context.Products.Add(p);
            this.unitOfWork.SaveChanges();

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

            var p = this.unitOfWork.Context.Products
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

            this.unitOfWork.SaveChanges();

            return MapToModel(p);
        }

        public bool Delete(int id)
        {
            var p = this.unitOfWork.Context.Products.FirstOrDefault(p => p.Id == id);
            if (p is null)
            {
                return false;
            }

            this.unitOfWork.Context.Products.Remove(p);
            this.unitOfWork.SaveChanges();
            return true;
        }

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
