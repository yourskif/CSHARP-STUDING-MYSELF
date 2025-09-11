using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>Repository contract for OrderState with lookup by name.</summary>
    public interface IOrderStateRepository : IRepository<OrderState>
    {
        OrderState? GetByName(string stateName);
    }
}
