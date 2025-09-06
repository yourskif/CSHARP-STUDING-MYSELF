namespace StoreBLL.Services;

using System.Collections.Generic;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

/// <summary>Simple service over order_states table.</summary>
public class OrderStateService
{
    private readonly IOrderStateRepository repository;

    public OrderStateService(StoreDbContext context)
    {
        this.repository = new OrderStateRepository(context);
    }

    public IEnumerable<OrderState> GetAll() => this.repository.GetAll();

    public OrderState? GetById(int id) => this.repository.GetById(id);

    public OrderState? GetByName(string name) => this.repository.GetByName(name);

    public string GetNameById(int id) => this.repository.GetById(id)?.StateName ?? $"State #{id}";
}
