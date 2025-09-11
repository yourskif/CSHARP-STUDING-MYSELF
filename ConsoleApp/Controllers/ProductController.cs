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
    /// Product maintenance controller for console UI.
    /// NOTE: Current ProductService in BLL exposes only read operations.
    /// We keep UI-facing CUD signatures but throw NotSupportedException to compile cleanly.
    /// If you want real CUD, say the word — we'll wire BLL to DAL repositories.
    /// </summary>
    public class ProductController
    {
        private readonly ProductService productService;

        // Accept DbContext and construct repository here (no DI container in ConsoleApp)
        public ProductController(StoreDbContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);
            var productRepo = new ProductRepository(ctx);

            // Your ProductService has a single-parameter constructor
            this.productService = new ProductService(productRepo);
        }

        public List<ProductModel> GetAll()
        {
            return this.productService.GetAll()
                ?.OfType<ProductModel>()
                .ToList()
                ?? new List<ProductModel>();
        }

        public ProductModel? GetById(int id)
        {
            return this.productService.GetById(id) as ProductModel;
        }

        // === Keep UI-facing signatures, but avoid compile errors ===

        // Old UI call: productController.Create(model)
        public void Create(ProductModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            throw new NotSupportedException("Create is not supported via ProductService. Enable CUD in BLL or map directly to DAL.");
        }

        // Old UI call: productController.Update(model)
        public void Update(ProductModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            throw new NotSupportedException("Update is not supported via ProductService. Enable CUD in BLL or map directly to DAL.");
        }

        // Old UI call: productController.Delete(id)
        public void Delete(int id)
        {
            throw new NotSupportedException("Delete is not supported via ProductService. Enable CUD in BLL or map directly to DAL.");
        }

        // UI convenience filter — ProductModel currently has no CategoryName, so return all
        public List<ProductModel> GetByCategory(string categoryName)
        {
            return GetAll();
        }
    }
}
