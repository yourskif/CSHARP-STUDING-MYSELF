// Path: console-online-store/StoreDAL/Repository/UserRoleRepository.cs
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository implementation for user role management.
    /// Uses Unit of Work pattern - does not call SaveChanges internally.
    /// </summary>
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for user role operations.</param>
        public UserRoleRepository(StoreDbContext context) => this.context = context;

        /// <summary>
        /// Gets a user role by its unique identifier.
        /// </summary>
        public UserRole? GetById(int id) =>
            this.context.UserRoles.FirstOrDefault(r => r.Id == id);

        /// <summary>
        /// Gets a user role by name using case-sensitive comparison.
        /// </summary>
        public UserRole? GetByName(string name) =>
            this.context.UserRoles.FirstOrDefault(r => r.RoleName == name);

        /// <summary>
        /// Gets all user roles from the database.
        /// </summary>
        public IEnumerable<UserRole> GetAll() =>
            this.context.UserRoles.AsNoTracking().ToList();

        /// <summary>
        /// Adds a new user role. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Add(UserRole role) =>
            this.context.UserRoles.Add(role);

        /// <summary>
        /// Updates an existing user role. Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void Update(UserRole role) =>
            this.context.UserRoles.Update(role);

        /// <summary>
        /// Deletes a user role by its identifier.
        /// Idempotent operation - no error if role doesn't exist.
        /// Changes are not persisted until SaveChanges is called.
        /// </summary>
        public void DeleteById(int id)
        {
            var entity = this.context.UserRoles.FirstOrDefault(r => r.Id == id);
            if (entity != null)
            {
                this.context.UserRoles.Remove(entity);
            }
        }
    }
}
