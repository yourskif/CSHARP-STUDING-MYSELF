// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\CustomerOrderService.cs
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
/// Business logic service for customer order management.
/// Provides CRUD operations, state transition validation, and inventory management integration.
/// Implements order workflow with automatic stock reservation/release based on state changes.
/// </summary>
/// <remarks>
/// <para>
/// This service manages the complete order lifecycle from creation to delivery confirmation.
/// It enforces business rules for state transitions and coordinates with <see cref="StockReservationService"/>
/// to maintain inventory consistency.
/// </para>
/// <para>
/// Order workflow states:
/// <list type="number">
/// <item><description>New Order - Initial state, stock reserved</description></item>
/// <item><description>Cancelled by user - Terminal state, stock released</description></item>
/// <item><description>Cancelled by administrator - Terminal state, stock released</description></item>
/// <item><description>Confirmed - Order approved for processing</description></item>
/// <item><description>Moved to delivery company - Transferred to logistics</description></item>
/// <item><description>In delivery - Currently being shipped</description></item>
/// <item><description>Delivered to client - Awaiting customer confirmation</description></item>
/// <item><description>Delivery confirmed by client - Terminal state, stock decremented</description></item>
/// </list>
/// </para>
/// </remarks>
public class CustomerOrderService : ICustomerOrderService
{
    /// <summary>
    /// Defines allowed state transitions for order workflow.
    /// Dictionary key is the current state, value is array of valid next states.
    /// </summary>
    /// <remarks>
    /// State transition rules:
    /// <list type="bullet">
    /// <item><description>1 (New Order) → 2 (Cancel by user), 3 (Cancel by admin), or 4 (Confirmed)</description></item>
    /// <item><description>4 (Confirmed) → 3 (Cancel by admin) or 5 (Moved to delivery)</description></item>
    /// <item><description>5 (Moved to delivery) → 6 (In delivery)</description></item>
    /// <item><description>6 (In delivery) → 7 (Delivered to client)</description></item>
    /// <item><description>7 (Delivered to client) → 8 (Delivery confirmed by client)</description></item>
    /// </list>
    /// Terminal states (2, 3, 8) have no outgoing transitions.
    /// </remarks>
    private static readonly Dictionary<int, int[]> AllowedTransitions = new()
    {
        { 1, new[] { 2, 3, 4 } }, // New -> Cancel(user/admin) or Confirmed
        { 4, new[] { 3, 5 } },    // Confirmed -> Cancel(admin) or Moved
        { 5, new[] { 6 } },       // Moved -> In delivery
        { 6, new[] { 7 } },       // In delivery -> Delivered to client
        { 7, new[] { 8 } },       // Delivered -> Delivery confirmed by client
    };

    private readonly StoreDbContext context;
    private readonly CustomerOrderRepository repository;
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerOrderService"/> class.
    /// </summary>
    /// <param name="context">EF Core database context for order operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    public CustomerOrderService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.repository = new CustomerOrderRepository(context);
        this.stockService = new StockReservationService(context);
    }

    /// <summary>
    /// Converts order state identifier to human-readable name.
    /// </summary>
    /// <param name="id">Order state identifier (1-8).</param>
    /// <returns>Localized state name, or "Unknown" if identifier is not recognized.</returns>
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
    /// Gets the list of valid next states from the current state.
    /// </summary>
    /// <param name="currentStateId">Current order state identifier.</param>
    /// <returns>
    /// Read-only collection of allowed next state identifiers.
    /// Returns empty collection if current state is terminal or unrecognized.
    /// </returns>
    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
        AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();

    /// <summary>
    /// Validates whether a state transition is allowed by workflow rules.
    /// </summary>
    /// <param name="fromStateId">Current state identifier.</param>
    /// <param name="toStateId">Target state identifier.</param>
    /// <returns>
    /// <see langword="true"/> if the transition from <paramref name="fromStateId"/>
    /// to <paramref name="toStateId"/> is permitted; otherwise <see langword="false"/>.
    /// </returns>
    public static bool CanTransition(int fromStateId, int toStateId) =>
        AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

    /// <summary>
    /// Adds a new customer order to the database.
    /// </summary>
    /// <param name="model">Order model containing order data (must be <see cref="CustomerOrderModel"/>).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not of type <see cref="CustomerOrderModel"/>.</exception>
    /// <remarks>
    /// The operation time defaults to current UTC time if not specified.
    /// The model's Id property is updated with the generated database identifier after insertion.
    /// Stock reservations must be managed separately by the caller.
    /// </remarks>
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

    /// <summary>
    /// Deletes an order by its identifier.
    /// </summary>
    /// <param name="modelId">Order identifier to delete.</param>
    /// <remarks>
    /// Warning: This operation does not automatically release stock reservations.
    /// Ensure reservations are released before deletion to maintain inventory integrity.
    /// </remarks>
    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    /// <summary>
    /// Retrieves all orders from the database.
    /// </summary>
    /// <returns>Collection of all orders as <see cref="AbstractModel"/> instances.</returns>
    public IEnumerable<AbstractModel> GetAll() =>
        this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));

    /// <summary>
    /// Retrieves a single order by its identifier.
    /// </summary>
    /// <param name="id">Order identifier.</param>
    /// <returns>Order model with the specified identifier.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when order with specified <paramref name="id"/> is not found.</exception>
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
    /// Updates an existing order with new values.
    /// </summary>
    /// <param name="model">Order model with updated data (must be <see cref="CustomerOrderModel"/>).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="model"/> is not of type <see cref="CustomerOrderModel"/>.</exception>
    /// <remarks>
    /// If the order is not found, the operation silently returns without error.
    /// The operation time is preserved if not specified in the model.
    /// State changes through this method bypass workflow validation - use <see cref="TryChangeState"/> instead.
    /// </remarks>
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
    /// Changes order state with workflow validation and automatic inventory adjustments.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="newStateId">Target state identifier.</param>
    /// <param name="error">Output parameter containing error message if operation fails.</param>
    /// <returns>
    /// <see langword="true"/> if state was successfully changed;
    /// <see langword="false"/> if validation failed or order not found.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method enforces workflow transition rules and triggers inventory side effects:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Transition to state 3 (Admin cancel): Releases stock reservations</description></item>
    /// <item><description>Transition to state 8 (Delivery confirmed): Decrements stock and releases reservations</description></item>
    /// </list>
    /// <para>
    /// All changes are persisted immediately via SaveChanges.
    /// </para>
    /// </remarks>
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

        // 1) Save new state
        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        this.context.SaveChanges();

        // 2) Inventory side-effects depending on the new state
        if (newStateId == 3)
        {
            // Admin cancel → release reservations
            this.stockService.ReleaseOrderReservations(orderId);
        }
        else if (newStateId == 8)
        {
            // Confirm delivery → decrement stock & re-evaluate reservations
            this.stockService.ConfirmOrderDelivery(orderId);
        }

        return true;
    }

    /// <summary>
    /// Cancels a user's own order (only from New state) and releases stock reservations.
    /// </summary>
    /// <param name="orderId">Order identifier to cancel.</param>
    /// <param name="userId">User identifier requesting cancellation.</param>
    /// <param name="error">Output parameter containing error message if operation fails.</param>
    /// <returns>
    /// <see langword="true"/> if order was successfully cancelled;
    /// <see langword="false"/> if validation failed or order not found.
    /// </returns>
    /// <remarks>
    /// Business rules enforced:
    /// <list type="bullet">
    /// <item><description>Order must belong to the requesting user</description></item>
    /// <item><description>Order must be in state 1 (New Order)</description></item>
    /// <item><description>Stock reservations are released before state change</description></item>
    /// </list>
    /// Transitions order to state 2 (Cancelled by user).
    /// </remarks>
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

        // Release reservations first, then switch to state 2
        this.stockService.ReleaseOrderReservations(orderId);

        entity.OrderStateId = 2;
        this.repository.Update(entity);
        this.context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Marks a delivered order as received by the customer, confirming delivery and finalizing inventory changes.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="userId">User identifier confirming receipt.</param>
    /// <param name="error">Output parameter containing error message if operation fails.</param>
    /// <returns>
    /// <see langword="true"/> if order was successfully marked as received;
    /// <see langword="false"/> if validation failed or order not found.
    /// </returns>
    /// <remarks>
    /// Business rules enforced:
    /// <list type="bullet">
    /// <item><description>Order must belong to the requesting user</description></item>
    /// <item><description>Order must be in state 7 (Delivered to client)</description></item>
    /// <item><description>Stock quantities are decremented and reservations released</description></item>
    /// </list>
    /// Transitions order to state 8 (Delivery confirmed by client).
    /// </remarks>
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

        // Change to state 8, then confirm delivery in stock
        entity.OrderStateId = 8;
        this.repository.Update(entity);
        this.context.SaveChanges();

        this.stockService.ConfirmOrderDelivery(orderId);
        return true;
    }

    /// <summary>
    /// Retrieves all orders for a specific user, sorted by identifier descending.
    /// </summary>
    /// <param name="userId">User identifier to filter by.</param>
    /// <returns>Collection of orders belonging to the specified user, most recent first.</returns>
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
