// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Services\StockReservationService.cs
namespace StoreBLL.Services;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;

/// <summary>
/// Stock reservations and delivery confirmation helpers.
/// Ensures idempotent operations to prevent double-processing.
/// </summary>
public sealed class StockReservationService
{
    private readonly StoreDbContext context;

    public StockReservationService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Releases reserved quantities for all lines of the order (used on cancel).
    /// Idempotent: safe to call multiple times.
    /// </summary>
    public void ReleaseOrderReservations(int orderId)
    {
        var details = this.context.OrderDetails
            .Where(d => d.OrderId == orderId)
            .ToList();

        if (details.Count == 0)
        {
            return;
        }

        // Fetch needed products in one query
        var pids = details.Select(d => d.ProductId).Distinct().ToList();
        var products = this.context.Products
            .Where(p => pids.Contains(p.Id))
            .ToDictionary(p => p.Id);

        foreach (var d in details)
        {
            if (!products.TryGetValue(d.ProductId, out var p))
            {
                continue;
            }

            // Subtract exactly the order quantity from current reservations
            var newReserved = p.ReservedQuantity - d.ProductAmount;

            // Ensure reservations never go negative
            p.ReservedQuantity = newReserved < 0 ? 0 : newReserved;
        }

        this.context.SaveChanges();
    }

    /// <summary>
    /// Confirms delivery: decreases stock AND clears matching reservations.
    /// IDEMPOTENT: Checks order state to prevent double-processing.
    /// </summary>
    public void ConfirmOrderDelivery(int orderId)
    {
        // CRITICAL: Check if order is already confirmed (state 8)
        var order = this.context.CustomerOrders
            .AsNoTracking()
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            throw new InvalidOperationException($"Order {orderId} not found.");
        }

        // If already confirmed (state 8), skip processing
        // This prevents double decrement of stock
        if (order.OrderStateId == 8)
        {
            // Already processed, do nothing
            return;
        }

        var details = this.context.OrderDetails
            .Where(d => d.OrderId == orderId)
            .ToList();

        if (details.Count == 0)
        {
            return;
        }

        var pids = details.Select(d => d.ProductId).Distinct().ToList();
        var products = this.context.Products
            .Where(p => pids.Contains(p.Id))
            .ToDictionary(p => p.Id);

        foreach (var d in details)
        {
            if (!products.TryGetValue(d.ProductId, out var p))
            {
                continue;
            }

            // 1) Release from reservations (subtract order quantity)
            var newReserved = p.ReservedQuantity - d.ProductAmount;
            p.ReservedQuantity = Math.Max(0, newReserved);

            // 2) Decrease stock (subtract order quantity)
            var newStock = p.StockQuantity - d.ProductAmount;
            p.StockQuantity = Math.Max(0, newStock);
        }

        this.context.SaveChanges();
    }
}
