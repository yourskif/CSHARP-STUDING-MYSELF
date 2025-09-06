// StoreDAL/Interfaces/IOrderStateRepository.cs
using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for OrderState. Inherits basic CRUD from IRepository{OrderState}.
    /// </summary>
    public interface IOrderStateRepository : IRepository<OrderState>
    {
        /// <summary>Returns an order state by its name, or null if not found.</summary>
        OrderState? GetByName(string stateName);
    }
}
