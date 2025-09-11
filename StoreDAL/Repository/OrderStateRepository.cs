using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>EF Core repository for order_states.</summary>
    public class OrderStateRepository : IOrderStateRepository
    {
        private readonly StoreDbContext context;

        public OrderStateRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public void Add(OrderState entity)
        {
            this.context.OrderStates.Add(entity);
            this.context.SaveChanges();
        }

        public void Update(OrderState entity)
        {
            this.context.OrderStates.Update(entity);
            this.context.SaveChanges();
        }

        public void Delete(OrderState entity)
        {
            this.context.OrderStates.Remove(entity);
            this.context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var state = this.context.OrderStates.Find(id);
            if (state != null)
            {
                this.context.OrderStates.Remove(state);
                this.context.SaveChanges();
            }
        }

        public IEnumerable<OrderState> GetAll()
        {
            return this.context.OrderStates
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToList();
        }

        public IEnumerable<OrderState> GetAll(int pageNumber, int rowCount)
        {
            return this.context.OrderStates
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public OrderState? GetById(int id) =>
            this.context.OrderStates.Find(id);

        public OrderState? GetByName(string stateName) =>
            this.context.OrderStates.AsNoTracking().FirstOrDefault(s => s.StateName == stateName);
    }
}
