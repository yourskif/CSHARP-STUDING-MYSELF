using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly StoreDbContext context;

        public OrderDetailRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public void Add(OrderDetail entity)
        {
            this.context.OrderDetails.Add(entity);
            this.context.SaveChanges();
        }

        public void Delete(OrderDetail entity)
        {
            this.context.OrderDetails.Remove(entity);
            this.context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var detail = this.context.OrderDetails.Find(id);
            if (detail != null)
            {
                this.context.OrderDetails.Remove(detail);
                this.context.SaveChanges();
            }
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            return this.context.OrderDetails.ToList();
        }

        public IEnumerable<OrderDetail> GetAll(int pageNumber, int rowCount)
        {
            return this.context.OrderDetails
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public OrderDetail GetById(int id)
        {
            return this.context.OrderDetails.Find(id);
        }

        public void Update(OrderDetail entity)
        {
            this.context.OrderDetails.Update(entity);
            this.context.SaveChanges();
        }
    }
}
