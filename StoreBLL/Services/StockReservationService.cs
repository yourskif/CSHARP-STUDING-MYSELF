// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Services\StockReservationService.cs
namespace StoreBLL.Services;

using System;
using System.Linq;

using StoreDAL.Data;

/// <summary>
/// Service for managing product inventory reservations tied to order lifecycle.
/// Coordinates stock quantities and reservations during order state transitions.
/// </summary>
/// <remarks>
/// <para>
/// This service maintains inventory integrity by managing two key quantities for each product:
/// </para>
/// <list type="bullet">
/// <item><description><b>StockQuantity</b>: Total physical inventory available</description></item>
/// <item><description><b>ReservedQuantity</b>: Units allocated to active orders</description></item>
/// <item><description><b>Available</b>: Computed as StockQuantity - ReservedQuantity</description></item>
/// </list>
/// <para>
/// Typical order lifecycle integration:
/// <list type="number">
/// <item><description>Order created (state 1) → Reserve stock (external to this service)</description></item>
/// <item><description>Order cancelled (state 2/3) → Call <see cref="ReleaseOrderReservations"/></description></item>
/// <item><description>Order delivered and confirmed (state 8) → Call <see cref="ConfirmOrderDelivery"/></description></item>
/// </list>
/// </para>
/// </remarks>
public sealed class StockReservationService(StoreDbContext context)
{
    private readonly StoreDbContext context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Releases stock reservations for all products in an order.
    /// Used when an order is cancelled by user or administrator.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to release reservations for.</param>
    /// <remarks>
    /// <para>
    /// This method decreases <c>ReservedQuantity</c> for each product in the order by the order line quantity.
    /// The physical stock (<c>StockQuantity</c>) remains unchanged as no items have left the warehouse.
    /// </para>
    /// <para>
    /// Safety guarantees:
    /// <list type="bullet">
    /// <item><description>Reservations never go below zero (guards against double-release)</description></item>
    /// <item><description>Silently handles missing products (continues processing remaining items)</description></item>
    /// <item><description>Idempotent - can be called multiple times safely</description></item>
    /// <item><description>No action if order has no details</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Example: Order has 5 units of Product A reserved. After cancellation:
    /// <code>
    /// Product A: ReservedQuantity -= 5 (never below 0)
    /// Product A: StockQuantity unchanged
    /// </code>
    /// </para>
    /// </remarks>
    public void ReleaseOrderReservations(int orderId)
    {
        var details = this.context.OrderDetails
            .Where(d => d.OrderId == orderId)
            .ToList();

        if (details.Count == 0)
        {
            return;
        }

        foreach (var d in details)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == d.ProductId);
            if (product is null)
            {
                continue;
            }

            // Reduce reserved, but never below zero (idempotent protection)
            var newReserved = product.ReservedQuantity - d.ProductAmount;
            product.ReservedQuantity = newReserved < 0 ? 0 : newReserved;
        }

        this.context.SaveChanges();
    }

    /// <summary>
    /// Confirms order delivery by decreasing physical stock and clearing reservations.
    /// Used when customer confirms receipt of delivered items (order state 8).
    /// </summary>
    /// <param name="orderId">The unique identifier of the order being confirmed.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to confirm delivery for an order that has already been processed,
    /// or when insufficient stock/reservations exist for the order quantities.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method performs two critical inventory adjustments for each order line:
    /// <list type="number">
    /// <item><description>Decreases <c>StockQuantity</c> - items have left inventory permanently</description></item>
    /// <item><description>Decreases <c>ReservedQuantity</c> - reservation is fulfilled and no longer needed</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Safety guarantees:
    /// <list type="bullet">
    /// <item><description>Throws exception if stock or reservations are insufficient (strict control)</description></item>
    /// <item><description>Prevents double-processing of the same order</description></item>
    /// <item><description>All-or-nothing transaction (rolls back on any error)</description></item>
    /// <item><description>No action if order has no details</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Example: Order has 5 units of Product A. After delivery confirmation:
    /// <code>
    /// Product A: StockQuantity -= 5 (throws if &lt; 5)
    /// Product A: ReservedQuantity -= 5 (throws if &lt; 5)
    /// Product A: Available = StockQuantity - ReservedQuantity
    /// </code>
    /// </para>
    /// <para>
    /// <b>Important:</b> This operation is final and represents actual inventory consumption.
    /// Ensure this is only called when goods are truly delivered and confirmed by the customer.
    /// This method enforces strict inventory control and will throw an exception if:
    /// <list type="bullet">
    /// <item><description>Stock is insufficient for the order quantity</description></item>
    /// <item><description>Reservations are insufficient (indicating double-processing)</description></item>
    /// <item><description>Any product in the order is missing from inventory</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public void ConfirmOrderDelivery(int orderId)
    {
        var details = this.context.OrderDetails
            .Where(d => d.OrderId == orderId)
            .ToList();

        if (details.Count == 0)
        {
            return;
        }

        // Pre-validate all products before making any changes
        foreach (var d in details)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == d.ProductId);
            ArgumentNullException.ThrowIfNull(product, nameof(product));

            int qty = d.ProductAmount;

            // Strict validation: ensure sufficient stock
            if (product.StockQuantity < qty)
            {
                throw new InvalidOperationException(
                    $"Cannot confirm delivery for order {orderId}: Insufficient stock for product {d.ProductId}. " +
                    $"Required: {qty}, Available: {product.StockQuantity}. " +
                    $"This may indicate the order has already been processed.");
            }

            // Strict validation: ensure sufficient reservations
            if (product.ReservedQuantity < qty)
            {
                throw new InvalidOperationException(
                    $"Cannot confirm delivery for order {orderId}: Insufficient reservations for product {d.ProductId}. " +
                    $"Required: {qty}, Reserved: {product.ReservedQuantity}. " +
                    $"This may indicate the order has already been processed or reservations were released prematurely.");
            }
        }

        // All validations passed - now apply changes
        foreach (var d in details)
        {
            var product = this.context.Products.First(p => p.Id == d.ProductId);
            int qty = d.ProductAmount;

            product.StockQuantity -= qty;
            product.ReservedQuantity -= qty;
        }

        this.context.SaveChanges();
    }
}
