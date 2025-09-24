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
    /// <summary>
    /// Allowed state transitions by <c>OrderStateId</c>.
    /// </summary>
    /// <remarks>
    /// 1: New Order<br/>
    /// 2: Canceled by user (terminal)<br/>
    /// 3: Canceled by administrator (terminal)<br/>
    /// 4: Confirmed<br/>
    /// 5: Moved to delivery company<br/>
    /// 6: In delivery<br/>
    /// 7: Delivered to client<br/>
    /// 8: Delivery confirmed by client (terminal).
    /// </remarks>
    private static readonly Dictionary<int, int[]> AllowedTransitions =
        new Dictionary<int, int[]>
        {
            // New -> Canceled(user/admin) or Confirmed
            [1] = new[] { 2, 3, 4 },

            // Confirmed -> Canceled by admin or Moved to delivery
            [4] = new[] { 3, 5 },

            // Moved -> In delivery
            [5] = new[] { 6 },

            // In delivery -> Delivered to client
            [6] = new[] { 7 },

            // Delivered -> Delivery confirmed by client
            [7] = new[] { 8 },
        };

    private readonly ICustomerOrderRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerOrderService"/> class.
    /// </summary>
    /// <param name="context">EF Core DbContext.</param>
    public CustomerOrderService(StoreDbContext context)
    {
        this.repository = new CustomerOrderRepository(context);
    }

    /// <summary>
    /// Maps state id to human-readable name.
    /// </summary>
    /// <param name="id">Order state ID.</param>
    /// <returns>Human-readable state name.</returns>
    public static string StatusName(int id) =>
        id switch
        {
            1 => "New Order",
            2 => "Cancelled by user",
            3 => "Cancelled by administrator",
            4 => "Confirmed",
            5 => "Moved to delivery company",
            6 => "In delivery",
            7 => "Delivered to client",
            8 => "Delivery confirmed by client",
            _ => "Unknown",
        };

    /// <summary>
    /// Returns allowed next states for the given current state.
    /// </summary>
    /// <param name="currentStateId">Current state id.</param>
    /// <returns>Read-only list of allowed next state ids. Empty if none.</returns>
    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId)
    {
        return AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();
    }

    /// <summary>
    /// Validates if a transition is allowed.
    /// </summary>
    /// <param name="fromStateId">Current state id.</param>
    /// <param name="toStateId">Target state id.</param>
    /// <returns><see langword="true"/> if transition is allowed; otherwise <see langword="false"/>.</returns>
    public static bool CanTransition(int fromStateId, int toStateId)
    {
        return AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);
    }

    /// <summary>
    /// Adds a new customer order.
    /// </summary>
    /// <param name="model">Order model to add.</param>
    /// <exception cref="ArgumentException">Thrown when the model type is not <see cref="CustomerOrderModel"/>.</exception>
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

        // In most repositories, persistence happens inside Add/Update.
        this.repository.Add(entity);

        // If Id is assigned by the repository after Add, reflect it back on the model.
        m.Id = entity.Id;
    }

    /// <summary>
    /// Deletes an order by its identifier.
    /// </summary>
    /// <param name="modelId">Order identifier.</param>
    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    /// <summary>
    /// Gets all customer orders.
    /// </summary>
    /// <returns>Sequence of all orders as <see cref="CustomerOrderModel"/>.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));
    }

    /// <summary>
    /// Gets a customer order by its identifier.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <returns>The order model.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the order is not found.</exception>
    public AbstractModel GetById(int id)
    {
        var o = this.repository.GetById(id)
            ?? throw new KeyNotFoundException($"Order with id {id} was not found.");

        return new CustomerOrderModel(
            id: o.Id,
            userId: o.UserId,
            operationTime: o.OperationTime,
            orderStateId: o.OrderStateId);
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="model">Order model with new values.</param>
    /// <exception cref="ArgumentException">Thrown when the model type is not <see cref="CustomerOrderModel"/>.</exception>
    public void Update(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
        }

        var entity = this.repository.GetById(m.Id);
        if (entity == null)
        {
            // No-op if not found (alternatively, throw).
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
    /// <param name="error">Error if transition is not allowed or order not found.</param>
    /// <returns>True if state change was successful, false otherwise.</returns>
    public bool TryChangeState(int orderId, int newStateId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (!CanTransition(entity.OrderStateId, newStateId))
        {
            var next = string.Join(", ", GetAllowedNextStates(entity.OrderStateId).Select(StatusName));
            error = $"Transition not allowed. Current: {StatusName(entity.OrderStateId)}. Allowed next: [{next}].";
            return false;
        }

        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Cancels user's own order if business rules allow it.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">User id who owns the order.</param>
    /// <param name="error">Error message if operation fails.</param>
    /// <returns><see langword="true"/> if canceled; otherwise <see langword="false"/>.</returns>
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

        // Only "New" orders can be canceled by user.
        if (entity.OrderStateId != 1)
        {
            error = "Only new orders can be canceled.";
            return false;
        }

        // Canceled by user.
        entity.OrderStateId = 2;
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Marks delivered order as received by user.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">User id who owns the order.</param>
    /// <param name="error">Error message if operation fails.</param>
    /// <returns><see langword="true"/> if marked as received; otherwise <see langword="false"/>.</returns>
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

        // Only "Delivered to client" orders can be confirmed.
        if (entity.OrderStateId != 7)
        {
            error = "Only delivered orders can be marked as received.";
            return false;
        }

        // Delivery confirmed by client.
        entity.OrderStateId = 8;
        this.repository.Update(entity);
        return true;
    }

    /// <summary>
    /// Gets all orders for a specific user.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <returns>Sequence of user's orders ordered by Id descending.</returns>
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
