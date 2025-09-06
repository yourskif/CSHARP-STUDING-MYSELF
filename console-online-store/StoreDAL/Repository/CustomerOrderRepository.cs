using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly StoreDbContext context;

        public CustomerOrderRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public void Add(CustomerOrder entity)
        {
            this.context.CustomerOrders.Add(entity);
            this.context.SaveChanges();
        }

        public void Update(CustomerOrder entity)
        {
            this.context.CustomerOrders.Update(entity);
            this.context.SaveChanges();
        }

        public void Delete(CustomerOrder entity)
        {
            this.context.CustomerOrders.Remove(entity);
            this.context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var order = this.context.CustomerOrders.Find(id);
            if (order != null)
            {
                this.context.CustomerOrders.Remove(order);
                this.context.SaveChanges();
            }
        }

        public IEnumerable<CustomerOrder> GetAll()
        {
            return this.context.CustomerOrders
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<CustomerOrder> GetAll(int pageNumber, int rowCount)
        {
            return this.context.CustomerOrders
                .AsNoTracking()
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public CustomerOrder? GetById(int id)
        {
            return this.context.CustomerOrders.Find(id);
        }
    }
}
