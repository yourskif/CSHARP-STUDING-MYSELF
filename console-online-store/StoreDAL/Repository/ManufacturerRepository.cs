// Path: console-online-store/StoreDAL/Repository/ManufacturerRepository.cs
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

        public ManufacturerRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Manufacturer> GetAll()
        {
            return this.context.Manufacturers
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToList();
        }

        public IEnumerable<Manufacturer> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Manufacturers
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public Manufacturer GetById(int id)
        {
            return this.context.Manufacturers.Find(id);
        }

        /// <summary>
        /// Adds a new manufacturer. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Add(Manufacturer entity)
        {
            this.context.Manufacturers.Add(entity);
        }

        /// <summary>
        /// Deletes a manufacturer. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Delete(Manufacturer entity)
        {
            this.context.Manufacturers.Remove(entity);
        }

        /// <summary>
        /// Deletes a manufacturer by ID. Changes are not persisted until SaveChanges is called.
        /// </summary>
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
        public void Update(Manufacturer entity)
        {
            this.context.Manufacturers.Update(entity);
        }
    }
}
