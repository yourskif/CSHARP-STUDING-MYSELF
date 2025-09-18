namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

/// <summary>
/// Business logic service for customer order operations.
/// Handles order creation, state transitions, and user-specific order management.
/// </summary>
public class CustomerOrderService : ICrud
{
    private readonly ICustomerOrderRepository repository;

    // Allowed state transitions by OrderStateId:
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
        // 2,3,8 are terminal states
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

        // Set the generated ID back to the model
        m.Id = entity.Id;
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

    /// <summary>
    /// Safely changes order state with validation of allowed transitions.
    /// </summary>
    /// <param name="orderId">Order ID to change state for.</param>
    /// <param name="newStateId">New state ID to transition to.</param>
    /// <returns>True if state change was successful, false otherwise.</returns>
    public bool TryChangeState(int orderId, int newStateId)
    {
        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            return false;
        }

        if (!AllowedTransitions.TryGetValue(entity.OrderStateId, out var allowed) ||
            !allowed.Contains(newStateId))
        {
            return false;
        }

        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Cancels user's own order if business rules allow it.
    /// </summary>
    /// <param name="orderId">Order ID to cancel.</param>
    /// <param name="userId">User ID requesting cancellation.</param>
    /// <param name="error">Error message if cancellation fails.</param>
    /// <returns>True if cancellation successful, false otherwise.</returns>
    public bool CancelOwnOrder(int orderId, int userId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (entity.UserId != userId)
        {
            error = "You can only cancel your own orders.";
            return false;
        }

        if (entity.OrderStateId != 1) // Only "New" orders can be canceled by user
        {
            error = "Only new orders can be canceled.";
            return false;
        }

        entity.OrderStateId = 2; // Canceled by user
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Marks delivered order as received by user.
    /// </summary>
    /// <param name="orderId">Order ID to mark as received.</param>
    /// <param name="userId">User ID confirming receipt.</param>
    /// <param name="error">Error message if operation fails.</param>
    /// <returns>True if marking successful, false otherwise.</returns>
    public bool MarkAsReceived(int orderId, int userId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (entity.UserId != userId)
        {
            error = "You can only confirm receipt of your own orders.";
            return false;
        }

        if (entity.OrderStateId != 7) // Only "Delivered to client" orders can be confirmed
        {
            error = "Only delivered orders can be marked as received.";
            return false;
        }

        entity.OrderStateId = 8; // Delivery confirmed by client
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Gets all orders for a specific user.
    /// </summary>
    /// <param name="userId">User ID to get orders for.</param>
    /// <returns>Collection of user's orders.</returns>
    public IEnumerable<CustomerOrderModel> GetOrdersByUser(int userId)
    {
        return this.repository.GetAll()
            .Where(o => o.UserId == userId)
            .Select(o => new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId))
            .OrderByDescending(o => o.Id);
    }
}
