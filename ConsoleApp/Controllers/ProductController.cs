// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\ProductController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Repository;

/// <summary>
/// Product maintenance controller for console UI.
/// Now wired to full ProductService CUD.
/// </summary>
public class ProductController
{
    private readonly ProductService productService;

    /// <summary>
    /// Accepts DbContext and builds required repositories (no DI container in ConsoleApp).
    /// </summary>
    public ProductController(StoreDbContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        var productRepo = new ProductRepository(ctx);
        this.productService = new ProductService(productRepo);
    }

    // -------- READ --------

    public List<ProductModel> GetAll()
    {
        return this.productService.GetAll().ToList();
    }

    public ProductModel? GetById(int id)
    {
        return this.productService.GetById(id);
    }

    /// <summary>
    /// UI convenience filter. For now it returns all; category filter can be added later.
    /// </summary>
    public List<ProductModel> GetByCategory(string categoryName) => GetAll();

    // -------- CUD --------

    /// <summary>
    /// Creates a product. Expectation for this step:
    /// - model.Category.Id contains ProductTitleId,
    /// - model.Manufacturer.Id contains ManufacturerId,
    /// - model.Description/Price/Stock are provided.
    /// </summary>
    public void Create(ProductModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var created = this.productService.Create(model);
        Console.WriteLine($"Product created: #{created.Id} '{created.Title}' price={created.Price} stock={created.Stock}");
    }

    /// <summary>
    /// Updates a product. Same ID convention as in Create.
    /// </summary>
    public void Update(ProductModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        this.productService.Update(model);
        Console.WriteLine($"Product #{model.Id} updated.");
    }

    public void Delete(int id)
    {
        var ok = this.productService.Delete(id);
        Console.WriteLine(ok ? $"Product #{id} deleted." : $"Product #{id} not found.");
    }
}
