using Microsoft.AspNetCore.Mvc;
using My._001_SimpleApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace My._001_SimpleApp.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult List(string? category)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "data.txt");
            List<Product> products = ProductRepository.LoadProducts(filePath);

            if (!string.IsNullOrEmpty(category))
            {
                products = products
                    .Where(p => p.Category.ToLower() == category.ToLower())
                    .ToList();
            }

            return View(products);
        }
    }
}
