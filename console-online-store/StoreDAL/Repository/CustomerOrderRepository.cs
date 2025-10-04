using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// Repository for managing customer order entities in the database.
    /// Implements CRUD operations for customer orders.
    /// </summary>
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly StoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderRepository"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing order data.</param>
        public CustomerOrderRepository(StoreDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a new customer order to the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Customer order entity to add.</param>
        public void Add(CustomerOrder entity)
        {
            this.context.CustomerOrders.Add(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes an existing customer order from the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Customer order entity to delete.</param>
        public void Delete(CustomerOrder entity)
        {
            this.context.CustomerOrders.Remove(entity);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Deletes a customer order by its identifier.
        /// If the order does not exist, no action is taken.
        /// </summary>
        /// <param name="id">Unique identifier of the order to delete.</param>
        public void DeleteById(int id)
        {
            var order = this.context.CustomerOrders.Find(id);
            if (order != null)
            {
                this.context.CustomerOrders.Remove(order);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves all customer orders from the database.
        /// </summary>
        /// <returns>Collection of all customer orders.</returns>
        public IEnumerable<CustomerOrder> GetAll()
        {
            return this.context.CustomerOrders.ToList();
        }

        /// <summary>
        /// Retrieves a paginated list of customer orders.
        /// </summary>
        /// <param name="pageNumber">Page number to retrieve (1-based).</param>
        /// <param name="rowCount">Number of rows per page.</param>
        /// <returns>Collection of customer orders for the specified page.</returns>
        public IEnumerable<CustomerOrder> GetAll(int pageNumber, int rowCount)
        {
            return this.context.CustomerOrders
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        /// <summary>
        /// Retrieves a customer order by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the order.</param>
        /// <returns>Customer order entity if found; otherwise, null.</returns>
        public CustomerOrder GetById(int id)
        {
            return this.context.CustomerOrders.Find(id);
        }

        /// <summary>
        /// Updates an existing customer order in the database and saves changes immediately.
        /// </summary>
        /// <param name="entity">Customer order entity with updated values.</param>
        public void Update(CustomerOrder entity)
        {
            this.context.CustomerOrders.Update(entity);
            this.context.SaveChanges();
        }
    }
}
