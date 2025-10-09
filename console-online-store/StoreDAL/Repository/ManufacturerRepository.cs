// Path: console-online-store/StoreDAL/Repository/ManufacturerRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository implementation for manufacturer entities.
    /// Uses Unit of Work pattern - does not call SaveChanges internally.
    /// </summary>
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturerRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public ManufacturerRepository(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets all manufacturers ordered by name.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all manufacturers.</returns>
        public IEnumerable<Manufacturer> GetAll()
        {
            return this.context.Manufacturers
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToList();
        }

        /// <summary>
        /// Gets paginated manufacturers ordered by name.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Paginated collection of manufacturers.</returns>
        public IEnumerable<Manufacturer> GetAll(int pageNumber, int rowCount)
        {
            var skip = pageNumber <= 1 ? 0 : (pageNumber - 1) * rowCount;

            return this.context.Manufacturers
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Skip(skip)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Gets a manufacturer by its identifier.
        /// </summary>
        /// <param name="id">Manufacturer identifier.</param>
        /// <returns>Manufacturer entity.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when manufacturer with specified id is not found.</exception>
        public Manufacturer GetById(int id)
        {
            return this.context.Manufacturers.Find(id)
                ?? throw new KeyNotFoundException($"Manufacturer with id {id} not found.");
        }

        /// <summary>
        /// Adds a new manufacturer. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Manufacturer entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(Manufacturer entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Manufacturers.Add(entity);
        }

        /// <summary>
        /// Deletes a manufacturer. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Manufacturer entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(Manufacturer entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Manufacturers.Remove(entity);
        }

        /// <summary>
        /// Deletes a manufacturer by ID. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="id">Manufacturer identifier to delete.</param>
        public void DeleteById(int id)
        {
            var entity = this.context.Manufacturers.Find(id);
            if (entity != null)
            {
                this.context.Manufacturers.Remove(entity);
            }
        }

        /// <summary>
        /// Updates a manufacturer. Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">Manufacturer entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(Manufacturer entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.Manufacturers.Update(entity);
        }
    }
}
