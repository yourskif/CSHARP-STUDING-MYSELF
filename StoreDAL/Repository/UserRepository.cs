// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Repository\UserRepository.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// EF Core-backed user repository.
    /// Implements generic CRUD and user-specific helpers.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContext db;

        public UserRepository(StoreDbContext db)
        {
            this.db = db;
        }

        // ===== IUserRepository specific =====
        public User? FindByLogin(string login)
        {
            return this.db.Users.FirstOrDefault(u => u.Login == login);
        }

        public bool HasOrders(int userId)
        {
            return this.db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.Orders.Count)
                .FirstOrDefault() > 0;
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        // ===== IRepository<User> implementation =====
        public IEnumerable<User> GetAll()
        {
            return this.db.Users.AsNoTracking().AsEnumerable();
        }

        public IEnumerable<User> GetAll(int pageNumber, int rowCount)
        {
            var skip = pageNumber <= 1 ? 0 : (pageNumber - 1) * rowCount;

            return this.db.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .Skip(skip)
                .Take(rowCount)
                .AsEnumerable();
        }

        public User GetById(int id)
        {
            // Non-null by contract: First(...) throws if not found.
            return this.db.Users
                .Include(u => u.Orders)
                .First(u => u.Id == id);
        }

        public void Add(User entity)
        {
            this.db.Users.Add(entity);
        }

        public void Delete(User entity)
        {
            this.db.Users.Remove(entity);
        }

        public void DeleteById(int id)
        {
            var entity = this.db.Users.FirstOrDefault(u => u.Id == id);
            if (entity != null)
            {
                this.db.Users.Remove(entity);
            }
        }

        public void Update(User entity)
        {
            var entry = this.db.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.db.Users.Attach(entity);
                entry = this.db.Entry(entity);
            }

            entry.State = EntityState.Modified;
        }
    }
}
