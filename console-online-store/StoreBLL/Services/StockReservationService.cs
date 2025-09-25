// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\console-online-store\StoreBLL\Services\StockReservationService.cs
namespace StoreBLL.Services;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Inventory operations tied to order lifecycle:
/// - Release reservations on cancel
/// - Confirm delivery: decrease stock and clear reservations
/// </summary>
public class StockReservationService
{
    private readonly StoreDbContext context;

    public StockReservationService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Releases reserved quantities for all products in the order.
    /// Used when order is cancelled (user/admin).
    /// </summary>
    /// <param name="orderId">Order id.</param>
    public void ReleaseOrderReservations(int orderId)
    {
        var details = this.context.OrderDetails
            .AsNoTracking()
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

            // Reduce reserved, but never below zero
            var newReserved = product.ReservedQuantity - d.ProductAmount;
            product.ReservedQuantity = newReserved < 0 ? 0 : newReserved;
        }

        this.context.SaveChanges();
    }

    /// <summary>
    /// Confirms delivery: for each order line
    /// - decrease stock by shipped amount
    /// - decrease (clear) reservations by shipped amount
    /// Tests expect reservations to be zeroed after moving to state 8.
    /// </summary>
    /// <param name="orderId">Order id.</param>
    public void ConfirmOrderDelivery(int orderId)
    {
        var details = this.context.OrderDetails
            .AsNoTracking()
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

            int qty = d.ProductAmount;

            // Decrease stock (can't go below zero)
            int newStock = product.StockQuantity - qty;
            product.StockQuantity = newStock < 0 ? 0 : newStock;

            // Decrease reserved (can't go below zero). After successful delivery,
            // tests expect reservations to be fully released for shipped items.
            int newReserved = product.ReservedQuantity - qty;
            product.ReservedQuantity = newReserved < 0 ? 0 : newReserved;
        }

        this.context.SaveChanges();
    }
}
