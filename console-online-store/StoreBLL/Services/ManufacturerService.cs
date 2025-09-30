namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for managing manufacturers.
/// </summary>
public class ManufacturerService : ICrud
{
    private readonly StoreDbContext context;

    public ManufacturerService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all manufacturers.
    /// </summary>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.Manufacturers
            .Select(m => new ManufacturerModel(m.Id, m.Name ?? string.Empty))
            .ToList();
    }

    /// <summary>
    /// Gets a manufacturer by id.
    /// </summary>
    public AbstractModel GetById(int id)
    {
        var manufacturer = this.context.Manufacturers.Find(id);
        if (manufacturer == null)
        {
            throw new KeyNotFoundException($"Manufacturer with id {id} not found.");
        }

        return new ManufacturerModel(manufacturer.Id, manufacturer.Name ?? string.Empty);
    }

    /// <summary>
    /// Adds a new manufacturer.
    /// </summary>
    public void Add(AbstractModel model)
    {
        if (model is not ManufacturerModel m)
        {
            throw new ArgumentException("Expected ManufacturerModel", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(m.Name))
        {
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
        }

        // Check for duplicate name
        if (this.context.Manufacturers.Any(man => man.Name == m.Name))
        {
            throw new InvalidOperationException($"Manufacturer with name '{m.Name}' already exists.");
        }

        var entity = new Manufacturer
        {
            Name = m.Name,
        };

        this.context.Manufacturers.Add(entity);
        this.context.SaveChanges();
    }

    /// <summary>
    /// Updates an existing manufacturer.
    /// </summary>
    public void Update(AbstractModel model)
    {
        if (model is not ManufacturerModel m)
        {
            throw new ArgumentException("Expected ManufacturerModel", nameof(model));
        }

        var entity = this.context.Manufacturers.Find(m.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Manufacturer with id {m.Id} not found.");
        }

        if (string.IsNullOrWhiteSpace(m.Name))
        {
            throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
        }

        // Check for duplicate name (excluding current entity)
        if (this.context.Manufacturers.Any(man => man.Name == m.Name && man.Id != m.Id))
        {
            throw new InvalidOperationException($"Manufacturer with name '{m.Name}' already exists.");
        }

        entity.Name = m.Name;
        this.context.SaveChanges();
    }

    /// <summary>
    /// Deletes a manufacturer by id.
    /// </summary>
    public void Delete(int modelId)
    {
        var entity = this.context.Manufacturers.Find(modelId);
        if (entity == null)
        {
            return; // Idempotent delete
        }

        // Check if any products reference this manufacturer
        var productsCount = this.context.Products.Count(p => p.ManufacturerId == modelId);
        if (productsCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete Manufacturer with id {modelId} because it has {productsCount} product(s) referencing it.");
        }

        this.context.Manufacturers.Remove(entity);
        this.context.SaveChanges();
    }
}
