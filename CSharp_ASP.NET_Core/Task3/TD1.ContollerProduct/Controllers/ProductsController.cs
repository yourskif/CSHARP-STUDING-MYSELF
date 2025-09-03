using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TD1.ContollerProduct
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", Price = 10.99 },
                new Product { Id = 2, Name = "Product B", Price = 20.49 },
                new Product { Id = 3, Name = "Product C", Price = 15.30 }
            };

            ViewBag.Products = products;

            return View(products); // Передаємо модель у View
        }
    }
}
