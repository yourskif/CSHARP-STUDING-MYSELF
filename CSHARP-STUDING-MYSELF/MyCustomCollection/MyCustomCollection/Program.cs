using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCustomCollection
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductCollection : IEnumerable<Product>
    {
        private Dictionary<string, Product> items = new Dictionary<string, Product>();


        // 🔔 Делегат
        public delegate void ProductAddedEventHandler(object sender, ProductEventArgs e);

        // 📢 Подія
        public event ProductAddedEventHandler ProductAdded;


        public void Add(Product product)
        {
            if (product == null)
                throw new ArgumentException("Товар не може бути null.");

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Назва товару не може бути порожньою.");

            if (product.Price < 0)
                throw new ArgumentException("Ціна товару не може бути від’ємною.");

            if (items.ContainsKey(product.Name))
                throw new ArgumentException($"Товар з назвою '{product.Name}' вже існує.");

            items.Add(product.Name, product);

            // 🔔 Виклик події
            OnProductAdded(product);
        }

        protected virtual void OnProductAdded(Product product)
        {
            ProductAdded?.Invoke(this, new ProductEventArgs(product));
        }


        public IEnumerator<Product> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ProductEventArgs : EventArgs
    {
        public Product Product { get; }

        public ProductEventArgs(Product product)
        {
            Product = product;
        }
    }

    public static class ProductExtensions
    {
        // Пошук дорогих товарів
        public static IEnumerable<Product> FindExpensiveProducts(this IEnumerable<Product> products, decimal minPrice)
        {
            return products.Where(p => p.Price > minPrice);
        }

        // Обчислення середньої ціни
        public static decimal AveragePrice(this IEnumerable<Product> products)
        {
            return products.Average(p => p.Price);
        }

        // Пошук товарів у певному діапазоні цін
        public static IEnumerable<Product> FindProductsInPriceRange(this IEnumerable<Product> products, decimal minPrice, decimal maxPrice)
        {
            return products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
        }

        // Сортування товарів за назвою
        public static IEnumerable<Product> SortByName(this IEnumerable<Product> products)
        {
            return products.OrderBy(p => p.Name);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            var products = new ProductCollection();

            products.ProductAdded += Products_ProductAdded;

            // Додаємо товари
            products.Add(new Product { Name = "Laptop", Price = 1500 });
            products.Add(new Product { Name = "Phone", Price = 800 });
            products.Add(new Product { Name = "Tablet", Price = 1200 });

            // Виведення всіх товарів
            Console.WriteLine("All products:");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Name}: {p.Price}");
            }

            // Фільтрація дорогих товарів
            var expensiveProducts = products.Where(p => p.Price > 1000).ToList();

            Console.WriteLine("\nChoose an option (1-5):");
            Console.WriteLine("1. Print all expensive products");
            Console.WriteLine("2. Access the first expensive product");
            Console.WriteLine("3. Sort expensive products by price");
            Console.WriteLine("4. Find the most expensive product");
            Console.WriteLine("5. Check if there are any expensive products");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    // Виведення всіх дорогих товарів
                    Console.WriteLine("Expensive products:");
                    foreach (var product in expensiveProducts)
                    {
                        Console.WriteLine($"{product.Name}: {product.Price}");
                    }
                    break;

                case 2:
                    // Доступ до першого дорогого товару
                    if (expensiveProducts.Any())
                    {
                        var firstExpensiveProduct = expensiveProducts[0];
                        Console.WriteLine($"First expensive product: {firstExpensiveProduct.Name} - {firstExpensiveProduct.Price}");
                    }
                    else
                    {
                        Console.WriteLine("No expensive products found.");
                    }
                    break;

                case 3:
                    // Сортування дорогих товарів за ціною
                    var sortedExpensiveProducts = expensiveProducts.OrderBy(p => p.Price).ToList();
                    Console.WriteLine("Sorted expensive products:");
                    foreach (var product in sortedExpensiveProducts)
                    {
                        Console.WriteLine($"{product.Name}: {product.Price}");
                    }
                    break;

                case 4:
                    // Знаходження найдорожчого товару
                    var mostExpensiveProduct = expensiveProducts.OrderByDescending(p => p.Price).FirstOrDefault();
                    if (mostExpensiveProduct != null)
                    {
                        Console.WriteLine($"Most expensive product: {mostExpensiveProduct.Name} - {mostExpensiveProduct.Price}");
                    }
                    else
                    {
                        Console.WriteLine("No expensive products found.");
                    }
                    break;

                case 5:
                    // Перевірка наявності дорогих товарів
                    if (expensiveProducts.Any())
                    {
                        Console.WriteLine("There are expensive products.");
                    }
                    else
                    {
                        Console.WriteLine("No expensive products found.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            Console.WriteLine("\nChoose an option (1-4) for extension methods:");
            Console.WriteLine("1. Find expensive products");
            Console.WriteLine("2. Calculate average price");
            Console.WriteLine("3. Find products in price range");
            Console.WriteLine("4. Sort products by name");

            int choiceExtend = int.Parse(Console.ReadLine());

            switch (choiceExtend)
            {
                case 1:
                    // Знайти дорогі товари
                    var expensiveExtendProducts = products.FindExpensiveProducts(1000);
                    Console.WriteLine("\nExpensive products:");
                    foreach (var product in expensiveExtendProducts)
                    {
                        Console.WriteLine($"{product.Name}: {product.Price}");
                    }
                    break;

                case 2:
                    // Обчислити середню ціну
                    decimal avgPrice = products.AveragePrice();
                    Console.WriteLine($"Average price: {avgPrice}");
                    break;

                case 3:
                    // Знайти товари в діапазоні цін
                    var rangeProducts = products.FindProductsInPriceRange(500, 1500);
                    Console.WriteLine("\nProducts in price range 500-1500:");
                    foreach (var product in rangeProducts)
                    {
                        Console.WriteLine($"{product.Name}: {product.Price}");
                    }
                    break;

                case 4:
                    // Сортувати товари за назвою
                    var sortedByName = products.SortByName();
                    Console.WriteLine("\nSorted products by name:");
                    foreach (var product in sortedByName)
                    {
                        Console.WriteLine($"{product.Name}: {product.Price}");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        
        Console.ReadLine();
        }

        // 🎯 Обробник події
        private static void Products_ProductAdded(object sender, ProductEventArgs e)
        {
            Console.WriteLine($"📢 Додано товар: {e.Product.Name}, ціна: {e.Product.Price}");
        }
    }
}
