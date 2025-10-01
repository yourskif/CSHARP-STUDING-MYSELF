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
/// Service for managing manufacturers with full CRUD operations.
/// Provides business logic layer for manufacturer entities including validation and integrity checks.
/// </summary>
public sealed class ManufacturerService : ICrud
{
    private readonly StoreDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManufacturerService"/> class.
    /// </summary>
    /// <param name="context">EF Core database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public ManufacturerService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all manufacturers from the database.
    /// </summary>
    /// <returns>Collection of manufacturer models.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.context.Manufacturers
            .AsNoTracking()
            .Select(m => new ManufacturerModel(m.Id, m.Name ?? string.Empty))
            .ToList();
    }

    /// <summary>
    /// Gets a manufacturer by its unique identifier.
    /// </summary>
    /// <param name="id">Manufacturer identifier.</param>
    /// <returns>Manufacturer model.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when manufacturer with specified id is not found.</exception>
    public AbstractModel GetById(int id)
    {
        var manufacturer = this.context.Manufacturers
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == id);

        if (manufacturer == null)
        {
            throw new KeyNotFoundException($"Manufacturer with id {id} not found.");
        }

        return new ManufacturerModel(manufacturer.Id, manufacturer.Name ?? string.Empty);
    }

    /// <summary>
    /// Adds a new manufacturer to the database.
    /// </summary>
    /// <param name="model">Manufacturer model containing data to add.</param>
    /// <exception cref="ArgumentException">Thrown when model is not ManufacturerModel or name is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when manufacturer with the same name already exists.</exception>
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

        // Check for duplicate name (case-insensitive)
        bool exists = this.context.Manufacturers
            .Any(man => man.Name != null && man.Name.ToLower() == m.Name.ToLower());

        if (exists)
        {
            throw new InvalidOperationException($"Manufacturer with name '{m.Name}' already exists.");
        }

        var entity = new Manufacturer
        {
            Name = m.Name,
        };

        this.context.Manufacturers.Add(entity);
        this.context.SaveChanges();

        // Update model Id with generated value
        m.Id = entity.Id;
    }

    /// <summary>
    /// Updates an existing manufacturer in the database.
    /// </summary>
    /// <param name="model">Manufacturer model with updated data.</param>
    /// <exception cref="ArgumentException">Thrown when model is not ManufacturerModel or name is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when manufacturer with specified id is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when another manufacturer with the same name already exists.</exception>
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

        // Check for duplicate name (case-insensitive, excluding current entity)
        bool exists = this.context.Manufacturers
            .Any(man => man.Id != m.Id &&
                        man.Name != null &&
                        man.Name.ToLower() == m.Name.ToLower());

        if (exists)
        {
            throw new InvalidOperationException($"Manufacturer with name '{m.Name}' already exists.");
        }

        entity.Name = m.Name;
        this.context.SaveChanges();
    }

    /// <summary>
    /// Deletes a manufacturer from the database.
    /// Ensures referential integrity by preventing deletion of manufacturers referenced by products.
    /// </summary>
    /// <param name="modelId">Manufacturer identifier to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown when manufacturer has products referencing it.</exception>
    public void Delete(int modelId)
    {
        var entity = this.context.Manufacturers.Find(modelId);
        if (entity == null)
        {
            return; // Idempotent delete - no error if already deleted
        }

        // Check if any products reference this manufacturer
        var productsCount = this.context.Products
            .Count(p => p.ManufacturerId == modelId);

        if (productsCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete manufacturer '{entity.Name}' (ID: {modelId}) because it has {productsCount} product(s) referencing it. Remove or reassign products first.");
        }

        this.context.Manufacturers.Remove(entity);
        this.context.SaveChanges();
    }
}
