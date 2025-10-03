// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Repository\UserRepository.cs
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
    /// EF Core-backed user repository.
    /// Implements generic CRUD operations and user-specific queries for authentication and authorization.
    /// Supports password hashing, role management, and order history tracking.
    /// </summary>
    public sealed class UserRepository : IUserRepository
    {
        private readonly StoreDbContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="db">EF Core database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
        public UserRepository(StoreDbContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            this.db = db;
        }

        // ===== IUserRepository specific =====

        /// <summary>
        /// Finds a user by their unique login identifier.
        /// Used for authentication and duplicate login validation.
        /// </summary>
        /// <param name="login">User login (username/email).</param>
        /// <returns>User entity if found, null otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown when login is null or whitespace.</exception>
        public User? FindByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Login cannot be null or whitespace.", nameof(login));
            }

            return this.db.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == login);
        }

        /// <summary>
        /// Checks if a user has any orders associated with their account.
        /// Used to prevent deletion of users with order history for data integrity.
        /// </summary>
        /// <param name="userId">User identifier to check.</param>
        /// <returns>True if user has at least one order, false otherwise.</returns>
        public bool HasOrders(int userId)
        {
            return this.db.CustomerOrders
                .AsNoTracking()
                .Any(o => o.UserId == userId);
        }

        /// <summary>
        /// Persists all pending changes in the database context to the database.
        /// Should be called after Add, Update, or Delete operations to commit changes.
        /// </summary>
        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        // ===== IRepository<User> implementation =====

        /// <summary>
        /// Returns all users without navigation properties.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all users ordered by Id.</returns>
        public IEnumerable<User> GetAll()
        {
            return this.db.Users
                .AsNoTracking()
                .OrderBy(u => u.Id)
                .AsEnumerable();
        }

        /// <summary>
        /// Returns a paginated subset of users.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based index).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Paginated collection of users.</returns>
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

        /// <summary>
        /// Returns a single user by id with related Orders eagerly loaded.
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <returns>User entity with Orders navigation property populated.</returns>
        /// <exception cref="InvalidOperationException">Thrown when user with specified id is not found.</exception>
        public User GetById(int id)
        {
            // Non-null by contract: First(...) throws if not found
            return this.db.Users
                .Include(u => u.Orders)
                .First(u => u.Id == id);
        }

        /// <summary>
        /// Adds a new user to the database context.
        /// Note: Changes are not persisted until SaveChanges is called.
        /// Passwords should be hashed before calling this method.
        /// </summary>
        /// <param name="entity">User entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            this.db.Users.Add(entity);
        }

        /// <summary>
        /// Removes a user from the database context.
        /// Note: Changes are not persisted until SaveChanges is called.
        /// Consider checking HasOrders before deletion to preserve data integrity.
        /// </summary>
        /// <param name="entity">User entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            this.db.Users.Remove(entity);
        }

        /// <summary>
        /// Removes a user by id from the database context.
        /// Idempotent operation - no error if user doesn't exist.
        /// Note: Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="id">User identifier to delete.</param>
        public void DeleteById(int id)
        {
            var entity = this.db.Users.FirstOrDefault(u => u.Id == id);
            if (entity != null)
            {
                this.db.Users.Remove(entity);
            }
        }

        /// <summary>
        /// Marks an existing user entity as modified in the database context.
        /// Handles both attached and detached entities.
        /// Note: Changes are not persisted until SaveChanges is called.
        /// </summary>
        /// <param name="entity">User entity to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

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
