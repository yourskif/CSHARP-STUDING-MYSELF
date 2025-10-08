// Path: console-online-store/StoreBLL/Services/OrderDiagnosticsService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

/// <summary>
/// Service for order diagnostics and testing operations.
/// Provides functionality for order snapshots, demo data seeding, and cleanup.
/// </summary>
public sealed class OrderDiagnosticsService
{
    private readonly IStoreUnitOfWork unitOfWork;
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderDiagnosticsService"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public OrderDiagnosticsService(IStoreUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.stockService = new StockReservationService(unitOfWork);
    }

    /// <summary>
    /// Gets snapshot of all orders with basic information.
    /// </summary>
    /// <returns>Enumerable of order snapshots.</returns>
    public IEnumerable<OrderSnapshot> GetOrdersSnapshot()
    {
        var rows = this.unitOfWork.Context.CustomerOrders
            .AsNoTracking()
            .Include(o => o.User)
            .OrderByDescending(o => o.Id)
            .ToList();

        return rows.Select(r => new OrderSnapshot
        {
            Id = r.Id,
            Date = r.OperationTime,
            OrderStateId = r.OrderStateId,
            UserName = GetUserLabel(r.User),
            Total = (decimal)this.unitOfWork.Context.OrderDetails
                .Where(d => d.OrderId == r.Id)
                .Select(d => (double)d.Price * d.ProductAmount)
                .Sum(),
        });
    }

    /// <summary>
    /// Cancels order by ID and releases reservations.
    /// </summary>
    /// <param name="orderId">Order ID to cancel.</param>
    /// <returns>True if cancelled successfully; false otherwise.</returns>
    public bool CancelOrderById(int orderId)
    {
        var order = this.unitOfWork.Context.CustomerOrders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return false;
        }

        if (order.OrderStateId is 2 or 3 or 8)
        {
            return false;
        }

        this.stockService.ReleaseOrderReservations(orderId);
        order.OrderStateId = 3;
        this.unitOfWork.SaveChanges();

        return true;
    }

    /// <summary>
    /// Seeds demo orders for testing purposes.
    /// Creates sample orders with different states.
    /// </summary>
    /// <returns>Count of orders created.</returns>
    public int SeedDemoOrders()
    {
        var u1 = this.unitOfWork.Context.Users.FirstOrDefault(u => u.RoleId != 1) ?? this.unitOfWork.Context.Users.First();
        var u2 = this.unitOfWork.Context.Users.Where(u => u.Id != u1.Id).FirstOrDefault(u => u.RoleId != 1)
                 ?? this.unitOfWork.Context.Users.OrderBy(u => u.Id).First();

        var p1 = this.unitOfWork.Context.Products.OrderBy(p => p.Id).FirstOrDefault();
        var p2 = this.unitOfWork.Context.Products.OrderBy(p => p.Id).Skip(1).FirstOrDefault();
        var p3 = this.unitOfWork.Context.Products.OrderBy(p => p.Id).Skip(2).FirstOrDefault();

        if (p1 == null || p2 == null)
        {
            return 0;
        }

        string now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        int amtA1 = Math.Min(3, Math.Max(1, p1.StockQuantity - p1.ReservedQuantity));
        int amtA2 = Math.Min(2, Math.Max(1, p2.StockQuantity - p2.ReservedQuantity));

        var orderA = new CustomerOrder
        {
            UserId = u1.Id,
            OperationTime = now,
            OrderStateId = 1,
            Details =
            {
                new OrderDetail { ProductId = p1.Id, ProductAmount = amtA1, Price = p1.UnitPrice },
                new OrderDetail { ProductId = p2.Id, ProductAmount = amtA2, Price = p2.UnitPrice },
            },
        };

        this.unitOfWork.Context.CustomerOrders.Add(orderA);
        this.unitOfWork.SaveChanges();

        foreach (var d in orderA.Details)
        {
            var prod = this.unitOfWork.Context.Products.Find(d.ProductId);
            if (prod != null)
            {
                prod.ReservedQuantity += d.ProductAmount;
            }
        }

        var detailsB = new List<OrderDetail>
        {
            new OrderDetail
            {
                ProductId = p2.Id,
                ProductAmount = Math.Min(1, Math.Max(1, p2.StockQuantity - p2.ReservedQuantity)),
                Price = p2.UnitPrice,
            },
        };

        if (p3 != null)
        {
            detailsB.Add(new OrderDetail
            {
                ProductId = p3.Id,
                ProductAmount = Math.Min(1, Math.Max(1, p3.StockQuantity - p3.ReservedQuantity)),
                Price = p3.UnitPrice,
            });
        }

        var orderB = new CustomerOrder
        {
            UserId = u2.Id,
            OperationTime = now,
            OrderStateId = 7,
            Details = detailsB,
        };

        this.unitOfWork.Context.CustomerOrders.Add(orderB);
        this.unitOfWork.SaveChanges();

        this.stockService.ConfirmOrderDelivery(orderB.Id);
        this.unitOfWork.SaveChanges();

        return 2;
    }

    /// <summary>
    /// Clears all orders and resets reservations.
    /// WARNING: This deletes all order data.
    /// </summary>
    public void ClearAllOrders()
    {
        foreach (var p in this.unitOfWork.Context.Products)
        {
            p.ReservedQuantity = 0;
        }

        this.unitOfWork.Context.OrderDetails.RemoveRange(this.unitOfWork.Context.OrderDetails);
        this.unitOfWork.Context.CustomerOrders.RemoveRange(this.unitOfWork.Context.CustomerOrders);
        this.unitOfWork.SaveChanges();
    }

    /// <summary>
    /// Resets demo environment by closing open orders and clearing reservations.
    /// </summary>
    /// <returns>Tuple with count of closed orders and zeroed products.</returns>
    public (int ClosedOrders, int ZeroedProducts) ResetDemo()
    {
        var openStates = new[] { 1, 4, 5, 6 };
        var openOrders = this.unitOfWork.Context.CustomerOrders
            .Where(o => openStates.Contains(o.OrderStateId))
            .ToList();

        foreach (var o in openOrders)
        {
            this.stockService.ReleaseOrderReservations(o.Id);
            o.OrderStateId = 3;
        }

        int closed = openOrders.Count;

        int zeroed = this.unitOfWork.Context.Products
            .AsEnumerable()
            .Count(p => TrySetInt(p, 0, "ReservedQuantity", "Reserved"));

        this.unitOfWork.SaveChanges();
        return (closed, zeroed);
    }

    /// <summary>
    /// Gets counts of open and closed orders.
    /// </summary>
    /// <returns>Tuple with open and closed order counts.</returns>
    public (int Open, int Closed) GetOrderCounts()
    {
        var openStates = new[] { 1, 4, 5, 6 };
        int open = this.unitOfWork.Context.CustomerOrders.Count(o => openStates.Contains(o.OrderStateId));
        int total = this.unitOfWork.Context.CustomerOrders.Count();
        return (open, total - open);
    }

    private static string GetUserLabel(User? u)
    {
        if (u is null)
        {
            return "unknown";
        }

        string? best =
            ReadString(u, "DisplayName") ??
            ReadString(u, "Email") ??
            ReadString(u, "Login") ??
            ReadString(u, "Username") ??
            ReadString(u, "Name");

        return string.IsNullOrWhiteSpace(best) ? $"User#{u.Id}" : best!;
    }

    private static string? ReadString(object obj, string propName)
    {
        var pi = obj.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (pi is null || !pi.CanRead)
        {
            return null;
        }

        return pi.GetValue(obj) as string;
    }

    private static bool TrySetInt(object obj, int value, params string[] names)
    {
        foreach (var n in names)
        {
            var pi = obj.GetType().GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
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

    /// <summary>
    /// Represents order snapshot with basic information.
    /// </summary>
    public class OrderSnapshot
    {
        /// <summary>
        /// Gets or sets order ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets order date.
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets order state ID.
        /// </summary>
        public int OrderStateId { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets order total amount.
        /// </summary>
        public decimal Total { get; set; }
    }
}
