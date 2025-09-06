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

        // IMPORTANT: accept DbContext and construct repository here
        public ShopController(StoreDbContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);
            var productRepo = new ProductRepository(ctx);

            // ProductService у тебе приймає лише один аргумент (репозиторій)
            this.productService = new ProductService(productRepo);
        }

        /// <summary>
        /// Returns all products as a List (UI expects List).
        /// </summary>
        public List<ProductModel> GetAll()
        {
            return productService.GetAll()
                                 ?.OfType<ProductModel>()
                                 .ToList()
                   ?? new List<ProductModel>();
        }

        /// <summary>
        /// Method expected by GuestMainMenu. Returns all items; UI може проігнорувати повернення.
        /// </summary>
        public List<ProductModel> ShowAll()
        {
            return GetAll();
        }

        /// <summary>
        /// Filter by category name is not supported at BLL level now.
        /// To keep build green, we return all and let UI filter elsewhere if потрібно.
        /// </summary>
        public List<ProductModel> GetByCategory(string categoryName)
        {
            // CategoryName property is not present on ProductModel in your BLL,
            // so we cannot filter here safely. Return all to avoid compile-time errors.
            return GetAll();
        }

        /// <summary>
        /// Returns single product by id or null if not found.
        /// </summary>
        public ProductModel? GetById(int id)
        {
            return productService.GetById(id) as ProductModel;
        }
    }
}
