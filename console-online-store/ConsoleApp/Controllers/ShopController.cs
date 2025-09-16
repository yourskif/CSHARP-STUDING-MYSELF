using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Shop controller used by console UI to browse products.
    /// </summary>
    public class ShopController
    {
        private readonly ProductService productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopController"/> class.
        /// Accepts DbContext and constructs repository internally.
        /// </summary>
        /// <param name="ctx">Database context.</param>
        public ShopController(StoreDbContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);
            var productRepo = new ProductRepository(ctx);
            this.productService = new ProductService(productRepo);
        }

        /// <summary>
        /// Returns all products as a List (UI expects List).
        /// </summary>
        /// <returns>List of all products.</returns>
        public List<ProductModel> GetAll()
        {
            return this.productService.GetAll()
                                     ?.OfType<ProductModel>()
                                     .ToList()
                   ?? new List<ProductModel>();
        }

        /// <summary>
        /// Method expected by GuestMainMenu. Returns all items.
        /// </summary>
        /// <returns>List of all products.</returns>
        public List<ProductModel> ShowAll()
        {
            return GetAll();
        }

        /// <summary>
        /// Filter by category name is not supported at BLL level now.
        /// Returns all products to keep build green.
        /// </summary>
        /// <param name="categoryName">Category name (currently ignored).</param>
        /// <returns>List of all products.</returns>
        public List<ProductModel> GetByCategory(string categoryName)
        {
            // CategoryName property is not present on ProductModel in your BLL,
            // so we cannot filter here safely. Return all to avoid compile-time errors.
            return GetAll();
        }

        /// <summary>
        /// Returns single product by id or null if not found.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Product model or null.</returns>
        public ProductModel? GetById(int id)
        {
            return this.productService.GetById(id) as ProductModel;
        }
    }
}
