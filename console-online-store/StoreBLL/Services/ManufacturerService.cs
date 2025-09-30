namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for managing product manufacturers.
/// Provides CRUD operations and business logic for manufacturer entities.
/// </summary>
public sealed class ManufacturerService : ICrud
{
    private readonly StoreDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManufacturerService"/> class.
    /// </summary>
    /// <param name="context">Database context for manufacturer operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public ManufacturerService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all manufacturers from the database.
    /// </summary>
    /// <returns>Collection of all manufacturer models.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.Manufacturers
            .Select(m => new ManufacturerModel(m.Id, m.Name ?? string.Empty))
            .ToList();
    }

    /// <summary>
    /// Retrieves a manufacturer by its unique identifier.
    /// </summary>
    /// <param name="id">Manufacturer ID.</param>
    /// <returns>Manufacturer model if found.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when manufacturer not found.</exception>
    public AbstractModel GetById(int id)
    {
        var entity = this.context.Manufacturers.Find(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Manufacturer with ID {id} not found.");
        }

        return new ManufacturerModel(entity.Id, entity.Name ?? string.Empty);
    }

    /// <summary>
    /// Adds a new manufacturer to the database.
    /// </summary>
    /// <param name="model">Manufacturer model to add.</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid or name is empty.</exception>
    public void Add(AbstractModel model)
    {
        if (model is not ManufacturerModel manufacturerModel)
        {
            throw new ArgumentException("Model must be of type ManufacturerModel.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(manufacturerModel.Name))
        {
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
        }

        var entity = new Manufacturer
        {
            Name = manufacturerModel.Name,
        };

        this.context.Manufacturers.Add(entity);
        this.context.SaveChanges();

        manufacturerModel.Id = entity.Id;
    }

    /// <summary>
    /// Updates an existing manufacturer in the database.
    /// </summary>
    /// <param name="model">Manufacturer model with updated data.</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid or name is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when manufacturer not found.</exception>
    public void Update(AbstractModel model)
    {
        if (model is not ManufacturerModel manufacturerModel)
        {
            throw new ArgumentException("Model must be of type ManufacturerModel.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(manufacturerModel.Name))
        {
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
        }

        var entity = this.context.Manufacturers.Find(manufacturerModel.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Manufacturer with ID {manufacturerModel.Id} not found.");
        }

        entity.Name = manufacturerModel.Name;
        this.context.SaveChanges();
    }

    /// <summary>
    /// Deletes a manufacturer from the database by its ID.
    /// </summary>
    /// <param name="modelId">ID of the manufacturer to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown when manufacturer not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when manufacturer has associated products.</exception>
    public void Delete(int modelId)
    {
        var entity = this.context.Manufacturers.Find(modelId);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Manufacturer with ID {modelId} not found.");
        }

        var hasProducts = this.context.Products.Any(p => p.ManufacturerId == modelId);
        if (hasProducts)
        {
            throw new InvalidOperationException(
                $"Cannot delete manufacturer '{entity.Name}' because it has associated products.");
        }

        this.context.Manufacturers.Remove(entity);
        this.context.SaveChanges();
    }
}
