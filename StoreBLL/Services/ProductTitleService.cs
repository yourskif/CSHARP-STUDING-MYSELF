namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for managing product titles (catalog items without SKU/price).
/// ProductTitle contains: Id, Title, CategoryId.
/// </summary>
public class ProductTitleService : ICrud
{
    private readonly StoreDbContext context;

    public ProductTitleService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all product titles.
    /// </summary>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.ProductTitles
            .Select(pt => new ProductTitleModel(pt.Id, pt.Title ?? string.Empty, pt.CategoryId))
            .ToList();
    }

    /// <summary>
    /// Gets a product title by id.
    /// </summary>
    public AbstractModel GetById(int id)
    {
        var pt = this.context.ProductTitles.Find(id);
        if (pt == null)
        {
            throw new KeyNotFoundException($"ProductTitle with id {id} not found.");
        }

        return new ProductTitleModel(pt.Id, pt.Title ?? string.Empty, pt.CategoryId);
    }

    /// <summary>
    /// Adds a new product title.
    /// </summary>
    public void Add(AbstractModel model)
    {
        if (model is not ProductTitleModel m)
        {
            throw new ArgumentException("Expected ProductTitleModel", nameof(model));
        }

        // Validate that category exists
        if (!this.context.Categories.Any(c => c.Id == m.CategoryId))
        {
            throw new InvalidOperationException($"Category with id {m.CategoryId} not found.");
        }

        var entity = new ProductTitle
        {
            Title = m.Title,
            CategoryId = m.CategoryId,
        };

        this.context.ProductTitles.Add(entity);
        this.context.SaveChanges();
    }

    /// <summary>
    /// Updates an existing product title.
    /// </summary>
    public void Update(AbstractModel model)
    {
        if (model is not ProductTitleModel m)
        {
            throw new ArgumentException("Expected ProductTitleModel", nameof(model));
        }

        var entity = this.context.ProductTitles.Find(m.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"ProductTitle with id {m.Id} not found.");
        }

        // Validate that category exists
        if (!this.context.Categories.Any(c => c.Id == m.CategoryId))
        {
            throw new InvalidOperationException($"Category with id {m.CategoryId} not found.");
        }

        entity.Title = m.Title;
        entity.CategoryId = m.CategoryId;

        this.context.SaveChanges();
    }

    /// <summary>
    /// Deletes a product title by id.
    /// </summary>
    public void Delete(int modelId)
    {
        var entity = this.context.ProductTitles.Find(modelId);
        if (entity == null)
        {
            return; // Idempotent delete
        }

        // Check if any products reference this title
        var productsCount = this.context.Products.Count(p => p.ProductTitleId == modelId);
        if (productsCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete ProductTitle with id {modelId} because it has {productsCount} product(s) referencing it.");
        }

        this.context.ProductTitles.Remove(entity);
        this.context.SaveChanges();
    }
}
