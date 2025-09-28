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
    /// Enhanced product business logic service with comprehensive search and filtering capabilities.
    /// Provides robust mapping to varying DAL shapes with advanced query support.
    /// Key features:
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
        /// <summary>
        /// Repository instance for data operations.
        /// </summary>
        private readonly object repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="repository">Repository for product data operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when repository is null.</exception>
        public ProductService(IProductRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Convenience constructor used by ConsoleApp wiring.
        /// </summary>
        public ProductService()
            : this(new ProductRepository(StoreDbFactory.Create()))
        {
        }

        /// <summary>
        /// Gets all products from the repository.
        /// </summary>
        /// <returns>List of all product models.</returns>
        public List<ProductModel> GetAll()
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Select(MapToModel).ToList();
        }

        /// <summary>
        /// Gets a product by its identifier.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>Product model if found, null otherwise.</returns>
        public ProductModel? GetById(int id)
        {
            var entity = this.RepoGetById(id);
            return entity is null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Searches for products based on a search term across multiple fields.
        /// </summary>
        /// <param name="searchTerm">Term to search for in title, category, manufacturer.</param>
        /// <returns>List of matching products.</returns>
        public List<ProductModel> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ProductModel>();

            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p =>
                (p.Title?.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                (p.Title?.Category?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                (p.Manufacturer?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true))
            .Select(MapToModel)
            .ToList();
        }

        /// <summary>
        /// Gets products filtered by category name.
        /// </summary>
        /// <param name="categoryName">Category name to filter by.</param>
        /// <returns>List of products in the specified category.</returns>
        public List<ProductModel> GetByCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return new List<ProductModel>();

            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p =>
                string.Equals(p.Title?.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase))
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Gets products filtered by manufacturer name.
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name to filter by.</param>
        /// <returns>List of products from the specified manufacturer.</returns>
        public List<ProductModel> GetByManufacturer(string manufacturerName)
        {
            if (string.IsNullOrWhiteSpace(manufacturerName))
                return new List<ProductModel>();

            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p =>
                string.Equals(p.Manufacturer?.Name, manufacturerName, StringComparison.OrdinalIgnoreCase))
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Gets products within a specified price range.
        /// </summary>
        /// <param name="minPrice">Minimum price (inclusive).</param>
        /// <param name="maxPrice">Maximum price (inclusive).</param>
        /// <returns>List of products within the price range.</returns>
        public List<ProductModel> GetByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p => p.UnitPrice >= minPrice && p.UnitPrice <= maxPrice)
                .Select(MapToModel)
                .ToList();
        }

        /// <summary>
        /// Gets products that are currently available (in stock).
        /// </summary>
        /// <returns>List of available products.</returns>
        public List<ProductModel> GetAvailableProducts()
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p =>
            {
                var stock = ReadInt(p, "StockQuantity", "Stock", "Quantity", "UnitsInStock");
                var reserved = ReadInt(p, "ReservedQuantity", "Reserved");
                return (stock - reserved) > 0;
            })
            .Select(MapToModel)
            .ToList();
        }

        /// <summary>
        /// Gets products with low stock (below specified threshold).
        /// </summary>
        /// <param name="threshold">Stock threshold (default: 10).</param>
        /// <returns>List of products with low stock.</returns>
        public List<ProductModel> GetLowStockProducts(int threshold = 10)
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list.Where(p =>
            {
                var stock = ReadInt(p, "StockQuantity", "Stock", "Quantity", "UnitsInStock");
                var reserved = ReadInt(p, "ReservedQuantity", "Reserved");
                var available = stock - reserved;
                return available > 0 && available <= threshold;
            })
            .Select(MapToModel)
            .OrderBy(p => p.Available)
            .ToList();
        }

        /// <summary>
        /// Gets all available categories from products.
        /// </summary>
        /// <returns>List of distinct category names.</returns>
        public List<string> GetAvailableCategories()
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list
                .Select(p => p.Title?.Category?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList()!;
        }

        /// <summary>
        /// Gets all available manufacturers from products.
        /// </summary>
        /// <returns>List of distinct manufacturer names.</returns>
        public List<string> GetAvailableManufacturers()
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            return list
                .Select(p => p.Manufacturer?.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList()!;
        }

        /// <summary>
        /// Performs advanced search with multiple criteria.
        /// </summary>
        /// <param name="searchTerm">General search term (optional).</param>
        /// <param name="category">Category filter (optional).</param>
        /// <param name="manufacturer">Manufacturer filter (optional).</param>
        /// <param name="minPrice">Minimum price filter (optional).</param>
        /// <param name="maxPrice">Maximum price filter (optional).</param>
        /// <param name="onlyAvailable">Filter only available products.</param>
        /// <returns>List of products matching all specified criteria.</returns>
        public List<ProductModel> AdvancedSearch(
            string? searchTerm = null,
            string? category = null,
            string? manufacturer = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool onlyAvailable = false)
        {
            var list = this.RepoGetAll() ?? Array.Empty<Product>();
            var query = list.AsEnumerable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    (p.Title?.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                    (p.Title?.Category?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                    (p.Manufacturer?.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                    (p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true));
            }

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p =>
                    string.Equals(p.Title?.Category?.Name, category, StringComparison.OrdinalIgnoreCase));
            }

            // Apply manufacturer filter
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                query = query.Where(p =>
                    string.Equals(p.Manufacturer?.Name, manufacturer, StringComparison.OrdinalIgnoreCase));
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice <= maxPrice.Value);
            }

            // Apply availability filter
            if (onlyAvailable)
            {
                query = query.Where(p =>
                {
                    var stock = ReadInt(p, "StockQuantity", "Stock", "Quantity", "UnitsInStock");
                    var reserved = ReadInt(p, "ReservedQuantity", "Reserved");
                    return (stock - reserved) > 0;
                });
            }

            return query.Select(MapToModel).ToList();
        }

        /// <summary>
        /// Adds a new product to the repository.
        /// </summary>
        /// <param name="title">Product title.</param>
        /// <param name="category">Category name.</param>
        /// <param name="manufacturer">Manufacturer name.</param>
        /// <param name="sku">Stock keeping unit (UI-only).</param>
        /// <param name="description">Product description (UI-only).</param>
        /// <param name="price">Unit price.</param>
        /// <param name="stock">Initial stock quantity.</param>
        /// <returns>Created product model.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when price or stock is negative.</exception>
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
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative");
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

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <param name="title">New product title.</param>
        /// <param name="category">New category name.</param>
        /// <param name="manufacturer">New manufacturer name.</param>
        /// <param name="sku">New SKU (UI-only).</param>
        /// <param name="description">New description (UI-only).</param>
        /// <param name="price">New unit price.</param>
        /// <param name="stock">New stock quantity.</param>
        /// <returns>Updated product model if found, null otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when price or stock is negative.</exception>
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
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative");
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

        /// <summary>
        /// Deletes a product by identifier.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>True if deleted successfully, false if not found.</returns>
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

        /// <summary>
        /// Maps a product entity to a product model with robust field handling.
        /// </summary>
        /// <param name="p">Product entity to map.</param>
        /// <returns>Mapped product model.</returns>
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

        /// <summary>
        /// Reads an integer value from an object using reflection with multiple property name fallbacks.
        /// </summary>
        /// <param name="obj">Object to read from.</param>
        /// <param name="names">Property names to try in order.</param>
        /// <returns>Integer value found, or 0 if none found.</returns>
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

        /// <summary>
        /// Attempts to set an integer value on an object using reflection with multiple property name fallbacks.
        /// </summary>
        /// <param name="obj">Object to set value on.</param>
        /// <param name="value">Value to set.</param>
        /// <param name="names">Property names to try in order.</param>
        /// <returns>True if successfully set, false otherwise.</returns>
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
                {
                    /* try next name */
                }
            }

            return false;
        }

        /// <summary>
        /// Reads a struct value from an object using reflection.
        /// </summary>
        /// <typeparam name="T">Struct type to read.</typeparam>
        /// <param name="obj">Object to read from.</param>
        /// <param name="name">Property name.</param>
        /// <returns>Value if found and convertible, null otherwise.</returns>
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

        // Repository wrapper methods using dynamic calls for flexibility
        #region Repository Wrappers

        /// <summary>
        /// Gets all products from repository with multiple method name fallbacks.
        /// </summary>
        /// <returns>Enumerable of products or null if not found.</returns>
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

        /// <summary>
        /// Gets a product by ID from repository with multiple method name fallbacks.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>Product if found, null otherwise.</returns>
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

        /// <summary>
        /// Adds a product to repository with multiple method name fallbacks.
        /// </summary>
        /// <param name="p">Product to add.</param>
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

        /// <summary>
        /// Updates a product in repository with multiple method name fallbacks.
        /// </summary>
        /// <param name="p">Product to update.</param>
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

        /// <summary>
        /// Deletes a product by ID from repository with multiple method name fallbacks.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>True if method was called successfully, false otherwise.</returns>
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

        /// <summary>
        /// Deletes a product entity from repository with multiple method name fallbacks.
        /// </summary>
        /// <param name="p">Product to delete.</param>
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

        /// <summary>
        /// Saves changes to repository.
        /// </summary>
        private void RepoSaveChanges()
        {
            dynamic repo = this.repository;
            try
            {
                repo.SaveChanges();
            }
            catch
            {
                /* repository may auto-commit */
            }
        }

        #endregion
    }
}
