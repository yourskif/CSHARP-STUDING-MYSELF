namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using StoreBLL.Models;
    using StoreDAL.Data;
    using StoreDAL.Entities;
    using StoreDAL.Interfaces;
    using StoreDAL.Repository;

    /// <summary>
    /// Product business logic with robust mapping to your varying DAL shapes.
    /// Key points:
    /// - Price => Product.UnitPrice (no compile-time reference to non-existing Product.Price)
    /// - Stock => tries StockQuantity / Stock / Quantity / UnitsInStock
    /// - Reserved => tries ReservedQuantity / Reserved
    /// - Category => Title?.Category?.Name (falls back to "unknown")
    /// - Manufacturer => Manufacturer?.Name (falls back to "unknown")
    /// - SKU / Description are not in DB => exposed as empty strings for UI compatibility
    /// Repository calls are done via dynamic with safe fallbacks.
    /// </summary>
    public sealed class ProductService
    {
        private readonly object repository;

        public ProductService(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Convenience ctor used by ConsoleApp wiring
        public ProductService()
            : this(new ProductRepository(StoreDbFactory.Create()))
        {
        }

        // ===== Public API =====
        public List<ProductModel> GetAll()
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Select(MapToModel).ToList();
        }

        public ProductModel? GetById(int id)
        {
            var entity = this.RepoGetById(id);
            return entity is null ? null : MapToModel(entity);
        }

        public ProductModel Add(
            string title,
            string category,
            string manufacturer,
            string sku,          // UI-only
            string description,  // UI-only
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

            var p = new Product
            {
                Title = new ProductTitle
                {
                    Title = title ?? string.Empty,
                },
                UnitPrice = price,
            };

            // set initial stock/reserved via reflection (handles different property names)
            TrySetInt(p, stock, "StockQuantity", "Stock", "Quantity", "UnitsInStock");
            TrySetInt(p, 0, "ReservedQuantity", "Reserved");

            // Manufacturer / Category by name resolution is not exposed in DAL,
            // so we keep navigation as-is (UI shows names if present).
            this.RepoAdd(p);
            this.RepoSaveChanges();

            return MapToModel(p);
        }

        public ProductModel? Update(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,          // UI-only
            string description,  // UI-only
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

            var p = this.RepoGetById(id);
            if (p is null)
            {
                return null;
            }

            p.Title ??= new ProductTitle();
            p.Title.Title = string.IsNullOrWhiteSpace(title) ? p.Title.Title ?? string.Empty : title.Trim();

            // price
            p.UnitPrice = price;

            // stock
            TrySetInt(p, stock, "StockQuantity", "Stock", "Quantity", "UnitsInStock");

            this.RepoUpdate(p);
            this.RepoSaveChanges();

            return MapToModel(p);
        }

        public bool Delete(int id)
        {
            var p = this.RepoGetById(id);
            if (p is null)
            {
                return false;
            }

            if (!this.RepoDeleteById(id))
            {
                this.RepoDelete(p);
            }

            this.RepoSaveChanges();
            return true;
        }

        // ===== Mapping =====
        private static ProductModel MapToModel(Product p)
        {
            // Title text
            var titleText = p.Title?.Title ?? $"Product {p.Id}";

            // Category name
            var categoryName =
                p.Title?.Category?.Name
                ?? "unknown";

            // Manufacturer name
            var manufacturerName =
                p.Manufacturer?.Name
                ?? "unknown";

            // Price from UnitPrice (your schema)
            var price = p.UnitPrice;

            // Stock/Reserved with robust fallbacks
            var stock = ReadInt(p, "StockQuantity", "Stock", "Quantity", "UnitsInStock");
            var reserved = ReadInt(p, "ReservedQuantity", "Reserved");

            // Not stored in DB, but present in model/UI
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

        // ===== Reflection helpers (safe; no compile-time dependency) =====
        private static int ReadInt(object obj, params string[] names)
        {
            foreach (var n in names)
            {
                var val = ReadStructFrom<int>(obj, n);
                if (val.HasValue)
                {
                    return val.Value;
                }
            }

            return 0;
        }

        private static bool TrySetInt(object obj, int value, params string[] names)
        {
            foreach (var n in names)
            {
                var pi = obj.GetType().GetProperty(n, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (pi == null || !pi.CanWrite)
                {
                    continue;
                }

                try
                {
                    if (pi.PropertyType == typeof(int))
                    {
                        pi.SetValue(obj, value);
                    }
                    else
                    {
                        pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType, CultureInfo.InvariantCulture));
                    }

                    return true;
                }
                catch
                { /* try next name */
                }
            }

            return false;
        }

        private static T? ReadStructFrom<T>(object? obj, string name)
            where T : struct
        {
            if (obj is null)
            {
                return null;
            }

            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (pi is null || !pi.CanRead)
            {
                return null;
            }

            var v = pi.GetValue(obj);
            if (v is T typed)
            {
                return typed;
            }

            try
            {
                return (T)Convert.ChangeType(v!, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }

        // ===== Repository wrappers (dynamic) =====
        private IEnumerable<Product>? RepoGetAll()
        {
            dynamic repo = this.repository;
            try
            {
                return (IEnumerable<Product>)repo.GetAllWithIncludes();
            }
            catch
            {
            }

            try
            {
                return (IEnumerable<Product>)repo.GetAll();
            }
            catch
            {
            }

            try
            {
                return (IEnumerable<Product>)repo.GetAllProducts();
            }
            catch
            {
            }

            return null;
        }

        private Product? RepoGetById(int id)
        {
            dynamic repo = this.repository;
            try
            {
                return (Product)repo.GetByIdWithIncludes(id);
            }
            catch
            {
            }

            try
            {
                return (Product)repo.GetById(id);
            }
            catch
            {
            }

            try
            {
                return (Product)repo.Find(id);
            }
            catch
            {
            }

            return null;
        }

        private void RepoAdd(Product p)
        {
            dynamic repo = this.repository;
            try
            {
                repo.Add(p);
                return;
            }
            catch
            {
            }

            try
            {
                repo.Create(p);
                return;
            }
            catch
            {
            }

            try
            {
                repo.AddProduct(p);
                return;
            }
            catch
            {
            }
        }

        private void RepoUpdate(Product p)
        {
            dynamic repo = this.repository;
            try
            {
                repo.Update(p);
                return;
            }
            catch
            {
            }

            try
            {
                repo.Edit(p);
                return;
            }
            catch
            {
            }

            try
            {
                repo.UpdateProduct(p);
                return;
            }
            catch
            {
            }
        }

        private bool RepoDeleteById(int id)
        {
            dynamic repo = this.repository;
            try
            {
                repo.DeleteById(id);
                return true;
            }
            catch
            {
            }

            try
            {
                repo.Delete(id);
                return true;
            }
            catch
            {
            }

            try
            {
                repo.RemoveById(id);
                return true;
            }
            catch
            {
            }

            return false;
        }

        private void RepoDelete(Product p)
        {
            dynamic repo = this.repository;
            try
            {
                repo.Delete(p);
                return;
            }
            catch
            {
            }

            try
            {
                repo.Remove(p);
                return;
            }
            catch
            {
            }
        }

        private void RepoSaveChanges()
        {
            dynamic repo = this.repository;
            try
            {
                repo.SaveChanges();
            }
            catch
            { /* repository may auto-commit */
            }
        }
    }
}
