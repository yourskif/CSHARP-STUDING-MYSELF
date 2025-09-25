// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Services\StockReservationService.cs
namespace StoreBLL.Services;

using System;
using System.Linq;
using StoreDAL.Data;

/// <summary>
/// Stock reservations & delivery confirmation helpers.
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

        // Підтягуємо потрібні продукти одним запитом
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

            // Віднімаємо рівно кількість із поточного замовлення
            var newReserved = p.ReservedQuantity - d.ProductAmount;
            p.ReservedQuantity = newReserved < 0 ? 0 : newReserved;
        }

        this.context.SaveChanges();
    }

    /// <summary>
    /// Confirms delivery: decreases stock AND clears matching reservations (subtracts exactly order qty).
    /// </summary>
    public void ConfirmOrderDelivery(int orderId)
    {
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

            // 1) знімаємо з резерву рівно кількість позиції
            var newReserved = p.ReservedQuantity - d.ProductAmount;
            p.ReservedQuantity = newReserved < 0 ? 0 : newReserved;

            // 2) списуємо склад на ту саму кількість
            var newStock = p.StockQuantity - d.ProductAmount;
            p.StockQuantity = newStock < 0 ? 0 : newStock;
        }

        this.context.SaveChanges();
    }
}
