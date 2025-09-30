namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for managing product titles (catalog items without pricing).
/// Provides CRUD operations and business logic for product title entities.
/// </summary>
public sealed class ProductTitleService : ICrud
{
    private readonly StoreDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductTitleService"/> class.
    /// </summary>
    /// <param name="context">Database context for product title operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public ProductTitleService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all product titles from the database.
    /// </summary>
    /// <returns>Collection of all product title models.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.ProductTitles
            .Select(pt => new ProductTitleModel(
                pt.Id,
                pt.Title ?? string.Empty,
                pt.CategoryId,
                pt.ManufacturerId))
            .ToList();
    }

    /// <summary>
    /// Retrieves a product title by its unique identifier.
    /// </summary>
    /// <param name="id">Product title ID.</param>
    /// <returns>Product title model if found.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when product title not found.</exception>
    public AbstractModel GetById(int id)
    {
        var entity = this.context.ProductTitles.Find(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product title with ID {id} not found.");
        }

        return new ProductTitleModel(
            entity.Id,
            entity.Title ?? string.Empty,
            entity.CategoryId,
            entity.ManufacturerId);
    }

    /// <summary>
    /// Adds a new product title to the database.
    /// </summary>
    /// <param name="model">Product title model to add.</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid or title is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when referenced category does not exist.</exception>
    public void Add(AbstractModel model)
    {
        if (model is not ProductTitleModel titleModel)
        {
            throw new ArgumentException("Model must be of type ProductTitleModel.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(titleModel.Title))
        {
            throw new ArgumentException("Product title cannot be empty.", nameof(model));
        }

        // Validate that category exists
        var categoryExists = this.context.Categories.Any(c => c.Id == titleModel.CategoryId);
        if (!categoryExists)
        {
            throw new InvalidOperationException($"Category with ID {titleModel.CategoryId} not found.");
        }

        // Validate that manufacturer exists
        var manufacturerExists = this.context.Manufacturers.Any(m => m.Id == titleModel.ManufacturerId);
        if (!manufacturerExists)
        {
            throw new InvalidOperationException($"Manufacturer with ID {titleModel.ManufacturerId} not found.");
        }

        var entity = new ProductTitle
        {
            Title = titleModel.Title,
            CategoryId = titleModel.CategoryId,
            ManufacturerId = titleModel.ManufacturerId,
        };

        this.context.ProductTitles.Add(entity);
        this.context.SaveChanges();

        titleModel.Id = entity.Id;
    }

    /// <summary>
    /// Updates an existing product title in the database.
    /// </summary>
    /// <param name="model">Product title model with updated data.</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid or title is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when product title not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when referenced category does not exist.</exception>
    public void Update(AbstractModel model)
    {
        if (model is not ProductTitleModel titleModel)
        {
            throw new ArgumentException("Model must be of type ProductTitleModel.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(titleModel.Title))
        {
            throw new ArgumentException("Product title cannot be empty.", nameof(model));
        }

        var entity = this.context.ProductTitles.Find(titleModel.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product title with ID {titleModel.Id} not found.");
        }

        // Validate that category exists
        var categoryExists = this.context.Categories.Any(c => c.Id == titleModel.CategoryId);
        if (!categoryExists)
        {
            throw new InvalidOperationException($"Category with ID {titleModel.CategoryId} not found.");
        }

        // Validate that manufacturer exists
        var manufacturerExists = this.context.Manufacturers.Any(m => m.Id == titleModel.ManufacturerId);
        if (!manufacturerExists)
        {
            throw new InvalidOperationException($"Manufacturer with ID {titleModel.ManufacturerId} not found.");
        }

        entity.Title = titleModel.Title;
        entity.CategoryId = titleModel.CategoryId;
        entity.ManufacturerId = titleModel.ManufacturerId;
        this.context.SaveChanges();
    }

    /// <summary>
    /// Deletes a product title from the database by its ID.
    /// </summary>
    /// <param name="modelId">ID of the product title to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when product title not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when product title has associated products.</exception>
    public void Delete(int modelId)
    {
        var entity = this.context.ProductTitles.Find(modelId);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product title with ID {modelId} not found.");
        }

        var hasProducts = this.context.Products.Any(p => p.ProductTitleId == modelId);
        if (hasProducts)
        {
            throw new InvalidOperationException(
                $"Cannot delete product title '{entity.Title}' because it has associated products.");
        }

        this.context.ProductTitles.Remove(entity);
        this.context.SaveChanges();
    }
}
