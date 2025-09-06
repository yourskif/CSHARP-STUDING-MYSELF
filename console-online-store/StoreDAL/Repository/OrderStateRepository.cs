// StoreDAL/Repository/OrderStateRepository.cs
using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    /// <summary>
    /// EF Core repository for OrderState. Inherits base CRUD and adds lookups.
    /// </summary>
    public class OrderStateRepository : BaseRepository<OrderState>, IOrderStateRepository
    {
        public OrderStateRepository(StoreDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns an order state by its name, or null if not found.
        /// </summary>
        public OrderState? GetByName(string stateName)
        {
            ArgumentException.ThrowIfNullOrEmpty(stateName);
            return this.context.OrderStates
                .AsNoTracking()
                .FirstOrDefault(s => s.StateName == stateName);
        }
    }
}
