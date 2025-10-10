// Path: console-online-store/StoreBLL/Services/InventoryDiagnosticsService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

/// <summary>
/// Service for inventory diagnostics and stock management operations.
/// Provides functionality for product snapshots, reservation management, and anomaly detection.
/// </summary>
public sealed class InventoryDiagnosticsService
{
    private readonly IStoreUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryDiagnosticsService"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public InventoryDiagnosticsService(IStoreUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public IEnumerable<ProductSnapshot> GetProductsSnapshot()
    {
        return this.unitOfWork.Context.Products
            .Select(p => new ProductSnapshot
            {
                Id = p.Id,
                Title = p.Title != null ? p.Title.Title ?? string.Empty : $"Product {p.Id}",
                SKU = GetString(p, "SKU", "Sku", "Code", "Article"),
                Price = GetDecimal(p, "UnitPrice", "Price"),
                Stock = GetInt(p, "StockQuantity", "Stock", "Quantity", "QuantityInStock", "UnitsInStock"),
                Reserved = GetInt(p, "ReservedQuantity", "Reserved"),
            })
            .OrderBy(x => x.Id)
            .ToList();
    }

    public IEnumerable<ProductSnapshot> GetLowAvailability(int threshold)
    {
        return this.GetProductsSnapshot()
            .Where(p => p.Available <= threshold)
            .OrderBy(p => p.Available)
            .ThenBy(p => p.Id)
            .Take(10);
    }

    public int RebuildReservedFromOpenOrders()
    {
        var openStates = new[] { 1, 4, 5, 6 };

        var reservedByProduct = (
            from d in this.unitOfWork.Context.OrderDetails
            join o in this.unitOfWork.Context.CustomerOrders on d.OrderId equals o.Id
            where openStates.Contains(o.OrderStateId)
            group d by d.ProductId into g
            select new { ProductId = g.Key, Reserved = g.Sum(x => x.ProductAmount) })
            .ToDictionary(x => x.ProductId, x => x.Reserved);

        var products = this.unitOfWork.Context.Products.ToList();
        int updated = 0;

        foreach (var p in products)
        {
            var newReserved = reservedByProduct.TryGetValue(p.Id, out var r) ? r : 0;

            if (TrySetInt(p, newReserved, "ReservedQuantity", "Reserved"))
            {
                updated++;
            }
        }

        this.unitOfWork.SaveChanges();
        return updated;
    }

    public int ClearAllReservations()
    {
        int updated = 0;
        foreach (var p in this.unitOfWork.Context.Products)
        {
            if (TrySetInt(p, 0, "ReservedQuantity", "Reserved"))
            {
                updated++;
            }
        }

        this.unitOfWork.SaveChanges();
        return updated;
    }

    public IEnumerable<ProductSnapshot> GetAnomalies()
    {
        return this.GetProductsSnapshot()
            .Where(x => x.Available < 0 || x.Reserved > x.Stock)
            .OrderBy(x => x.Id);
    }

    public int GetTotalProductCount()
    {
        return this.unitOfWork.Context.Products.Count();
    }

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

    public class ProductSnapshot
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int Reserved { get; set; }

        public int Available => this.Stock - this.Reserved;
    }
}
