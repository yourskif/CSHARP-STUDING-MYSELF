// StoreDAL/Repository/BaseRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// EF Core base repository providing generic CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class BaseRepository<T> : IRepository<T>
        where T : class
    {
        protected readonly StoreDbContext context;
        protected readonly DbSet<T> set;

        public BaseRepository(StoreDbContext context)
            : base()
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

        public virtual T? GetById(int id)
        {
            return this.set.Find(id);
        }

        public virtual int Create(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Add(entity);
            return this.context.SaveChanges();
        }

        public virtual bool Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.set.Update(entity);
            return this.context.SaveChanges() > 0;
        }

        public virtual bool Delete(int id)
        {
            var entity = this.set.Find(id);
            if (entity is null)
            {
                return false;
            }

            this.set.Remove(entity);
            return this.context.SaveChanges() > 0;
        }
    }
}
