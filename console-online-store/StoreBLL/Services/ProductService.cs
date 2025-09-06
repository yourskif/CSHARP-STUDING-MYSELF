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
/// Adds CUD methods for admin flows.
/// </summary>
public class ProductService
{
    private readonly IProductRepository repo;

    public ProductService(IProductRepository repository)
    {
        this.repo = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // ---------- READ ----------
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

    // ---------- CUD ----------
    /// <summary>
    /// Creates a product from explicit foreign keys (ProductTitleId, ManufacturerId).
    /// </summary>
    public ProductModel CreateFromIds(int productTitleId, int manufacturerId, string description, decimal price, int stock)
    {
        if (productTitleId <= 0) throw new ArgumentOutOfRangeException(nameof(productTitleId));
        if (manufacturerId <= 0) throw new ArgumentOutOfRangeException(nameof(manufacturerId));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
        if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock));

        var e = new Product
        {
            TitleId = productTitleId,
            ManufacturerId = manufacturerId,
            Description = description ?? string.Empty,
            UnitPrice = price,
            Stock = stock,
        };

        var created = this.repo.Add(e);
        var reloaded = this.repo.GetByIdWithIncludes(created.Id)!;
        return Map(reloaded);
    }

    /// <summary>
    /// Updates a product by Id using explicit foreign keys.
    /// </summary>
    public void UpdateFromIds(int id, int productTitleId, int manufacturerId, string description, decimal price, int stock)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (productTitleId <= 0) throw new ArgumentOutOfRangeException(nameof(productTitleId));
        if (manufacturerId <= 0) throw new ArgumentOutOfRangeException(nameof(manufacturerId));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
        if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock));

        var e = new Product
        {
            Id = id,
            TitleId = productTitleId,
            ManufacturerId = manufacturerId,
            Description = description ?? string.Empty,
            UnitPrice = price,
            Stock = stock,
        };
        this.repo.Update(e);
    }

    /// <summary>
    /// Deletes a product by Id.
    /// </summary>
    public bool Delete(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        return this.repo.Delete(id);
    }

    /// <summary>
    /// Thin wrapper that expects IDs inside ProductModel:
    /// - Category.Id is used as ProductTitleId (temporary convention for this step),
    /// - Manufacturer.Id is used as ManufacturerId.
    /// </summary>
    public ProductModel Create(ProductModel m)
    {
        ArgumentNullException.ThrowIfNull(m);
        var titleId = m.Category?.Id ?? 0;      // TEMP: Category.Id as ProductTitleId
        var manId = m.Manufacturer?.Id ?? 0;    // Manufacturer.Id
        return CreateFromIds(titleId, manId, m.Description, m.Price, m.Stock);
    }

    /// <summary>
    /// Thin wrapper for updates (uses the same temporary ID convention).
    /// </summary>
    public void Update(ProductModel m)
    {
        ArgumentNullException.ThrowIfNull(m);
        var titleId = m.Category?.Id ?? 0;
        var manId = m.Manufacturer?.Id ?? 0;
        UpdateFromIds(m.Id, titleId, manId, m.Description, m.Price, m.Stock);
    }

    // ---------- Mapping ----------
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
