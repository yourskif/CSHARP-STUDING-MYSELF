// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Services\CustomerOrderService.cs
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
/// Business logic service for customer orders:
/// CRUD, state transitions, and user/admin operations.
/// Now includes inventory side-effects to satisfy tests.
/// </summary>
public class CustomerOrderService : ICustomerOrderService
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

    /// <summary>Initializes a new instance of the <see cref="CustomerOrderService"/> class.</summary>
    /// <param name="context">EF Core <see cref="StoreDbContext"/> instance.</param>
    public CustomerOrderService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.repository = new CustomerOrderRepository(context);
        this.stockService = new StockReservationService(context);
    }

    /// <summary>Maps state id to human-readable name.</summary>
    /// <param name="id">Order state id.</param>
    /// <returns>Readable state name.</returns>
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

    /// <summary>Returns allowed next states for the given current state.</summary>
    /// <param name="currentStateId">Current state id.</param>
    /// <returns>Read-only list of allowed next state ids.</returns>
    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
        AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();

    /// <summary>Validates if transition is allowed.</summary>
    /// <param name="fromStateId">Current state id.</param>
    /// <param name="toStateId">Target state id.</param>
    /// <returns><see langword="true"/> if transition is allowed; otherwise <see langword="false"/>.</returns>
    public static bool CanTransition(int fromStateId, int toStateId) =>
        AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

    // ---------------- ICrud ----------------

    /// <summary>Adds a new customer order.</summary>
    /// <param name="model">Order model to add (must be <see cref="CustomerOrderModel"/>).</param>
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

    /// <summary>Deletes an order by its id.</summary>
    /// <param name="modelId">Order id.</param>
    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    /// <summary>Gets all orders.</summary>
    /// <returns>Sequence of <see cref="CustomerOrderModel"/>.</returns>
    public IEnumerable<AbstractModel> GetAll() =>
        this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));

    /// <summary>Gets an order by id.</summary>
    /// <param name="id">Order id.</param>
    /// <returns>Order model.</returns>
    /// <exception cref="KeyNotFoundException">If order not found.</exception>
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

    /// <summary>Updates an existing order.</summary>
    /// <param name="model">Order model.</param>
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
    /// Safely changes order state with validation of allowed transitions and applies inventory side-effects.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="newStateId">Target state id.</param>
    /// <param name="error">Output error message when transition is rejected.</param>
    /// <returns><see langword="true"/> if state was changed; otherwise <see langword="false"/>.</returns>
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

    /// <summary>Cancels user's own order (only state 1). Also releases reservations.</summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">Owner user id.</param>
    /// <param name="error">Output error message on failure.</param>
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

    /// <summary>Marks delivered order as received (to 8) and confirms delivery in stock service.</summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">Owner user id.</param>
    /// <param name="error">Output error message on failure.</param>
    /// <returns><see langword="true"/> if marked; otherwise <see langword="false"/>.</returns>
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

    /// <summary>Gets all orders for a specific user ordered by Id descending.</summary>
    /// <param name="userId">User id.</param>
    /// <returns>Sequence of user orders.</returns>
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
