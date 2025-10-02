namespace StoreBLL.Interfaces;

using System.Collections.Generic;

using StoreBLL.Models;

/// <summary>
/// Service interface for customer order operations.
/// Extends basic CRUD functionality with order-specific business logic including state management and user operations.
/// </summary>
public interface ICustomerOrderService : ICrud
{
    /// <summary>
    /// Attempts to change the state of an order with validation of allowed transitions.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="newStateId">The target state identifier to transition to.</param>
    /// <param name="error">Output parameter containing error message if transition fails.</param>
    /// <returns>
    /// <see langword="true"/> if the state was successfully changed;
    /// <see langword="false"/> if the transition is not allowed or order not found.
    /// </returns>
    /// <remarks>
    /// State transitions must follow the allowed workflow defined in business rules.
    /// See documentation for valid state transition diagram.
    /// </remarks>
    bool TryChangeState(int orderId, int newStateId, out string error);

    /// <summary>
    /// Cancels an order owned by a specific user.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to cancel.</param>
    /// <param name="userId">The unique identifier of the user requesting cancellation.</param>
    /// <param name="error">Output parameter containing error message if cancellation fails.</param>
    /// <returns>
    /// <see langword="true"/> if the order was successfully cancelled;
    /// <see langword="false"/> if cancellation is not allowed or validation fails.
    /// </returns>
    /// <remarks>
    /// Only orders in "New Order" state (state 1) can be cancelled by users.
    /// Users can only cancel their own orders.
    /// </remarks>
    bool CancelOwnOrder(int orderId, int userId, out string error);

    /// <summary>
    /// Marks an order as received by the customer.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <param name="userId">The unique identifier of the user confirming receipt.</param>
    /// <param name="error">Output parameter containing error message if operation fails.</param>
    /// <returns>
    /// <see langword="true"/> if the order was successfully marked as received;
    /// <see langword="false"/> if operation is not allowed or validation fails.
    /// </returns>
    /// <remarks>
    /// Only orders in "Delivered to client" state (state 7) can be marked as received.
    /// Users can only confirm receipt of their own orders.
    /// This action transitions the order to "Delivery confirmed by client" state (state 8).
    /// </remarks>
    bool MarkAsReceived(int orderId, int userId, out string error);

    /// <summary>
    /// Retrieves all orders for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Collection of orders belonging to the specified user, ordered by ID descending.</returns>
    IEnumerable<CustomerOrderModel> GetOrdersByUser(int userId);
}
