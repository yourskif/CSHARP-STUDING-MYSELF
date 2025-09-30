// Path: console-online-store/StoreBLL/Services/InventoryDiagnosticsService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for inventory diagnostics and stock management operations.
/// Provides functionality for product snapshots, reservation management, and anomaly detection.
/// </summary>
public sealed class InventoryDiagnosticsService
{
    private readonly StoreDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryDiagnosticsService"/> class.
    /// </summary>
    /// <param name="db">Database context for inventory operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    public InventoryDiagnosticsService(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Gets snapshot of all products with inventory details.
    /// </summary>
    /// <returns>Enumerable of product snapshots with stock, reserved, and available quantities.</returns>
    public IEnumerable<ProductSnapshot> GetProductsSnapshot()
    {
        return this.db.Products
            .Select(p => new ProductSnapshot
            {
                Id = p.Id,
                Title = p.Title != null ? p.Title.Title : $"Product {p.Id}",
                SKU = GetString(p, "SKU", "Sku", "Code", "Article"),
                Price = GetDecimal(p, "UnitPrice", "Price"),
                Stock = GetInt(p, "StockQuantity", "Stock", "Quantity", "QuantityInStock", "UnitsInStock"),
                Reserved = GetInt(p, "ReservedQuantity", "Reserved"),
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    /// <summary>
    /// Gets products with low availability based on threshold.
    /// </summary>
    /// <param name="threshold">Minimum available quantity threshold.</param>
    /// <returns>Enumerable of products with available quantity at or below threshold.</returns>
    public IEnumerable<ProductSnapshot> GetLowAvailability(int threshold)
    {
        return this.GetProductsSnapshot()
            .Where(p => p.Available <= threshold)
            .OrderBy(p => p.Available)
            .ThenBy(p => p.Id)
            .Take(10);
    }

    /// <summary>
    /// Rebuilds reserved quantities from open orders.
    /// Recalculates reservation counters based on actual order details.
    /// </summary>
    /// <returns>Number of products updated.</returns>
    public int RebuildReservedFromOpenOrders()
    {
        var openStates = new[] { 1, 4, 5, 6 }; // New, Confirmed, Moved to delivery, In delivery

        var reservedByProduct = (
            from d in this.db.OrderDetails
            join o in this.db.CustomerOrders on d.OrderId equals o.Id
            where openStates.Contains(o.OrderStateId)
            group d by d.ProductId into g
            select new { ProductId = g.Key, Reserved = g.Sum(x => x.ProductAmount) })
            .ToDictionary(x => x.ProductId, x => x.Reserved);

        var products = this.db.Products.ToList();
        int updated = 0;

        foreach (var p in products)
        {
            var newReserved = reservedByProduct.TryGetValue(p.Id, out var r) ? r : 0;

            if (TrySetInt(p, newReserved, "ReservedQuantity", "Reserved"))
            {
                updated++;
            }
        }

        this.db.SaveChanges();
        return updated;
    }

    /// <summary>
    /// Clears all product reservations setting reserved quantity to zero.
    /// </summary>
    /// <returns>Number of products updated.</returns>
    public int ClearAllReservations()
    {
        int updated = 0;
        foreach (var p in this.db.Products)
        {
            if (TrySetInt(p, 0, "ReservedQuantity", "Reserved"))
            {
                updated++;
            }
        }

        this.db.SaveChanges();
        return updated;
    }

    /// <summary>
    /// Gets products with stock anomalies (negative available or reserved exceeds stock).
    /// </summary>
    /// <returns>Enumerable of products with inventory anomalies.</returns>
    public IEnumerable<ProductSnapshot> GetAnomalies()
    {
        return this.GetProductsSnapshot()
            .Where(x => x.Available < 0 || x.Reserved > x.Stock)
            .OrderBy(x => x.Id);
    }

    /// <summary>
    /// Gets total count of products in inventory.
    /// </summary>
    /// <returns>Total product count.</returns>
    public int GetTotalProductCount()
    {
        return this.db.Products.Count();
    }

    // -------- Private helpers --------

    private static int GetInt(object obj, params string[] names)
    {
        foreach (var n in names)
        {
            var pi = GetProp(obj, n);
            if (pi == null)
            {
                continue;
            }

            var v = pi.GetValue(obj);
            if (v is int i)
            {
                return i;
            }

            if (v != null && int.TryParse(v.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return 0;
    }

    private static bool TrySetInt(object obj, int value, params string[] names)
    {
        foreach (var n in names)
        {
            var pi = GetProp(obj, n);
            if (pi == null || !pi.CanWrite)
            {
                continue;
            }

            try
            {
                if (pi.PropertyType == typeof(int))
                {
                    pi.SetValue(obj, value);
                }
                else
                {
                    pi.SetValue(obj, Convert.ChangeType(value, pi.PropertyType));
                }

                return true;
            }
            catch
            {
                // Try next name
            }
        }

        return false;
    }

    private static decimal GetDecimal(object obj, params string[] names)
    {
        foreach (var n in names)
        {
            var pi = GetProp(obj, n);
            if (pi == null)
            {
                continue;
            }

            var v = pi.GetValue(obj);
            if (v is decimal d)
            {
                return d;
            }

            if (v is double f)
            {
                return (decimal)f;
            }

            if (v != null && decimal.TryParse(v.ToString(), out var parsed))
            {
                return parsed;
            }
        }

        return 0m;
    }

    private static string GetString(object obj, params string[] names)
    {
        foreach (var n in names)
        {
            var pi = GetProp(obj, n);
            if (pi == null)
            {
                continue;
            }

            var v = pi.GetValue(obj)?.ToString();
            if (!string.IsNullOrWhiteSpace(v))
            {
                return v!;
            }
        }

        return string.Empty;
    }

    private static PropertyInfo? GetProp(object obj, string name)
    {
        return obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
    }

    /// <summary>
    /// Represents product snapshot with inventory details.
    /// </summary>
    public class ProductSnapshot
    {
        /// <summary>
        /// Gets or sets product ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets product title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets product SKU.
        /// </summary>
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets product price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets total stock quantity.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets reserved quantity.
        /// </summary>
        public int Reserved { get; set; }

        /// <summary>
        /// Gets available quantity (Stock - Reserved).
        /// </summary>
        public int Available => this.Stock - this.Reserved;
    }
}
