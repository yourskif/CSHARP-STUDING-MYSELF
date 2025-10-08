// Path: console-online-store/StoreDAL/Repository/BaseRepository.cs
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
    /// EF Core base repository providing generic CRUD operations.
    /// Uses Unit of Work pattern - does not call SaveChanges internally.
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity.</typeparam>
    public class BaseRepository<T> : IRepository<T>
        where T : BaseEntity
    {
        protected readonly StoreDbContext context;
        protected readonly DbSet<T> set;

        public BaseRepository(StoreDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.context = context;
            this.set = context.Set<T>();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.set.AsNoTracking().ToList();
        }

        public virtual IEnumerable<T> GetAll(int pageNumber, int rowCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);

            return this.set.AsNoTracking()
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public virtual T GetById(int id)
        {
            return this.set.Find(id);
        }

        /// <summary>
        /// Adds entity to the context. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public virtual void Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Add(entity);
        }

        /// <summary>
        /// Removes entity from the context. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public virtual void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Remove(entity);
        }

        /// <summary>
        /// Removes entity by ID from the context. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public virtual void DeleteById(int id)
        {
            var entity = this.set.Find(id);
            if (entity != null)
            {
                this.set.Remove(entity);
            }
        }

        /// <summary>
        /// Updates entity in the context. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public virtual void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Update(entity);
        }
    }
}
