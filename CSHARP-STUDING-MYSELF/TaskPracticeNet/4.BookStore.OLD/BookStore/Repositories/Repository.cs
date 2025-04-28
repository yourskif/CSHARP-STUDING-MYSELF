using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Repositories
{
    public abstract class Repository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public virtual async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
        public virtual async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public virtual async Task UpdateAsync(T entity) => _context.Set<T>().Update(entity);
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}