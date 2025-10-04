// Path: console-online-store/StoreDAL/UnitOfWork/IUnitOfWork.cs
using System;
using System.Threading.Tasks;

using StoreDAL.Interfaces;

namespace StoreDAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work pattern interface for coordinating repository operations.
    /// Provides centralized transaction management and ensures all changes are saved atomically.
    /// </summary>
    public interface IStoreUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the product repository.
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Gets the user repository.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets the customer order repository.
        /// </summary>
        ICustomerOrderRepository Orders { get; }

        /// <summary>
        /// Gets the order detail repository.
        /// </summary>
        IOrderDetailRepository OrderDetails { get; }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        /// <returns>Number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all pending changes to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        void Rollback();
    }
}
