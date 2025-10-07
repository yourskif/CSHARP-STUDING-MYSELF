namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for managing product titles (catalog items without SKU/price).
/// Product titles represent shared product names that can be associated with multiple SKU variants.
/// Each ProductTitle contains: Id, Title (display name), and CategoryId (foreign key to Category).
/// </summary>
public sealed class ProductTitleService : ICrud
{
    private readonly StoreDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductTitleService"/> class.
    /// </summary>
    /// <param name="context">EF Core database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public ProductTitleService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all product titles from the database.
    /// </summary>
    /// <returns>Collection of product title models ordered by Id.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.ProductTitles
            .AsNoTracking()
            .OrderBy(pt => pt.Id)
            .Select(pt => new ProductTitleModel(pt.Id, pt.Title ?? string.Empty, pt.CategoryId))
            .ToList();
    }

    /// <summary>
    /// Gets a product title by its unique identifier.
    /// </summary>
    /// <param name="id">Product title identifier.</param>
    /// <returns>Product title model.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product title with specified id is not found.</exception>
    public AbstractModel GetById(int id)
    {
        var pt = this.context.ProductTitles
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == id);

        if (pt == null)
        {
            throw new KeyNotFoundException($"ProductTitle with id {id} not found.");
        }

        return new ProductTitleModel(pt.Id, pt.Title ?? string.Empty, pt.CategoryId);
    }

    /// <summary>
    /// Adds a new product title to the database.
    /// Validates that the referenced category exists before insertion.
    /// </summary>
    /// <param name="model">Product title model containing data to add.</param>
    /// <exception cref="ArgumentException">Thrown when model is not ProductTitleModel or title is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when referenced category does not exist.</exception>
    public void Add(AbstractModel model)
    {
        if (model is not ProductTitleModel m)
        {
            throw new ArgumentException("Expected ProductTitleModel", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(m.Title))
        {
            throw new ArgumentException("Product title cannot be empty.", nameof(model));
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

        // Update model Id with generated value
        m.Id = entity.Id;
    }

    /// <summary>
    /// Updates an existing product title in the database.
    /// Validates that the referenced category exists before updating.
    /// </summary>
    /// <param name="model">Product title model with updated data.</param>
    /// <exception cref="ArgumentException">Thrown when model is not ProductTitleModel or title is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when product title with specified id is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when referenced category does not exist.</exception>
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

        if (string.IsNullOrWhiteSpace(m.Title))
        {
            throw new ArgumentException("Product title cannot be empty.", nameof(model));
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
    /// Deletes a product title from the database.
    /// Ensures referential integrity by preventing deletion of titles referenced by products.
    /// </summary>
    /// <param name="modelId">Product title identifier to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown when product title has products referencing it.</exception>
    public void Delete(int modelId)
    {
        var entity = this.context.ProductTitles.Find(modelId);
        if (entity == null)
        {
            return; // Idempotent delete - no error if already deleted
        }

        // Check if any products reference this title
        var productsCount = this.context.Products
            .Count(p => p.ProductTitleId == modelId);

        if (productsCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete product title '{entity.Title}' (ID: {modelId}) because it has {productsCount} product(s) referencing it. Remove or reassign products first.");
        }

        this.context.ProductTitles.Remove(entity);
        this.context.SaveChanges();
    }
}
