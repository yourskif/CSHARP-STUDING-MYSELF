// File: StoreBLL/Services/CustomerOrderService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Repository;

/// <summary>
/// Service for CustomerOrder business logic.
/// </summary>
public class CustomerOrderService : ICrud
{
    private readonly CustomerOrderRepository repository;

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
        {
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
        }

        var entity = new CustomerOrder(
            id: 0,
            operationTime: m.OperationTime ?? DateTime.UtcNow.ToString("u"),
            userId: m.UserId,
            orderStateId: m.OrderStateId);

        this.repository.Add(entity);
        m.Id = entity.Id;
    }

    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));
    }

    public AbstractModel? GetById(int id)
    {
        var o = this.repository.GetById(id);
        if (o == null)
        {
            return null;
        }

        return new CustomerOrderModel(
            id: o.Id,
            userId: o.UserId,
            operationTime: o.OperationTime,
            orderStateId: o.OrderStateId);
    }

    public void Update(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
        }

        var entity = this.repository.GetById(m.Id);
        if (entity == null)
        {
            return;
        }

        entity.UserId = m.UserId;
        entity.OperationTime = m.OperationTime ?? entity.OperationTime;
        entity.OrderStateId = m.OrderStateId;

        this.repository.Update(entity);
    }

    /// <summary>Низькорівневий перехід стану (із валідацією).</summary>
    private bool TryChangeState(int orderId, int newStateId)
    {
        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            return false;
        }

        if (!AllowedTransitions.TryGetValue(entity.OrderStateId, out var allowed) || !allowed.Contains(newStateId))
        {
            return false;
        }

        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        return true;
    }

    /// <summary>Скасування замовлення користувачем (-> stateId = 2).</summary>
    public bool CancelOwnOrder(int orderId, int userId, out string? error)
    {
        error = null;

        var order = this.repository.GetById(orderId);
        if (order == null)
        {
            error = "Order not found.";
            return false;
        }

        if (order.UserId != userId)
        {
            error = "You can cancel only your own order.";
            return false;
        }

        if (order.OrderStateId is 2 or 3 or 8)
        {
            error = "This order is already terminal and cannot be canceled.";
            return false;
        }

        if (!this.TryChangeState(orderId, 2))
        {
            error = $"State transition from {order.OrderStateId} to 2 is not allowed.";
            return false;
        }

        return true;
    }

    /// <summary>Скасування замовлення адміністратором (-> stateId = 3).</summary>
    public bool CancelByAdmin(int orderId, out string? error)
    {
        error = null;

        var order = this.repository.GetById(orderId);
        if (order == null)
        {
            error = "Order not found.";
            return false;
        }

        if (order.OrderStateId is 2 or 3 or 8)
        {
            error = "This order is already terminal and cannot be canceled.";
            return false;
        }

        if (!this.TryChangeState(orderId, 3))
        {
            error = $"State transition from {order.OrderStateId} to 3 is not allowed.";
            return false;
        }

        return true;
    }

    /// <summary>Зміна статусу адміністратором (відповідно до дозволених переходів).</summary>
    public bool ChangeStateByAdmin(int orderId, int newStateId, out string? error)
    {
        error = null;

        var order = this.repository.GetById(orderId);
        if (order == null)
        {
            error = "Order not found.";
            return false;
        }

        if (order.OrderStateId is 2 or 3 or 8)
        {
            error = "This order is terminal and its status cannot be changed.";
            return false;
        }

        if (!this.TryChangeState(orderId, newStateId))
        {
            error = $"State transition from {order.OrderStateId} to {newStateId} is not allowed.";
            return false;
        }

        return true;
    }
}
