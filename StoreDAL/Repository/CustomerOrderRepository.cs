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
    /// Repository for managing customer order entities in the database.
    /// Implements CRUD operations for customer orders.
    /// Uses Unit of Work pattern - does NOT call SaveChanges internally.
    /// </summary>
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing order data.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public CustomerOrderRepository(StoreDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new customer order to the database context.
        /// Note: Changes are not persisted until SaveChanges is called on UnitOfWork.
        /// </summary>
        /// <param name="entity">Customer order entity to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Add(CustomerOrder entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.CustomerOrders.Add(entity);

            // SaveChanges is called by UnitOfWork - NOT here
        }

        /// <summary>
        /// Deletes an existing customer order from the database context.
        /// Note: Changes are not persisted until SaveChanges is called on UnitOfWork.
        /// </summary>
        /// <param name="entity">Customer order entity to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Delete(CustomerOrder entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            this.context.CustomerOrders.Remove(entity);

            // SaveChanges is called by UnitOfWork - NOT here
        }

        /// <summary>
        /// Deletes a customer order by its identifier.
        /// If the order does not exist, no action is taken.
        /// Note: Changes are not persisted until SaveChanges is called on UnitOfWork.
        /// </summary>
        /// <param name="id">Unique identifier of the order to delete.</param>
        public void DeleteById(int id)
        {
            var order = this.context.CustomerOrders.Find(id);
            if (order != null)
            {
                this.context.CustomerOrders.Remove(order);

                // SaveChanges is called by UnitOfWork - NOT here
            }
        }

        /// <summary>
        /// Retrieves all customer orders from the database.
        /// Uses AsNoTracking for read-only operations to improve performance.
        /// </summary>
        /// <returns>Collection of all customer orders.</returns>
        public IEnumerable<CustomerOrder> GetAll()
        {
            return this.context.CustomerOrders
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .ToList();
        }

        /// <summary>
        /// Retrieves a paginated list of customer orders.
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve (1-based).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Collection of customer orders for the specified page.</returns>
        public IEnumerable<CustomerOrder> GetAll(int pageNumber, int rowCount)
        {
            var skip = pageNumber <= 1 ? 0 : (pageNumber - 1) * rowCount;

            return this.context.CustomerOrders
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .Skip(skip)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Retrieves a customer order by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the order.</param>
        /// <returns>Customer order entity.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when order with specified id is not found.</exception>
        public CustomerOrder GetById(int id)
        {
            return this.context.CustomerOrders.Find(id)
                ?? throw new KeyNotFoundException($"Order with id {id} not found.");
        }

        /// <summary>
        /// Updates an existing customer order in the database context.
        /// Note: Changes are not persisted until SaveChanges is called on UnitOfWork.
        /// </summary>
        /// <param name="entity">Customer order entity with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        public void Update(CustomerOrder entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.context.CustomerOrders.Attach(entity);
                entry = this.context.Entry(entity);
            }

            entry.State = EntityState.Modified;

            // SaveChanges is called by UnitOfWork - NOT here
        }
    }
}
