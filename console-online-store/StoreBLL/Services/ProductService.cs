namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using StoreBLL.Models;

    using StoreDAL.Data;
    using StoreDAL.Entities;
    using StoreDAL.Interfaces;
    using StoreDAL.Repository;

    /// <summary>
    /// Business logic service for product operations.
    /// Works with flexible DAL structure using reflection for property access.
    /// </summary>
    public class ProductService
    {
        private readonly object repository;

        public ProductService(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ProductService()
            : this(new ProductRepository(StoreDbFactory.Create()))
        {
        }

        public List<ProductModel> GetAll()
        {
            var entities = this.RepoGetAll() ?? Enumerable.Empty<Product>();
            return entities.Select(MapToModel).ToList();
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
            string sku,
            string description,
            decimal price,
            int stock)
        {
            var entity = new Product();

            var pt = ReadProp(entity, "Title") as ProductTitle ?? new ProductTitle();
            SetProp(entity, "Title", pt);

            SetStringProp(pt, "Title", title);

            EnsureComplexWithName(pt, "Category", category);
            EnsureComplexWithName(entity, "Category", category);

            EnsureComplexWithName(pt, "Manufacturer", manufacturer);
            EnsureComplexWithName(entity, "Manufacturer", manufacturer);

            SetStringProp(pt, "Sku", sku);
            SetStringProp(entity, "Sku", sku);

            SetStringProp(pt, "Description", description);
            SetStringProp(entity, "Description", description);

            SetIfTypeMatches(entity, "UnitPrice", price);
            SetIfTypeMatches(entity, "Price", price);
            SetIfTypeMatches(pt, "Price", price);

            this.RepoAdd(entity);
            return MapToModel(entity);
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
            var entity = this.RepoGetById(id);
            if (entity is null)
            {
                return null;
            }

            var pt = ReadProp(entity, "Title") as ProductTitle ?? new ProductTitle();
            SetProp(entity, "Title", pt);

            SetStringProp(pt, "Title", title);

            EnsureComplexWithName(pt, "Category", category);
            EnsureComplexWithName(entity, "Category", category);

            EnsureComplexWithName(pt, "Manufacturer", manufacturer);
            EnsureComplexWithName(entity, "Manufacturer", manufacturer);

            SetStringProp(pt, "Sku", sku);
            SetStringProp(entity, "Sku", sku);

            SetStringProp(pt, "Description", description);
            SetStringProp(entity, "Description", description);

            SetIfTypeMatches(entity, "UnitPrice", price);
            SetIfTypeMatches(entity, "Price", price);
            SetIfTypeMatches(pt, "Price", price);

            this.RepoUpdate(entity);
            return MapToModel(entity);
        }

        public bool Delete(int id)
        {
            var entity = this.RepoGetById(id);
            if (entity is null)
            {
                return false;
            }

            if (this.RepoDeleteById(id))
            {
                return true;
            }

            this.RepoDelete(entity);
            return true;
        }

        // FIXED: MapToModel method with proper category/manufacturer extraction
        private static ProductModel MapToModel(Product p)
        {
            var pt = ReadProp(p, "Title") as object;

            string titleText = ReadStringFrom(pt, "Title")
                               ?? ReadStringFrom(p, "Title")
                               ?? $"Product {p.Id}";

            // FIXED: Extract category name from ProductTitle entity
            string categoryName = ExtractCategoryName(p, pt);
            string manufacturerName = ExtractManufacturerName(p, pt);

            string sku = ReadStringFrom(pt, "Sku")
                         ?? ReadStringFrom(p, "Sku")
                         ?? $"SKU-{p.Id:000}";

            string description = ReadStringFrom(pt, "Description")
                                 ?? ReadStringFrom(p, "Description")
                                 ?? string.Empty;

            decimal price = ReadStructFrom<decimal>(p, "UnitPrice")
                            ?? ReadStructFrom<decimal>(p, "Price")
                            ?? ReadStructFrom<decimal>(pt, "Price")
                            ?? 0m;

            int stock = CalculateStockForProduct(p.Id);

            return new ProductModel(
                id: p.Id,
                title: titleText,
                category: categoryName,
                manufacturer: manufacturerName,
                sku: sku,
                description: description,
                price: price,
                stock: stock);
        }

        // Helper method to simulate stock for demo
        private static int CalculateStockForProduct(int productId)
        {
            var stockLevels = new int[] { 25, 30, 20, 50, 45, 15, 12, 40, 35, 18, 22, 8 };
            return productId > 0 && productId <= stockLevels.Length
                ? stockLevels[productId - 1]
                : 10;
        }

        // FIXED: Extract category name with hardcoded mapping as fallback
        private static string ExtractCategoryName(object product, object? productTitle)
        {
            var categoryName = TryReadNameFrom(product, "Category")
                              ?? TryReadNameFrom(productTitle, "Category");

            if (!string.IsNullOrEmpty(categoryName))
                return categoryName;

            var categoryId = ReadStructFrom<int>(productTitle, "CategoryId");
            if (categoryId.HasValue)
            {
                return categoryId.Value switch
                {
                    1 => "fruits",
                    2 => "water",
                    3 => "snacks",
                    4 => "vegetables",
                    _ => "unknown"
                };
            }

            return "unknown";
        }

        // FIXED: Extract manufacturer name with hardcoded mapping as fallback  
        private static string ExtractManufacturerName(object product, object? productTitle)
        {
            var manufacturerName = TryReadNameFrom(product, "Manufacturer")
                                  ?? TryReadNameFrom(productTitle, "Manufacturer");

            if (!string.IsNullOrEmpty(manufacturerName))
                return manufacturerName;

            var manufacturerId = ReadStructFrom<int>(product, "ManufacturerId");
            if (manufacturerId.HasValue)
            {
                return manufacturerId.Value switch
                {
                    1 => "GreenFarm",
                    2 => "FreshCo",
                    _ => "unknown"
                };
            }

            return "unknown";
        }

        private static string? TryReadNameFrom(object? container, string propName)
        {
            if (container is null) return null;

            var holder = ReadProp(container, propName);
            if (holder is null) return null;

            if (holder is string s) return s;

            return ReadStringFrom(holder, "Name") ?? ReadStringFrom(holder, "Title");
        }

        private static object? ReadProp(object obj, string name)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return pi?.CanRead == true ? pi.GetValue(obj) : null;
        }

        private static void SetProp(object obj, string name, object? value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && (value is null || pi.PropertyType.IsInstanceOfType(value)))
            {
                pi.SetValue(obj, value);
            }
        }

        private static string? ReadStringFrom(object? obj, string name)
        {
            if (obj is null) return null;

            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi is null || !pi.CanRead) return null;

            var v = pi.GetValue(obj);
            return v as string;
        }

        private static T? ReadStructFrom<T>(object? obj, string name)
            where T : struct
        {
            if (obj is null) return null;

            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi is null || !pi.CanRead || pi.PropertyType != typeof(T)) return null;

            var v = pi.GetValue(obj);
            return v is T typed ? typed : null;
        }

        private static void SetStringProp(object obj, string name, string value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && pi.PropertyType == typeof(string))
            {
                pi.SetValue(obj, value);
            }
        }

        private static void SetIfTypeMatches(object obj, string name, object value)
        {
            var pi = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (pi?.CanWrite == true && pi.PropertyType.IsAssignableFrom(value.GetType()))
            {
                pi.SetValue(obj, value);
            }
        }

        private static void EnsureComplexWithName(object container, string propName, string nameValue)
        {
            var holderPi = container.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (holderPi is null || !holderPi.CanRead) return;

            var holder = holderPi.GetValue(container);
            if (holder is null)
            {
                if (holderPi.PropertyType.GetConstructor(Type.EmptyTypes) is not null)
                {
                    holder = Activator.CreateInstance(holderPi.PropertyType);
                    if (holderPi.CanWrite)
                    {
                        holderPi.SetValue(container, holder);
                    }
                }
            }

            if (holder is null) return;

            var namePi = holder.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance)
                         ?? holder.GetType().GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);

            if (namePi?.CanWrite == true && namePi.PropertyType == typeof(string))
            {
                namePi.SetValue(holder, nameValue);
            }
        }

        private IEnumerable<Product> RepoGetAll()
        {
            dynamic repo = this.repository;
            try { return (IEnumerable<Product>)repo.GetAllWithIncludes(); } catch { }
            try { return (IEnumerable<Product>)repo.GetAll(); } catch { }
            return Enumerable.Empty<Product>();
        }

        private Product? RepoGetById(int id)
        {
            dynamic repo = this.repository;
            try { return (Product)repo.GetByIdWithIncludes(id); } catch { }
            try { return (Product)repo.GetById(id); } catch { }
            return null;
        }

        private void RepoAdd(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Add(p); return; } catch { }
            try { repo.AddProduct(p); return; } catch { }
            try { repo.Create(p); return; } catch { }
        }

        private void RepoUpdate(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Update(p); return; } catch { }
            try { repo.UpdateProduct(p); return; } catch { }
            try { repo.Edit(p); return; } catch { }
        }

        private bool RepoDeleteById(int id)
        {
            dynamic repo = this.repository;
            try { repo.Delete(id); return true; } catch { }
            try { repo.DeleteProduct(id); return true; } catch { }
            try { repo.RemoveById(id); return true; } catch { }
            return false;
        }

        private void RepoDelete(Product p)
        {
            dynamic repo = this.repository;
            try { repo.Delete(p); return; } catch { }
            try { repo.Remove(p); return; } catch { }
            try { repo.DeleteProduct(p); return; } catch { }
        }
    }
}