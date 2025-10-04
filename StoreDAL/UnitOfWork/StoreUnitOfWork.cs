// Path: console-online-store/StoreDAL/UnitOfWork/UnitOfWork.cs
using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

using StoreDAL.Data;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

namespace StoreDAL.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementation that coordinates multiple repositories and manages transactions.
    /// Ensures all changes across repositories are saved atomically.
    /// </summary>
    public sealed class StoreUnitOfWork : IStoreUnitOfWork
    {
        private readonly StoreDbContext context;
        private IDbContextTransaction? transaction;

        private IProductRepository? products;
        private IUserRepository? users;
        private ICustomerOrderRepository? orders;
        private IOrderDetailRepository? orderDetails;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public StoreUnitOfWork(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public IProductRepository Products =>
            this.products ??= new ProductRepository(this.context);

        /// <inheritdoc/>
        public IUserRepository Users =>
            this.users ??= new UserRepository(this.context);

        /// <inheritdoc/>
        public ICustomerOrderRepository Orders =>
            this.orders ??= new CustomerOrderRepository(this.context);

        /// <inheritdoc/>
        public IOrderDetailRepository OrderDetails =>
            this.orderDetails ??= new OrderDetailRepository(this.context);

        /// <inheritdoc/>
        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        /// <inheritdoc/>
        public Task<int> SaveChangesAsync()
        {
            return this.context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void BeginTransaction()
        {
            if (this.transaction != null)
            {
                throw new InvalidOperationException("Transaction already started.");
            }

            this.transaction = this.context.Database.BeginTransaction();
        }

        /// <inheritdoc/>
        public void Commit()
        {
            if (this.transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            try
            {
                this.context.SaveChanges();
                this.transaction.Commit();
            }
            finally
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
        }

        /// <inheritdoc/>
        public void Rollback()
        {
            if (this.transaction == null)
            {
                throw new InvalidOperationException("No active transaction to rollback.");
            }

            try
            {
                this.transaction.Rollback();
            }
            finally
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.transaction?.Dispose();
                this.context.Dispose();
                this.disposed = true;
            }
        }
    }
}
