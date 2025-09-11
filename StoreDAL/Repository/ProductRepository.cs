// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Repository\ProductRepository.cs
namespace StoreDAL.Repository;

using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

/// <summary>
/// Concrete product repository. Includes Title->Category and Manufacturer for read operations.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext context;

    public ProductRepository(StoreDbContext context)
    {
        this.context = context;
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

    // Convenience (used in older code)
    public IEnumerable<Product> GetAll() =>
        this.context.Products.AsNoTracking().ToList();

    public Product? GetById(int id) =>
        this.context.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
}
