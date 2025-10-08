// Path: console-online-store/StoreBLL/Services/ManufacturerService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using StoreBLL.Models;

    using StoreDAL.Entities;
    using StoreDAL.UnitOfWork;

    /// <summary>
    /// Service for managing manufacturers in the business logic layer.
    /// Provides CRUD operations with validation and referential integrity checks.
    /// </summary>
    public sealed class ManufacturerService
    {
        private readonly IStoreUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturerService"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="unitOfWork"/> is <see langword="null"/>.</exception>
        public ManufacturerService(IStoreUnitOfWork unitOfWork)
        {
            ArgumentNullException.ThrowIfNull(unitOfWork);
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns all manufacturers ordered by identifier.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all manufacturers as <see cref="ManufacturerModel"/> instances.</returns>
        public IEnumerable<ManufacturerModel> GetAll()
        {
            return this.unitOfWork.Context.Manufacturers
                .AsNoTracking()
                .OrderBy(m => m.Id)
                .Select(m => new ManufacturerModel { Id = m.Id, Name = m.Name ?? string.Empty })
                .ToList();
        }

        /// <summary>
        /// Returns a manufacturer by its unique identifier.
        /// </summary>
        /// <param name="id">Manufacturer identifier.</param>
        /// <returns>
        /// <see cref="ManufacturerModel"/> instance when found; otherwise, <see langword="null"/>.
        /// </returns>
        public ManufacturerModel? GetById(int id)
        {
            var entity = this.unitOfWork.Context.Manufacturers
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);

            return entity == null ? null : new ManufacturerModel { Id = entity.Id, Name = entity.Name ?? string.Empty };
        }

        /// <summary>
        /// Adds a new manufacturer to the database.
        /// Validates that the manufacturer name is not empty and does not already exist.
        /// </summary>
        /// <param name="model">Manufacturer model to add.</param>
        /// <returns>Created manufacturer model with assigned identifier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when manufacturer name is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a manufacturer with the same name already exists.</exception>
        public ManufacturerModel Add(ManufacturerModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
            }

            // Check for duplicate name (case-insensitive)
            if (this.unitOfWork.Context.Manufacturers.Any(m => m.Name != null && m.Name.ToLower() == model.Name.ToLower()))
            {
                throw new InvalidOperationException($"Manufacturer with name '{model.Name}' already exists.");
            }

            var entity = new Manufacturer
            {
                Name = model.Name,
            };

            this.unitOfWork.Context.Manufacturers.Add(entity);
            this.unitOfWork.SaveChanges();

            return new ManufacturerModel { Id = entity.Id, Name = entity.Name ?? string.Empty };
        }

        /// <summary>
        /// Updates an existing manufacturer in the database.
        /// Validates that the new name is not empty and does not conflict with existing manufacturers.
        /// </summary>
        /// <param name="model">Manufacturer model with the updated data.</param>
        /// <returns><see langword="true"/> if the manufacturer was updated; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when manufacturer name is null or whitespace.</exception>
        /// <exception cref="InvalidOperationException">Thrown when another manufacturer with the same name already exists.</exception>
        public bool Update(ManufacturerModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var entity = this.unitOfWork.Context.Manufacturers.FirstOrDefault(m => m.Id == model.Id);
            if (entity is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentException("Manufacturer name cannot be empty.", nameof(model));
            }

            // Check for duplicate name (case-insensitive, excluding current entity)
            if (this.unitOfWork.Context.Manufacturers.Any(m => m.Id != model.Id && m.Name != null && m.Name.ToLower() == model.Name.ToLower()))
            {
                throw new InvalidOperationException($"Manufacturer with name '{model.Name}' already exists.");
            }

            entity.Name = model.Name;
            this.unitOfWork.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes a manufacturer by its identifier.
        /// Ensures referential integrity by preventing deletion of manufacturers referenced by products.
        /// </summary>
        /// <param name="id">Manufacturer identifier.</param>
        /// <returns><see langword="true"/> if the manufacturer was deleted; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when manufacturer has products referencing it.</exception>
        public bool Delete(int id)
        {
            var entity = this.unitOfWork.Context.Manufacturers.Find(id);
            if (entity is null)
            {
                return false;
            }

            // Check if any products reference this manufacturer
            var productsCount = this.unitOfWork.Context.Products.Count(p => p.ManufacturerId == id);

            if (productsCount > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot delete manufacturer '{entity.Name}' (ID: {id}) because it has {productsCount} product(s) referencing it. Remove or reassign products first.");
            }

            this.unitOfWork.Context.Manufacturers.Remove(entity);
            this.unitOfWork.SaveChanges();
            return true;
        }
    }
}
