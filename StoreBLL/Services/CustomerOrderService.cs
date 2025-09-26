// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\CustomerOrderService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Repository;

/// <summary>
/// Business logic service for customer orders (CRUD + state transitions).
/// Includes inventory side-effects for cancel/confirm flows.
/// </summary>
public class CustomerOrderService(StoreDbContext context)
{
    /// <summary>
    /// Allowed order status transitions.
    /// 1: New; 2: Cancelled by user (terminal); 3: Cancelled by administrator (terminal);
    /// 4: Confirmed; 5: Moved to delivery company; 6: In delivery; 7: Delivered to client;
    /// 8: Delivery confirmed by client (terminal).
    /// </summary>
    private static readonly Dictionary<int, int[]> AllowedTransitions = new Dictionary<int, int[]>
    {
        { 1, new[] { 2, 3, 4 } }, // New -> Cancel(user/admin) or Confirmed
        { 4, new[] { 3, 5 } },    // Confirmed -> Cancel(admin) or Moved
        { 5, new[] { 6 } },       // Moved -> In delivery
        { 6, new[] { 7 } },       // In delivery -> Delivered to client
        { 7, new[] { 8 } },       // Delivered -> Delivery confirmed by client
    };

    // Primary-constructor field.
    private readonly StoreDbContext context = context ?? throw new ArgumentNullException(nameof(context));

    // Explicit types (avoid target-typed new to keep analyzers happy).
    private readonly CustomerOrderRepository repository = new CustomerOrderRepository(context);
    private readonly StockReservationService stockService = new StockReservationService(context);

    /// <summary>
    /// Returns a human-readable name for an order status identifier.
    /// </summary>
    /// <param name="id">Order status identifier.</param>
    /// <returns>Human-readable status name.</returns>
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
    /// Returns the list of allowed next statuses for the current status.
    /// </summary>
    /// <param name="currentStateId">Current order status identifier.</param>
    /// <returns>Read-only list of allowed next status identifiers, or an empty list.</returns>
    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
        AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();

    /// <summary>
    /// Checks whether a transition from one status to another is allowed.
    /// </summary>
    /// <param name="fromStateId">Current status identifier.</param>
    /// <param name="toStateId">Target status identifier.</param>
    /// <returns><see langword="true"/> if transition is allowed; otherwise, <see langword="false"/>.</returns>
    public static bool CanTransition(int fromStateId, int toStateId) =>
        AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

    // ---------- CRUD ----------

    /// <summary>
    /// Adds a new order entity from the specified model.
    /// </summary>
    /// <param name="model">Order model (expected <see cref="CustomerOrderModel"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not a <see cref="CustomerOrderModel"/>.</exception>
    public void Add(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel.", nameof(model));
        }

        var entity = new CustomerOrder(
            id: 0,
            operationTime: m.OperationTime ?? DateTime.UtcNow.ToString("u"),
            userId: m.UserId,
            orderStateId: m.OrderStateId);

        this.repository.Add(entity);
        m.Id = entity.Id;
    }

    /// <summary>
    /// Deletes an order by identifier.
    /// </summary>
    /// <param name="modelId">Order identifier.</param>
    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    /// <summary>
    /// Returns all orders as models.
    /// </summary>
    /// <returns>Enumeration of orders as <see cref="CustomerOrderModel"/>.</returns>
    public IEnumerable<AbstractModel> GetAll() =>
        this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));

    /// <summary>
    /// Returns an order by identifier or throws if not found.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <returns>Order model mapped to <see cref="CustomerOrderModel"/>.</returns>
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
    /// Updates an existing order from the specified model.
    /// </summary>
    /// <param name="model">Order model (expected <see cref="CustomerOrderModel"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not a <see cref="CustomerOrderModel"/>.</exception>
    public void Update(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel.", nameof(model));
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
    /// Changes the order status with validation and inventory side-effects.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="newStateId">Target status identifier.</param>
    /// <param name="error">Error message if the operation fails.</param>
    /// <returns><see langword="true"/> if the status was changed; otherwise, <see langword="false"/>.</returns>
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

        // 1) Save new state.
        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        this.context.SaveChanges();

        // 2) Inventory side-effects.
        if (newStateId == 3)
        {
            // Admin cancel → release reservations.
            this.stockService.ReleaseOrderReservations(orderId);
        }
        else if (newStateId == 8)
        {
            // Confirm delivery → decrease stock & zero reservations.
            this.stockService.ConfirmOrderDelivery(orderId);
        }

        return true;
    }

    /// <summary>
    /// Cancels the user's own order (only if state is 1: New).
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <param name="error">Error message if the operation fails.</param>
    /// <returns><see langword="true"/> if the order was canceled; otherwise, <see langword="false"/>.</returns>
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

        if (entity.OrderStateId != 1)
        {
            error = "Only new orders can be canceled.";
            return false;
        }

        this.stockService.ReleaseOrderReservations(orderId);

        entity.OrderStateId = 2;
        this.repository.Update(entity);
        this.context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Marks the order as received by the user (7 → 8) with inventory effects.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="userId">User identifier.</param>
    /// <param name="error">Error message if the operation fails.</param>
    /// <returns><see langword="true"/> if the order was marked as received; otherwise, <see langword="false"/>.</returns>
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

        if (entity.OrderStateId != 7)
        {
            error = "Only delivered orders can be marked as received.";
            return false;
        }

        entity.OrderStateId = 8;
        this.repository.Update(entity);
        this.context.SaveChanges();

        this.stockService.ConfirmOrderDelivery(orderId);
        return true;
    }

    /// <summary>
    /// Returns user orders ordered by identifier descending.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <returns>Enumeration of user orders as <see cref="CustomerOrderModel"/>.</returns>
    public IEnumerable<CustomerOrderModel> GetOrdersByUser(int userId) =>
        this.repository.GetAll()
            .Where(o => o.UserId == userId)
            .Select(o => new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId))
            .OrderByDescending(o => o.Id);
}
