// Path: console-online-store/StoreDAL/UnitOfWork/IStoreUnitOfWork.cs
using System;
using System.Threading.Tasks;

using StoreDAL.Data;
using StoreDAL.Interfaces;

namespace StoreDAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work pattern contract that coordinates multiple repositories and manages transactions.
    /// Provides centralized access to repositories and database context.
    /// </summary>
    public interface IStoreUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the underlying database context.
        /// Provides direct access for complex queries and EF Core features not exposed through repositories.
        /// </summary>
        StoreDbContext Context { get; }

        /// <summary>
        /// Gets the product repository for managing product entities.
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Gets the user repository for managing user entities.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets the customer order repository for managing order entities.
        /// </summary>
        ICustomerOrderRepository Orders { get; }

        /// <summary>
        /// Gets the order detail repository for managing order detail entities.
        /// </summary>
        IOrderDetailRepository OrderDetails { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves all changes made in this unit of work to the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// The task result contains the number of state entries written to the database.
        /// </returns>
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
