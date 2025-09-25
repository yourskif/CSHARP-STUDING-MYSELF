// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Interfaces\IUserRepository.cs
using System.Collections.Generic;
using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// User repository contract. Extends generic repository with user-specific operations.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Finds a user by login or returns null if not found.
        /// </summary>
        User? FindByLogin(string login);

        /// <summary>
        /// Returns true if the user has at least one order.
        /// Prevents physical deletion when business rules require keeping data.
        /// </summary>
        bool HasOrders(int userId);

        /// <summary>
        /// Persists changes.
        /// </summary>
        void SaveChanges();
    }
}
