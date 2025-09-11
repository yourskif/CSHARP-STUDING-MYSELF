// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\ProductService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;

using StoreDAL.Entities;
using StoreDAL.Interfaces;

/// <summary>
/// BLL service for products. Maps DAL entities to BLL models using
/// UnitPrice/Stock and tries Title->Category/Manufacturer names when available.
/// </summary>
public class ProductService
{
    private readonly IProductRepository repo;

    public ProductService(IProductRepository repository)
    {
        this.repo = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IEnumerable<ProductModel> GetAll()
    {
        // Prefer includes to have titles/categories; repository returns them.
        var items = this.repo.GetAllWithIncludes();
        return items.Select(Map);
    }

    public ProductModel? GetById(int id)
    {
        var p = this.repo.GetByIdWithIncludes(id);
        return p is null ? null : Map(p);
    }

    private static ProductModel Map(Product p)
    {
        // Title text: prefer ProductTitle.Title, fallback to Product.Description
        var titleText = p.Title?.Title ?? p.Description ?? string.Empty;
        var categoryName = p.Title?.Category?.Name;
        var manufacturerName = p.Manufacturer?.Name;

        return new ProductModel(
            id: p.Id,
            title: titleText,
            category: categoryName ?? string.Empty,
            manufacturer: manufacturerName ?? string.Empty,
            sku: string.Empty, // no SKU column in schema
            description: p.Description ?? string.Empty,
            price: p.UnitPrice,
            stock: p.Stock
        );
    }
}
