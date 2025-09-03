using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace My._001_SimpleApp.Models
{
    public static class ProductRepository
    {
        public static List<Product> LoadProducts(string filePath)
        {
            var products = new List<Product>();

            if (!File.Exists(filePath))
                return products;

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length == 4)
                {
                    var product = new Product
                    {
                        Id = int.Parse(parts[0].Trim()),
                        Name = parts[1].Trim(),
                        Price = decimal.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                        Category = parts[3].Trim()
                    };

                    products.Add(product);
                }
            }

            return products;
        }
    }
}
