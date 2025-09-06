// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Repository\ProductRepository.cs
namespace StoreDAL.Repository;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

/// <summary>
/// Concrete product repository. Includes Title->Category and Manufacturer for read operations.
/// Also provides Add/Update/Delete for administrative tasks.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext context;

    public ProductRepository(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<Product> GetAllWithIncludes()
    {
        return this.context.Products
            .Include(p => p.Title).ThenInclude(t => t.Category)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .ToList();
    }

    public IEnumerable<Product> GetByCategoryId(int categoryId)
    {
        // Safe filter when Title might be null (fixes CS8602).
        return this.context.Products
            .Include(p => p.Title)
            .Where(p => p.Title != null && p.Title.CategoryId == categoryId)
            .AsNoTracking()
            .ToList();
    }

    public Product? GetByIdWithIncludes(int id)
    {
        return this.context.Products
            .Include(p => p.Title).ThenInclude(t => t.Category)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);
    }

    // Convenience (used in some older code)
    public IEnumerable<Product> GetAll() =>
        this.context.Products.AsNoTracking().ToList();

    public Product? GetById(int id) =>
        this.context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);

    // ---------------- CUD ----------------

    public Product Add(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        this.context.Products.Add(entity);
        this.context.SaveChanges();
        return entity;
    }

    public void Update(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var existing = this.context.Products.FirstOrDefault(p => p.Id == entity.Id);
        if (existing is null)
        {
            throw new KeyNotFoundException($"Product #{entity.Id} not found.");
        }

        // Update simple fields
        existing.TitleId = entity.TitleId;
        existing.ManufacturerId = entity.ManufacturerId;
        existing.Description = entity.Description;
        existing.UnitPrice = entity.UnitPrice;
        existing.Stock = entity.Stock;

        this.context.SaveChanges();
    }

    public bool Delete(int id)
    {
        var existing = this.context.Products.FirstOrDefault(p => p.Id == id);
        if (existing is null)
        {
            return false;
        }

        this.context.Products.Remove(existing);
        this.context.SaveChanges();
        return true;
    }
}
