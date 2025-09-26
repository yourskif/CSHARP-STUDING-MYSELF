// Path: StoreBLL/Patterns/OrderStateStrategy.cs
namespace StoreBLL.Patterns;

using System;
using StoreBLL.Services;
using StoreDAL.Data;

/// <summary>
/// Strategy pattern for handling order state transitions.
/// Encapsulates the logic for different order state changes.
/// </summary>
public interface IOrderStateStrategy
{
    /// <summary>
    /// Executes the state transition logic.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <param name="context">Database context.</param>
    /// <returns>True if transition successful.</returns>
    bool Execute(int orderId, StoreDbContext context);

    /// <summary>
    /// Validates if the transition is allowed.
    /// </summary>
    /// <param name="currentState">Current order state.</param>
    /// <returns>True if transition is allowed.</returns>
    bool CanExecute(int currentState);
}

/// <summary>
/// Strategy for confirming an order (New -> Confirmed).
/// </summary>
public class ConfirmOrderStrategy : IOrderStateStrategy
{
    /// <inheritdoc/>
    public bool Execute(int orderId, StoreDbContext context)
    {
        var order = context.CustomerOrders.Find(orderId);
        if (order == null) return false;

        order.OrderStateId = 4; // Confirmed
        context.SaveChanges();
        return true;
    }

    /// <inheritdoc/>
    public bool CanExecute(int currentState) => currentState == 1; // From New only
}

/// <summary>
/// Strategy for cancelling an order by user.
/// </summary>
public class UserCancelStrategy : IOrderStateStrategy
{
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCancelStrategy"/> class.
    /// </summary>
    /// <param name="stockService">Stock reservation service.</param>
    public UserCancelStrategy(StockReservationService stockService)
    {
        this.stockService = stockService;
    }

    /// <inheritdoc/>
    public bool Execute(int orderId, StoreDbContext context)
    {
        var order = context.CustomerOrders.Find(orderId);
        if (order == null) return false;

        // Release stock reservations
        this.stockService.ReleaseOrderReservations(orderId);

        order.OrderStateId = 2; // Cancelled by user
        context.SaveChanges();
        return true;
    }

    /// <inheritdoc/>
    public bool CanExecute(int currentState) => currentState == 1; // From New only
}

/// <summary>
/// Strategy for admin cancellation with more permissions.
/// </summary>
public class AdminCancelStrategy : IOrderStateStrategy
{
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminCancelStrategy"/> class.
    /// </summary>
    /// <param name="stockService">Stock reservation service.</param>
    public AdminCancelStrategy(StockReservationService stockService)
    {
        this.stockService = stockService;
    }

    /// <inheritdoc/>
    public bool Execute(int orderId, StoreDbContext context)
    {
        var order = context.CustomerOrders.Find(orderId);
        if (order == null) return false;

        // Release stock reservations
        this.stockService.ReleaseOrderReservations(orderId);

        order.OrderStateId = 3; // Cancelled by administrator
        context.SaveChanges();
        return true;
    }

    /// <inheritdoc/>
    public bool CanExecute(int currentState) => currentState is 1 or 4; // From New or Confirmed
}

/// <summary>
/// Strategy for delivery confirmation.
/// </summary>
public class DeliveryConfirmationStrategy : IOrderStateStrategy
{
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeliveryConfirmationStrategy"/> class.
    /// </summary>
    /// <param name="stockService">Stock reservation service.</param>
    public DeliveryConfirmationStrategy(StockReservationService stockService)
    {
        this.stockService = stockService;
    }

    /// <inheritdoc/>
    public bool Execute(int orderId, StoreDbContext context)
    {
        var order = context.CustomerOrders.Find(orderId);
        if (order == null) return false;

        order.OrderStateId = 8; // Delivery confirmed by client
        context.SaveChanges();

        // Confirm stock changes
        this.stockService.ConfirmOrderDelivery(orderId);
        return true;
    }

    /// <inheritdoc/>
    public bool CanExecute(int currentState) => currentState == 7; // From Delivered only
}

/// <summary>
/// Context for executing order state strategies.
/// </summary>
public class OrderStateContext
{
    private readonly StoreDbContext context;
    private IOrderStateStrategy? strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStateContext"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public OrderStateContext(StoreDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Sets the strategy to execute.
    /// </summary>
    /// <param name="strategy">Strategy implementation.</param>
    public void SetStrategy(IOrderStateStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <summary>
    /// Executes the current strategy.
    /// </summary>
    /// <param name="orderId">Order identifier.</param>
    /// <returns>True if execution successful.</returns>
    /// <exception cref="InvalidOperationException">When no strategy is set.</exception>
    public bool ExecuteStrategy(int orderId)
    {
        if (this.strategy == null)
        {
            throw new InvalidOperationException("No strategy set");
        }

        return this.strategy.Execute(orderId, this.context);
    }
}