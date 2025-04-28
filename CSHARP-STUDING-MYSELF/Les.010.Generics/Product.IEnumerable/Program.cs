using System;
using System.Collections.Generic;

class Product
{
    public string Name { get; set; }
    public double Price { get; set; }

    public Product(string name, double price)
    {
        Name = name;
        Price = price;
    }
}

class Program
{
    static void Main()
    {
        List<Product> products = new List<Product>
        {
            new Product("Laptop", 1200),
            new Product("Mouse", 25),
            new Product("Keyboard", 80),
            new Product("Monitor", 300),
            new Product("USB Cable", 10)
        };

        Console.WriteLine("Cheap products under $100:");
        foreach (var product in GetCheapProducts(products, 100))
        {
            Console.WriteLine($"{product.Name} - ${product.Price}");
        }

        Console.ReadLine();
    }

    static IEnumerable<Product> GetCheapProducts(List<Product> products, double maxPrice)
    {
        foreach (var product in products)
        {
            if (product.Price < maxPrice)
            {
                yield return product;
            }
        }
    }
}
