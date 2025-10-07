// Path: console-online-store/StoreBLL/Services/CustomerOrderService.cs
namespace StoreBLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using StoreBLL.Interfaces;
    using StoreBLL.Models;

    using StoreDAL.Data;
    using StoreDAL.Entities;
    using StoreDAL.Repository;

    /// <summary>
    /// Business logic service for customer order management with comprehensive logging.
    /// Provides CRUD operations, state transition validation, and inventory management integration.
    /// Implements order workflow with automatic stock reservation/release based on state changes.
    /// </summary>
    public class CustomerOrderService : ICustomerOrderService
    {
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
        private readonly ILogger<CustomerOrderService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderService"/> class.
        /// </summary>
        /// <param name="context">EF Core database context for order operations.</param>
        /// <param name="logger">Logger instance (optional, uses NullLogger if not provided).</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public CustomerOrderService(StoreDbContext context, ILogger<CustomerOrderService>? logger = null)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.repository = new CustomerOrderRepository(context);
            this.stockService = new StockReservationService(context);
            this.logger = logger ?? NullLogger<CustomerOrderService>.Instance;
        }

        /// <summary>
        /// Gets the human-readable name for an order state ID.
        /// </summary>
        /// <param name="id">Order state identifier (1-8).</param>
        /// <returns>Descriptive name of the order state, or "Unknown" if ID is invalid.</returns>
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
        /// Gets the list of allowed next states for a given current state.
        /// </summary>
        /// <param name="currentStateId">Current order state identifier.</param>
        /// <returns>Read-only list of allowed next state IDs, or empty list if no transitions are allowed.</returns>
        public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
            AllowedTransitions.TryGetValue(currentStateId, out var arr)
                ? Array.AsReadOnly(arr)
                : Array.Empty<int>();

        /// <summary>
        /// Checks if a state transition is valid according to the order workflow rules.
        /// </summary>
        /// <param name="fromStateId">Source state identifier.</param>
        /// <param name="toStateId">Target state identifier.</param>
        /// <returns><see langword="true"/> if the transition is allowed; otherwise, <see langword="false"/>.</returns>
        public static bool CanTransition(int fromStateId, int toStateId) =>
            AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

        /// <summary>
        /// Adds a new customer order to the database.
        /// </summary>
        /// <param name="model">Customer order model containing order details.</param>
        /// <exception cref="ArgumentException">Thrown when model is not of type CustomerOrderModel.</exception>
        public void Add(AbstractModel model)
        {
            if (model is not CustomerOrderModel m)
            {
                this.logger.LogError("Invalid model type. Expected CustomerOrderModel, got {ModelType}", model?.GetType().Name ?? "null");
                throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
            }

            var entity = new CustomerOrder(
                id: 0,
                operationTime: m.OperationTime ?? DateTime.UtcNow.ToString("u"),
                userId: m.UserId,
                orderStateId: m.OrderStateId);

            this.repository.Add(entity);
            m.Id = entity.Id;

            this.logger.LogInformation("Order {OrderId} created for user {UserId} with state {StateId}", entity.Id, m.UserId, m.OrderStateId);
        }

        /// <summary>
        /// Deletes a customer order from the database by ID.
        /// </summary>
        /// <param name="modelId">Order identifier to delete.</param>
        /// <remarks>WARNING: This method does not automatically release stock reservations. Ensure reservations are handled before deletion.</remarks>
        public void Delete(int modelId)
        {
            this.logger.LogWarning("Deleting order {OrderId}. Ensure reservations are released manually.", modelId);
            this.repository.DeleteById(modelId);
        }

        /// <summary>
        /// Retrieves all customer orders from the database.
        /// </summary>
        /// <returns>Collection of all customer order models.</returns>
        public IEnumerable<AbstractModel> GetAll() =>
            this.repository.GetAll().Select(o =>
                new CustomerOrderModel(
                    id: o.Id,
                    userId: o.UserId,
                    operationTime: o.OperationTime,
                    orderStateId: o.OrderStateId));

        /// <summary>
        /// Retrieves a customer order by its identifier.
        /// </summary>
        /// <param name="id">Order identifier.</param>
        /// <returns>Customer order model with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when order with specified ID does not exist.</exception>
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
        /// Updates an existing customer order in the database.
        /// </summary>
        /// <param name="model">Customer order model with updated values.</param>
        /// <exception cref="ArgumentException">Thrown when model is not of type CustomerOrderModel.</exception>
        public void Update(AbstractModel model)
        {
            if (model is not CustomerOrderModel m)
            {
                throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
            }

            var entity = this.repository.GetById(m.Id);
            if (entity == null)
            {
                this.logger.LogWarning("Attempted to update non-existent order {OrderId}", m.Id);
                return;
            }

            entity.UserId = m.UserId;
            entity.OperationTime = m.OperationTime ?? entity.OperationTime;
            entity.OrderStateId = m.OrderStateId;
            this.repository.Update(entity);

            this.logger.LogInformation("Order {OrderId} updated", m.Id);
        }

        /// <summary>
        /// Attempts to change the state of an order according to workflow rules.
        /// Automatically handles stock reservations and releases based on state transitions.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="newStateId">Target state identifier.</param>
        /// <param name="error">Output parameter containing error description if transition fails.</param>
        /// <returns><see langword="true"/> if state was changed successfully; otherwise, <see langword="false"/>.</returns>
        public bool TryChangeState(int orderId, int newStateId, out string error)
        {
            error = string.Empty;

            var entity = this.repository.GetById(orderId);
            if (entity == null)
            {
                error = "Order not found.";
                this.logger.LogWarning("State change failed for order {OrderId}: Order not found", orderId);
                return false;
            }

            if (!CanTransition(entity.OrderStateId, newStateId))
            {
                var next = string.Join(", ", GetAllowedNextStates(entity.OrderStateId).Select(StatusName));
                error = $"Transition not allowed. Current: {StatusName(entity.OrderStateId)}. Allowed next: [{next}].";
                this.logger.LogWarning(
                    "Invalid state transition for order {OrderId}: {CurrentState} -> {NewState}",
                    orderId,
                    StatusName(entity.OrderStateId),
                    StatusName(newStateId));
                return false;
            }

            this.logger.LogInformation(
                "Processing state change for order {OrderId}: {CurrentState} -> {NewState}",
                orderId,
                StatusName(entity.OrderStateId),
                StatusName(newStateId));

            // CRITICAL: Process inventory side-effects BEFORE changing state
            if (newStateId == 8)
            {
                this.logger.LogInformation("Confirming delivery for order {OrderId}", orderId);
                this.stockService.ConfirmOrderDelivery(orderId);
            }
            else if (newStateId == 3)
            {
                this.logger.LogInformation("Releasing reservations for cancelled order {OrderId}", orderId);
                this.stockService.ReleaseOrderReservations(orderId);
            }

            // THEN save new state
            entity.OrderStateId = newStateId;
            this.repository.Update(entity);
            this.context.SaveChanges();

            this.logger.LogInformation(
                "Order {OrderId} state changed successfully to {NewState}",
                orderId,
                StatusName(newStateId));

            return true;
        }

        /// <summary>
        /// Allows a user to cancel their own order if it is in "New Order" state.
        /// Automatically releases all stock reservations associated with the order.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="userId">User identifier attempting to cancel the order.</param>
        /// <param name="error">Output parameter containing error description if cancellation fails.</param>
        /// <returns><see langword="true"/> if order was cancelled successfully; otherwise, <see langword="false"/>.</returns>
        public bool CancelOwnOrder(int orderId, int userId, out string error)
        {
            error = string.Empty;

            var entity = this.repository.GetById(orderId);
            if (entity == null)
            {
                error = "Order not found.";
                this.logger.LogWarning("User {UserId} attempted to cancel non-existent order {OrderId}", userId, orderId);
                return false;
            }

            if (entity.UserId != userId)
            {
                error = "You can only cancel your own orders.";
                this.logger.LogWarning(
                    "User {UserId} attempted to cancel order {OrderId} belonging to user {OwnerId}",
                    userId,
                    orderId,
                    entity.UserId);
                return false;
            }

            if (entity.OrderStateId != 1)
            {
                error = "Only new orders can be canceled.";
                this.logger.LogWarning(
                    "User {UserId} attempted to cancel order {OrderId} in state {State}",
                    userId,
                    orderId,
                    StatusName(entity.OrderStateId));
                return false;
            }

            this.logger.LogInformation("User {UserId} cancelling order {OrderId}", userId, orderId);

            // Release reservations first, then switch to state 2
            this.stockService.ReleaseOrderReservations(orderId);

            entity.OrderStateId = 2;
            this.repository.Update(entity);
            this.context.SaveChanges();

            this.logger.LogInformation("Order {OrderId} cancelled by user {UserId}", orderId, userId);
            return true;
        }

        /// <summary>
        /// Allows a user to mark their order as received (confirm delivery).
        /// Only orders in "Delivered to client" state can be confirmed.
        /// Automatically finalizes stock operations when delivery is confirmed.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="userId">User identifier attempting to confirm receipt.</param>
        /// <param name="error">Output parameter containing error description if confirmation fails.</param>
        /// <returns><see langword="true"/> if delivery was confirmed successfully; otherwise, <see langword="false"/>.</returns>
        public bool MarkAsReceived(int orderId, int userId, out string error)
        {
            error = string.Empty;

            var entity = this.repository.GetById(orderId);
            if (entity == null)
            {
                error = "Order not found.";
                this.logger.LogWarning("User {UserId} attempted to confirm non-existent order {OrderId}", userId, orderId);
                return false;
            }

            if (entity.UserId != userId)
            {
                error = "You can only confirm receipt of your own orders.";
                this.logger.LogWarning(
                    "User {UserId} attempted to confirm order {OrderId} belonging to user {OwnerId}",
                    userId,
                    orderId,
                    entity.UserId);
                return false;
            }

            if (entity.OrderStateId != 7)
            {
                error = "Only delivered orders can be marked as received.";
                this.logger.LogWarning(
                    "User {UserId} attempted to confirm order {OrderId} in state {State}",
                    userId,
                    orderId,
                    StatusName(entity.OrderStateId));
                return false;
            }

            this.logger.LogInformation("User {UserId} confirming receipt of order {OrderId}", userId, orderId);

            // CRITICAL: Confirm delivery BEFORE changing state
            this.stockService.ConfirmOrderDelivery(orderId);

            // THEN change to state 8
            entity.OrderStateId = 8;
            this.repository.Update(entity);
            this.context.SaveChanges();

            this.logger.LogInformation("Order {OrderId} marked as received by user {UserId}", orderId, userId);
            return true;
        }

        /// <summary>
        /// Retrieves all orders for a specific user, ordered by ID descending (most recent first).
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Collection of customer order models belonging to the specified user.</returns>
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
}
