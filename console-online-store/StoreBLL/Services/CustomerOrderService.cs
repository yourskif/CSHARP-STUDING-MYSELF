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
/// Enhanced business logic service for customer orders with comprehensive validation and state management.
/// Provides CRUD operations, state transitions, user/admin operations, and advanced validation rules.
/// Includes inventory side-effects and business rule enforcement to maintain data integrity.
/// </summary>
public class CustomerOrderService : ICustomerOrderService
{
    /// <summary>
    /// Allowed state transitions by OrderStateId with comprehensive workflow support.
    /// </summary>
    /// <remarks>
    /// Order State Flow:
    /// 1: New Order (can transition to: 2-Cancel(user), 3-Cancel(admin), 4-Confirmed)
    /// 2: Canceled by user (terminal state)
    /// 3: Canceled by administrator (terminal state)
    /// 4: Confirmed (can transition to: 3-Cancel(admin), 5-Moved to delivery)
    /// 5: Moved to delivery company (can transition to: 6-In delivery)
    /// 6: In delivery (can transition to: 7-Delivered to client)
    /// 7: Delivered to client (can transition to: 8-Delivery confirmed by client)
    /// 8: Delivery confirmed by client (terminal state)
    /// </remarks>
    private static readonly Dictionary<int, int[]> AllowedTransitions = new()
    {
        { 1, new[] { 2, 3, 4 } }, // New -> Cancel(user/admin) or Confirmed
        { 4, new[] { 3, 5 } },    // Confirmed -> Cancel(admin) or Moved to delivery
        { 5, new[] { 6 } },       // Moved to delivery -> In delivery
        { 6, new[] { 7 } },       // In delivery -> Delivered to client
        { 7, new[] { 8 } },       // Delivered to client -> Delivery confirmed by client
    };

    /// <summary>
    /// Business validation rules for state transitions.
    /// </summary>
    private static readonly Dictionary<int, string> StateValidationRules = new()
    {
        { 1, "Order must have valid items and sufficient stock" },
        { 2, "Only new orders can be cancelled by user" },
        { 3, "Admin can cancel orders in New or Confirmed state" },
        { 4, "Order must be validated and stock reserved" },
        { 5, "Order must be confirmed before moving to delivery" },
        { 6, "Order must be with delivery company first" },
        { 7, "Order must be in delivery before marking as delivered" },
        { 8, "Order must be delivered before client confirmation" }
    };

    /// <summary>
    /// Database context for operations.
    /// </summary>
    private readonly StoreDbContext context;

    /// <summary>
    /// Repository for customer order operations.
    /// </summary>
    private readonly CustomerOrderRepository repository;

    /// <summary>
    /// Service for managing stock reservations and inventory.
    /// </summary>
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerOrderService"/> class.
    /// </summary>
    /// <param name="context">EF Core StoreDbContext instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public CustomerOrderService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.repository = new CustomerOrderRepository(context);
        this.stockService = new StockReservationService(context);
    }

    /// <summary>
    /// Maps state id to human-readable name with proper formatting.
    /// </summary>
    /// <param name="id">Order state id.</param>
    /// <returns>Readable state name with appropriate formatting.</returns>
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
            _ => $"Unknown Status ({id})",
        };

    /// <summary>
    /// Gets the validation rule description for a given state.
    /// </summary>
    /// <param name="stateId">Order state id.</param>
    /// <returns>Validation rule description.</returns>
    public static string GetStateValidationRule(int stateId) =>
        StateValidationRules.TryGetValue(stateId, out var rule) ? rule : "No specific validation rules";

    /// <summary>
    /// Returns allowed next states for the given current state.
    /// </summary>
    /// <param name="currentStateId">Current state id.</param>
    /// <returns>Read-only list of allowed next state ids.</returns>
    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
        AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();

    /// <summary>
    /// Validates if transition is allowed between states.
    /// </summary>
    /// <param name="fromStateId">Current state id.</param>
    /// <param name="toStateId">Target state id.</param>
    /// <returns>True if transition is allowed; otherwise false.</returns>
    public static bool CanTransition(int fromStateId, int toStateId) =>
        AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

    /// <summary>
    /// Checks if a state is a terminal state (no further transitions possible).
    /// </summary>
    /// <param name="stateId">State id to check.</param>
    /// <returns>True if state is terminal; otherwise false.</returns>
    public static bool IsTerminalState(int stateId) =>
        !AllowedTransitions.ContainsKey(stateId) || AllowedTransitions[stateId].Length == 0;

    /// <summary>
    /// Checks if a state represents a cancelled order.
    /// </summary>
    /// <param name="stateId">State id to check.</param>
    /// <returns>True if order is cancelled; otherwise false.</returns>
    public static bool IsCancelledState(int stateId) => stateId == 2 || stateId == 3;

    /// <summary>
    /// Checks if a state represents a completed order.
    /// </summary>
    /// <param name="stateId">State id to check.</param>
    /// <returns>True if order is completed; otherwise false.</returns>
    public static bool IsCompletedState(int stateId) => stateId == 8;

    /// <summary>
    /// Checks if a state represents an active (in-progress) order.
    /// </summary>
    /// <param name="stateId">State id to check.</param>
    /// <returns>True if order is active; otherwise false.</returns>
    public static bool IsActiveState(int stateId) => stateId >= 1 && stateId <= 7 && !IsCancelledState(stateId);

    #region ICrud Implementation

    /// <summary>
    /// Adds a new customer order with validation.
    /// </summary>
    /// <param name="model">Order model to add (must be CustomerOrderModel).</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid.</exception>
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
    /// Deletes an order by its id with validation.
    /// </summary>
    /// <param name="modelId">Order id.</param>
    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    /// <summary>
    /// Gets all orders with proper mapping.
    /// </summary>
    /// <returns>Sequence of CustomerOrderModel instances.</returns>
    public IEnumerable<AbstractModel> GetAll() =>
        this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));

    /// <summary>
    /// Gets an order by id with validation.
    /// </summary>
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

    /// <summary>
    /// Updates an existing order with validation.
    /// </summary>
    /// <param name="model">Order model.</param>
    /// <exception cref="ArgumentException">Thrown when model type is invalid.</exception>
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

    #endregion

    #region Enhanced State Management

    /// <summary>
    /// Safely changes order state with comprehensive validation and inventory side-effects.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="newStateId">Target state id.</param>
    /// <param name="error">Output error message when transition is rejected.</param>
    /// <returns>True if state was changed; otherwise false.</returns>
    public bool TryChangeState(int orderId, int newStateId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        // Validate transition is allowed
        if (!CanTransition(entity.OrderStateId, newStateId))
        {
            var next = string.Join(", ", GetAllowedNextStates(entity.OrderStateId).Select(StatusName));
            error = $"Transition not allowed. Current: {StatusName(entity.OrderStateId)}. Allowed next: [{next}].";
            return false;
        }

        // Perform comprehensive business validation
        if (!ValidateOrderTransition(orderId, entity.OrderStateId, newStateId, out var validationError))
        {
            error = validationError;
            return false;
        }

        // Check for duplicate state transition
        if (entity.OrderStateId == newStateId)
        {
            error = $"Order is already in {StatusName(newStateId)} state.";
            return false;
        }

        // Save new state
        var previousState = entity.OrderStateId;
        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        this.context.SaveChanges();

        // Apply inventory side-effects based on state transition
        try
        {
            ApplyInventorySideEffects(orderId, previousState, newStateId);
        }
        catch (Exception ex)
        {
            // Rollback state change if inventory operation fails
            entity.OrderStateId = previousState;
            this.repository.Update(entity);
            this.context.SaveChanges();

            error = $"State transition failed due to inventory error: {ex.Message}";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Performs comprehensive business validation for order state transitions.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="currentStateId">Current order state.</param>
    /// <param name="newStateId">Target state.</param>
    /// <param name="validationError">Output validation error message.</param>
    /// <returns>True if validation passes; otherwise false.</returns>
    public bool ValidateOrderTransition(int orderId, int currentStateId, int newStateId, out string validationError)
    {
        validationError = string.Empty;

        // State-specific validation rules
        switch (newStateId)
        {
            case 2: // User cancellation
                if (currentStateId != 1)
                {
                    validationError = "Only new orders can be cancelled by user.";
                    return false;
                }
                break;

            case 3: // Admin cancellation
                if (currentStateId != 1 && currentStateId != 4)
                {
                    validationError = "Admin can only cancel orders in New or Confirmed state.";
                    return false;
                }
                break;

            case 4: // Confirmation
                if (!HasSufficientStock(orderId))
                {
                    validationError = "Insufficient stock to confirm order.";
                    return false;
                }

                if (!HasValidOrderItems(orderId))
                {
                    validationError = "Order contains invalid or discontinued items.";
                    return false;
                }
                break;

            case 8: // Client confirmation
                if (HasAlreadyBeenConfirmed(orderId))
                {
                    validationError = "Order has already been confirmed by client.";
                    return false;
                }
                break;
        }

        // Time-based validations
        if (!ValidateTransitionTiming(orderId, currentStateId, newStateId, out var timingError))
        {
            validationError = timingError;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates if order has sufficient stock for all items.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>True if sufficient stock available; otherwise false.</returns>
    private bool HasSufficientStock(int orderId)
    {
        var details = this.context.OrderDetails.Where(d => d.OrderId == orderId).ToList();

        foreach (var detail in details)
        {
            var product = this.context.Products.Find(detail.ProductId);
            if (product == null || product.AvailableQuantity < detail.ProductAmount)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates if order contains valid items that are still available.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>True if all items are valid; otherwise false.</returns>
    private bool HasValidOrderItems(int orderId)
    {
        var details = this.context.OrderDetails.Where(d => d.OrderId == orderId).ToList();

        if (!details.Any())
        {
            return false; // Empty orders are invalid
        }

        foreach (var detail in details)
        {
            var product = this.context.Products.Find(detail.ProductId);
            if (product == null || detail.ProductAmount <= 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if order has already been confirmed by client.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>True if already confirmed; otherwise false.</returns>
    private bool HasAlreadyBeenConfirmed(int orderId)
    {
        var order = this.context.CustomerOrders.Find(orderId);
        return order?.OrderStateId == 8;
    }

    /// <summary>
    /// Validates timing constraints for state transitions.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="currentState">Current state.</param>
    /// <param name="newState">Target state.</param>
    /// <param name="error">Output error message.</param>
    /// <returns>True if timing is valid; otherwise false.</returns>
    private bool ValidateTransitionTiming(int orderId, int currentState, int newState, out string error)
    {
        error = string.Empty;

        // Example: Orders older than 30 days cannot be cancelled
        if (newState == 2 || newState == 3)
        {
            var order = this.context.CustomerOrders.Find(orderId);
            if (order != null && DateTime.TryParse(order.OperationTime, out var orderDate))
            {
                if (DateTime.UtcNow.Subtract(orderDate).TotalDays > 30)
                {
                    error = "Orders older than 30 days cannot be cancelled.";
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Applies appropriate inventory side-effects based on state transitions.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="previousState">Previous order state.</param>
    /// <param name="newState">New order state.</param>
    private void ApplyInventorySideEffects(int orderId, int previousState, int newState)
    {
        switch (newState)
        {
            case 2: // User cancellation
            case 3: // Admin cancellation
                this.stockService.ReleaseOrderReservations(orderId);
                break;

            case 4: // Confirmation
                // Ensure stock is properly reserved
                if (previousState == 1)
                {
                    // Stock should already be reserved from order creation
                    // but we can validate and adjust if needed
                    ValidateAndAdjustReservations(orderId);
                }
                break;

            case 8: // Client confirmation - finalize delivery
                this.stockService.ConfirmOrderDelivery(orderId);
                break;
        }
    }

    /// <summary>
    /// Validates and adjusts stock reservations for an order.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    private void ValidateAndAdjustReservations(int orderId)
    {
        var details = this.context.OrderDetails.Where(d => d.OrderId == orderId).ToList();

        foreach (var detail in details)
        {
            var product = this.context.Products.Find(detail.ProductId);
            if (product != null)
            {
                // Ensure adequate reservation exists
                var currentReservation = product.ReservedQuantity;
                var neededReservation = detail.ProductAmount;

                if (currentReservation < neededReservation)
                {
                    // Adjust reservation if insufficient
                    var additionalReservation = neededReservation - currentReservation;
                    if (product.AvailableQuantity >= additionalReservation)
                    {
                        product.ReservedQuantity += additionalReservation;
                    }
                }
            }
        }

        this.context.SaveChanges();
    }

    #endregion

    #region User Operations

    /// <summary>
    /// Cancels user's own order with enhanced validation and feedback.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">Owner user id.</param>
    /// <param name="error">Output error message on failure.</param>
    /// <returns>True if canceled; otherwise false.</returns>
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
            error = $"Only new orders can be canceled. Current status: {StatusName(entity.OrderStateId)}.";
            return false;
        }

        // Use the enhanced state transition with validation
        return this.TryChangeState(orderId, 2, out error);
    }

    /// <summary>
    /// Marks delivered order as received with comprehensive validation.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    /// <param name="userId">Owner user id.</param>
    /// <param name="error">Output error message on failure.</param>
    /// <returns>True if marked; otherwise false.</returns>
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
            error = $"Only delivered orders can be marked as received. Current status: {StatusName(entity.OrderStateId)}.";
            return false;
        }

        // Use the enhanced state transition with validation
        return this.TryChangeState(orderId, 8, out error);
    }

    #endregion

    #region Query Operations

    /// <summary>
    /// Gets all orders for a specific user ordered by Id descending with enhanced filtering.
    /// </summary>
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

    /// <summary>
    /// Gets orders by state with optional user filtering.
    /// </summary>
    /// <param name="stateId">Order state to filter by.</param>
    /// <param name="userId">Optional user ID filter.</param>
    /// <returns>Sequence of matching orders.</returns>
    public IEnumerable<CustomerOrderModel> GetOrdersByState(int stateId, int? userId = null)
    {
        var query = this.repository.GetAll().Where(o => o.OrderStateId == stateId);

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        return query.Select(o => new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId))
            .OrderByDescending(o => o.Id);
    }

    /// <summary>
    /// Gets order statistics for reporting.
    /// </summary>
    /// <param name="userId">Optional user ID filter.</param>
    /// <returns>Dictionary with order statistics.</returns>
    public Dictionary<string, int> GetOrderStatistics(int? userId = null)
    {
        var query = this.repository.GetAll();

        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }

        var orders = query.ToList();

        return new Dictionary<string, int>
        {
            ["Total"] = orders.Count,
            ["Active"] = orders.Count(o => IsActiveState(o.OrderStateId)),
            ["Completed"] = orders.Count(o => IsCompletedState(o.OrderStateId)),
            ["Cancelled"] = orders.Count(o => IsCancelledState(o.OrderStateId)),
            ["New"] = orders.Count(o => o.OrderStateId == 1),
            ["Confirmed"] = orders.Count(o => o.OrderStateId == 4),
            ["InDelivery"] = orders.Count(o => o.OrderStateId == 6),
            ["Delivered"] = orders.Count(o => o.OrderStateId == 7)
        };
    }

    #endregion
}
