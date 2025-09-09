namespace StoreBLL.Services;
using StoreBLL.Interfaces;
using StoreBLL.Models;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

public class CustomerOrderService : ICrud
{
    private readonly ICustomerOrderRepository repository;

    // Allowed transitions by OrderStateId:
    // 1: New Order
    // 2: Canceled by user
    // 3: Canceled by administrator
    // 4: Confirmed
    // 5: Moved to delivery company
    // 6: In delivery
    // 7: Delivered to client
    // 8: Delivery confirmed by client
    private static readonly Dictionary<int, int[]> AllowedTransitions = new()
    {
        [1] = new[] { 2, 3, 4 }, // New -> Canceled(user/admin) or Confirmed
        [4] = new[] { 3, 5 },    // Confirmed -> Canceled by admin or Moved to delivery
        [5] = new[] { 6 },       // Moved -> In delivery
        [6] = new[] { 7 },       // In delivery -> Delivered to client
        [7] = new[] { 8 },       // Delivered -> Delivery confirmed by client
        // 2,3,8 are terminal
    };

    public CustomerOrderService(StoreDbContext context)
    {
        this.repository = new CustomerOrderRepository(context);
    }

    public void Add(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));

        var entity = new CustomerOrder(
            id: 0,
            operationTime: m.OperationTime ?? DateTime.UtcNow.ToString("u"),
            userId: m.UserId,
            orderStateId: m.OrderStateId
        );
        this.repository.Add(entity);
    }

    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId
            ));
    }

    public AbstractModel GetById(int id)
    {
        var o = this.repository.GetById(id);
        if (o == null) return null;
        return new CustomerOrderModel(
            id: o.Id,
            userId: o.UserId,
            operationTime: o.OperationTime,
            orderStateId: o.OrderStateId
        );
    }

    public void Update(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));

        var entity = this.repository.GetById(m.Id);
        if (entity == null) return;

        entity.UserId = m.UserId;
        entity.OperationTime = m.OperationTime ?? entity.OperationTime;
        entity.OrderStateId = m.OrderStateId;

        this.repository.Update(entity);
    }

    /// <summary>Безпечно змінює стан з валідацією дозволених переходів.</summary>
    public bool TryChangeState(int orderId, int newStateId)
    {
        var entity = this.repository.GetById(orderId);
        if (entity == null) return false;

        if (!AllowedTransitions.TryGetValue(entity.OrderStateId, out var allowed) || !allowed.Contains(newStateId))
            return false;

        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        return true;
    }
}
