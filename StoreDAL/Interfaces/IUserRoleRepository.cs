// Path: console-online-store/StoreDAL/Interfaces/IUserRoleRepository.cs
using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for user role management.
    /// Provides CRUD operations and role lookup by name.
    /// Uses Unit of Work pattern - SaveChanges is called externally.
    /// </summary>
    public interface IUserRoleRepository
    {
        /// <summary>
        /// Gets a user role by its unique identifier.
        /// </summary>
        UserRole? GetById(int id);

        /// <summary>
        /// Gets a user role by name.
        /// </summary>
        UserRole? GetByName(string name);

        /// <summary>
        /// Gets all user roles.
        /// Required for UserRoleService operations.
        /// </summary>
        IEnumerable<UserRole> GetAll();

        /// <summary>
        /// Adds a new user role.
        /// </summary>
        void Add(UserRole role);

        /// <summary>
        /// Updates an existing user role.
        /// </summary>
        void Update(UserRole role);

        /// <summary>
        /// Deletes a user role by its identifier.
        /// </summary>
        void DeleteById(int id);
    }
}
