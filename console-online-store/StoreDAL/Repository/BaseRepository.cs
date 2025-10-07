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
            if (pageNumber <= 0) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (rowCount <= 0) throw new ArgumentOutOfRangeException(nameof(rowCount));

            return this.set.AsNoTracking()
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public virtual T GetById(int id)
        {
            return this.set.Find(id);
        }

        public virtual void Add(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Add(entity);
            this.context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Remove(entity);
            this.context.SaveChanges();
        }

        public virtual void DeleteById(int id)
        {
            var entity = this.set.Find(id);
            if (entity != null)
            {
                this.set.Remove(entity);
                this.context.SaveChanges();
            }
        }

        public virtual void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Update(entity);
            this.context.SaveChanges();
        }
    }
}
