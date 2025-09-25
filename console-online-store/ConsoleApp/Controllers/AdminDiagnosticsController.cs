// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Controllers\AdminDiagnosticsController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StoreBLL.Services;
using StoreDAL.Data;
using DalCustomerOrder = StoreDAL.Entities.CustomerOrder;
using DalOrderDetail = StoreDAL.Entities.OrderDetail;
using DalProduct = StoreDAL.Entities.Product;
using DalUser = StoreDAL.Entities.User;

// Use ConsoleApp.Helpers factory (has hashing + EnsureDefaultAdmin)
using AppStoreDbFactory = ConsoleApp.Helpers.StoreDbFactory;

/// <summary>
/// Admin diagnostics: inventory snapshot, reserved rebuild/clear, anomaly checks,
/// orders snapshot, user hash checks, default admin reset, demo seed/clear.
/// </summary>
public sealed class AdminDiagnosticsController
{
    // Open order states: 1-New, 4-Confirmed, 5-Moved to delivery, 6-In delivery
    private static readonly int[] OpenStates = { 1, 4, 5, 6 };

    private readonly StoreDbContext db;

    public AdminDiagnosticsController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public void Run()
    {
        var prev = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        try
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADMIN: DIAGNOSTICS ===");
                Console.WriteLine($"UTC Now: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine(new string('-', 78));

                try
                {
                    this.PrintCounts();
                    this.PrintLowAvailable(5);
                    Console.WriteLine(new string('-', 78));
                    Console.WriteLine("[1] Products snapshot");
                    Console.WriteLine("[2] Rebuild Reserved from OPEN orders");
                    Console.WriteLine("[3] CLEAR ALL reservations (demo reset)");
                    Console.WriteLine("[4] Find stock anomalies (Reserved>Stock / Available<0)");
                    Console.WriteLine("[5] RESET DEMO (close open orders + zero reservations)");
                    Console.WriteLine("[6] Orders snapshot (table)");
                    Console.WriteLine("[7] Admin cancel order by ID");
                    Console.WriteLine("[8] Users: Show (hash check)");
                    Console.WriteLine("[9] Users: Reset default admin (admin / Admin@123)");
                    Console.WriteLine("[0] Seed DEMO orders");
                    Console.WriteLine("[C] Clear ALL orders (delete) + reset reservations");
                    Console.WriteLine();
                    Console.WriteLine("[R] Refresh    [Q]/Esc Back");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Application error: {ex.Message}");
                    Console.WriteLine("[R] Refresh    [Q]/Esc Back");
                }

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                {
                    return;
                }

                if (key == ConsoleKey.R)
                {
                    continue;
                }

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.ShowProductsSnapshot();
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.RebuildReservedFromOpenOrders();
                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        this.ClearAllReservations();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        this.ShowAnomalies();
                        break;

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        this.ResetDemo();
                        break;

                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        this.OrdersSnapshot();
                        break;

                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        this.AdminCancelOrderById();
                        break;

                    case ConsoleKey.D8:
                    case ConsoleKey.NumPad8:
                        this.ShowUsersHash();
                        break;

                    case ConsoleKey.D9:
                    case ConsoleKey.NumPad9:
                        this.ResetDefaultAdmin();
                        break;

                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        this.SeedDemoOrders();
                        break;

                    case ConsoleKey.C:
                        this.ClearAllOrders();
                        break;
                }
            }
        }
        finally
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = prev;
        }
    }

    // ---------- small static helpers ----------
    private static PropertyInfo? GetProp(object obj, string name)
    {
        return obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
    }

    private static string Trunc(string? s, int max)
    {
        if (string.IsNullOrEmpty(s) || max <= 0)
        {
            return string.Empty;
        }

        if (s.Length <= max)
        {
            return s;
        }

        var take = Math.Max(0, max - 1);
        return string.Concat(s.AsSpan(0, take), "…"); // ellipsis
    }

    private static string StatusName(int id)
    {
        return id switch
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
    }

    private static string UserLabel(StoreDAL.Entities.User? u)
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

            if (v != null && int.TryParse(v.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
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
                    var converted = Convert.ChangeType(value, pi.PropertyType, CultureInfo.InvariantCulture);
                    pi.SetValue(obj, converted);
                }

                return true;
            }
            catch
            {
                // try next name
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

            if (v != null && decimal.TryParse(v.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
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

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    // ---------- instance helpers / actions ----------
    private void PrintCounts()
    {
        int open = this.db.Set<DalCustomerOrder>().Count(o => OpenStates.Contains(o.OrderStateId));
        int totalOrders = this.db.Set<DalCustomerOrder>().Count();
        int closed = totalOrders - open;

        Console.WriteLine("Overview");
        Console.WriteLine($"Users total........ {this.db.Set<DalUser>().Count()}");
        Console.WriteLine($"Products total..... {this.db.Set<DalProduct>().Count()}");
        Console.WriteLine($"Orders total....... {totalOrders}");
        Console.WriteLine($"Open / Closed...... {open} / {closed}");
        Console.WriteLine();
    }

    private void PrintLowAvailable(int threshold)
    {
        Console.WriteLine($"Low availability (threshold: {threshold})");

        var items = this.db.Set<DalProduct>()
            .Include(p => p.Title)
            .AsEnumerable()
            .Select(p =>
            {
                int stock = GetInt(p, "StockQuantity", "Stock", "Quantity", "QuantityInStock", "UnitsInStock");
                int reserved = GetInt(p, "ReservedQuantity", "Reserved");
                int available = stock - reserved;

                return new
                {
                    p.Id,
                    Title = p.Title != null ? p.Title.Title : $"Product {p.Id}",
                    Stock = stock,
                    Reserved = reserved,
                    Available = available,
                };
            })
            .OrderBy(x => x.Available)
            .ThenBy(x => x.Id)
            .Where(x => x.Available <= threshold)
            .Take(10)
            .ToList();

        if (items.Count == 0)
        {
            Console.WriteLine("  No low availability alerts");
        }
        else
        {
            foreach (var x in items)
            {
                Console.WriteLine($"  #{x.Id,-3} {Trunc(x.Title, 30),-30} | Stock:{x.Stock,5} | Reserved:{x.Reserved,5} | Available:{x.Available,5}");
            }
        }

        Console.WriteLine();
    }

    private void ShowProductsSnapshot()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: PRODUCTS SNAPSHOT ===\n");

        var list = this.db.Set<DalProduct>()
            .Include(p => p.Title)
            .AsEnumerable()
            .Select(p =>
            {
                var sku = GetString(p, "SKU", "Sku", "Code", "Article");
                var price = GetDecimal(p, "UnitPrice", "Price");
                var stock = GetInt(p, "StockQuantity", "Stock", "Quantity", "QuantityInStock", "UnitsInStock");
                var reserved = GetInt(p, "ReservedQuantity", "Reserved");
                var available = stock - reserved;

                return new
                {
                    p.Id,
                    Title = p.Title != null ? p.Title.Title : $"Product {p.Id}",
                    SKU = sku,
                    Price = price,
                    Stock = stock,
                    Reserved = reserved,
                    Available = available,
                };
            })
            .OrderBy(x => x.Id)
            .ToList();

        Console.WriteLine($"{"ID",3}  {"Title",-30}  {"SKU",-14}  {"Price",12}  {"Stock",7}  {"Reserved",9}  {"Available",10}");
        Console.WriteLine(new string('-', 3 + 2 + 30 + 2 + 14 + 2 + 12 + 2 + 7 + 2 + 9 + 2 + 10));

        foreach (var p in list)
        {
            Console.WriteLine($"{p.Id,3}  {Trunc(p.Title, 30),-30}  {Trunc(p.SKU, 14),-14}  {p.Price,12:0.00}  {p.Stock,7}  {p.Reserved,9}  {p.Available,10}");
        }

        Pause();
    }

    private void RebuildReservedFromOpenOrders()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: REBUILD RESERVED FROM OPEN ORDERS ===\n");

        try
        {
            var reservedByProduct =
                (from d in this.db.Set<DalOrderDetail>()
                 join o in this.db.Set<DalCustomerOrder>() on d.OrderId equals o.Id
                 where OpenStates.Contains(o.OrderStateId)
                 group d by d.ProductId into g
                 select new { ProductId = g.Key, Reserved = g.Sum(x => x.ProductAmount) })
                .ToDictionary(x => x.ProductId, x => x.Reserved);

            var products = this.db.Set<DalProduct>().ToList();

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
            Console.WriteLine($"Reserved rebuilt for {updated} product(s).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex.Message}");
        }

        Pause();
    }

    private void ClearAllReservations()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: CLEAR ALL RESERVATIONS ===\n");
        Console.Write("Are you sure? This will set ReservedQuantity=0 for ALL products. [y/N]: ");
        var key = Console.ReadKey(true).Key;
        Console.WriteLine();

        if (key != ConsoleKey.Y)
        {
            Console.WriteLine("Aborted.");
            Pause();
            return;
        }

        try
        {
            int updated = 0;
            foreach (var p in this.db.Set<DalProduct>())
            {
                if (TrySetInt(p, 0, "ReservedQuantity", "Reserved"))
                {
                    updated++;
                }
            }

            this.db.SaveChanges();
            Console.WriteLine($"Cleared reservations for {updated} product(s).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex.Message}");
        }

        Pause();
    }

    private void ShowAnomalies()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: STOCK ANOMALIES ===\n");

        var bad = this.db.Set<DalProduct>()
            .Include(p => p.Title)
            .AsEnumerable()
            .Select(p =>
            {
                var stock = GetInt(p, "StockQuantity", "Stock", "Quantity", "QuantityInStock", "UnitsInStock");
                var reserved = GetInt(p, "ReservedQuantity", "Reserved");
                var available = stock - reserved;

                return new
                {
                    p.Id,
                    Title = p.Title != null ? p.Title.Title : $"Product {p.Id}",
                    Stock = stock,
                    Reserved = reserved,
                    Available = available,
                };
            })
            .Where(x => x.Available < 0 || x.Reserved > x.Stock)
            .OrderBy(x => x.Id)
            .ToList();

        if (bad.Count == 0)
        {
            Console.WriteLine("No anomalies found.");
        }
        else
        {
            foreach (var x in bad)
            {
                Console.WriteLine($"#{x.Id,3}  {Trunc(x.Title, 30),-30} | Stock:{x.Stock,5} | Reserved:{x.Reserved,5} | Available:{x.Available,5}");
            }
        }

        Pause();
    }

    private void ResetDemo()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: RESET DEMO ===");
        Console.WriteLine("This will:");
        Console.WriteLine(" - set all OPEN orders to 'Cancelled by administrator' (state 3)");
        Console.WriteLine(" - set ReservedQuantity=0 for ALL products\n");
        Console.Write("Proceed? [y/N]: ");
        var key = Console.ReadKey(true).Key;
        Console.WriteLine();

        if (key != ConsoleKey.Y)
        {
            Console.WriteLine("Aborted.");
            Pause();
            return;
        }

        int closed = 0;
        var openOrders = this.db.Set<DalCustomerOrder>()
            .Where(o => OpenStates.Contains(o.OrderStateId))
            .ToList();

        var stockSvc = new StockReservationService(this.db);

        foreach (var o in openOrders)
        {
            stockSvc.ReleaseOrderReservations(o.Id);
            o.OrderStateId = 3;
            closed++;
        }

        int zeroed = 0;
        foreach (var p in this.db.Set<DalProduct>())
        {
            if (TrySetInt(p, 0, "ReservedQuantity", "Reserved"))
            {
                zeroed++;
            }
        }

        this.db.SaveChanges();
        Console.WriteLine($"Closed open orders: {closed}");
        Console.WriteLine($"Zeroed reservations for products: {zeroed}");
        Console.WriteLine("Demo state has been reset.");

        Pause();
    }

    private void OrdersSnapshot()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: ORDERS SNAPSHOT ===\n");

        var rows = this.db.Set<DalCustomerOrder>()
            .AsNoTracking()
            .Include(o => o.User)
            .OrderByDescending(o => o.Id)
            .Select(o => new { o.Id, o.OperationTime, o.OrderStateId, o.User })
            .ToList();

        if (rows.Count == 0)
        {
            Console.WriteLine("No orders found.");
            Pause();
            return;
        }

        Console.WriteLine($"{"ID",4}  {"Date",19}  {"User",-20}  {"Status",-28}  {"Total",10}");
        Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 20 + 2 + 28 + 2 + 10));

        foreach (var r in rows)
        {
            decimal total = (decimal)this.db.Set<DalOrderDetail>()
                .Where(d => d.OrderId == r.Id)
                .Select(d => (double)d.Price * d.ProductAmount)
                .Sum();

            Console.WriteLine($"{r.Id,4}  {r.OperationTime,19}  {UserLabel(r.User),-20}  {StatusName(r.OrderStateId),-28}  {total,10:0.00}");
        }

        Console.WriteLine();
        Console.WriteLine("Tip: Use [7] to cancel by ID.");
        Pause();
    }

    private void AdminCancelOrderById()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: ADMIN CANCEL ORDER ===");
        Console.Write("Enter Order ID to cancel: ");

        if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.CurrentCulture, out int id))
        {
            Console.WriteLine("Invalid ID.");
            Pause();
            return;
        }

        var order = this.db.Set<DalCustomerOrder>().FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            Console.WriteLine("Order not found.");
            Pause();
            return;
        }

        if (order.OrderStateId is 2 or 3 or 8)
        {
            Console.WriteLine($"Order already final: {StatusName(order.OrderStateId)}");
            Pause();
            return;
        }

        var stock = new StockReservationService(this.db);
        stock.ReleaseOrderReservations(id);
        order.OrderStateId = 3;
        this.db.SaveChanges();

        Console.WriteLine($"Order {id} cancelled by admin and reservations released.");
        Pause();
    }

    // ---------- NEW: users/hash + admin reset + demo seed/clear ----------
    private void ShowUsersHash()
    {
        Console.Clear();
        Console.WriteLine("=== USERS (hash check) ===\n");

        var users = this.db.Set<DalUser>()
            .OrderBy(u => u.Id)
            .Select(u => new { u.Id, u.Login, u.RoleId, u.Password })
            .Take(100)
            .ToList();

        if (users.Count == 0)
        {
            Console.WriteLine("No users.");
            Pause();
            return;
        }

        Console.WriteLine($"{"ID",4}  {"Login",-16}  {"Role",4}  {"Hash?",-6}  {"Preview",-28}");
        Console.WriteLine(new string('-', 4 + 2 + 16 + 2 + 4 + 2 + 6 + 2 + 28));

        foreach (var u in users)
        {
            bool hashed = u.Password?.StartsWith("PBKDF2$", StringComparison.Ordinal) == true;
            string preview = string.IsNullOrEmpty(u.Password)
                ? string.Empty
                : (u.Password.Length > 28 ? u.Password[..28] : u.Password);

            Console.WriteLine($"{u.Id,4}  {u.Login,-16}  {u.RoleId,4}  {(hashed ? "YES" : "NO"),-6}  {preview,-28}");
        }

        Pause();
    }

    private void ResetDefaultAdmin()
    {
        Console.Clear();
        Console.WriteLine("=== RESET DEFAULT ADMIN ===\n");

        AppStoreDbFactory.EnsureDefaultAdmin(this.db);

        var admin = this.db.Set<DalUser>().FirstOrDefault(u => u.Login == "admin");
        if (admin == null)
        {
            Console.WriteLine("Admin user not found after reset (unexpected).");
        }
        else
        {
            bool hashed = admin.Password?.StartsWith("PBKDF2$", StringComparison.Ordinal) == true;
            Console.WriteLine($"Admin fixed: Id={admin.Id}, RoleId={admin.RoleId}, IsBlocked={admin.IsBlocked}, Hash={(hashed ? "YES" : "NO")}");
            Console.WriteLine("Use credentials: admin / Admin@123");
        }

        Pause();
    }

    private void SeedDemoOrders()
    {
        Console.Clear();
        Console.WriteLine("=== SEED DEMO ORDERS ===\n");

        var stockSvc = new StockReservationService(this.db);

        var u1 = this.db.Set<DalUser>().FirstOrDefault(u => u.RoleId != 1) ?? this.db.Set<DalUser>().First();
        var u2 = this.db.Set<DalUser>().Where(u => u.Id != u1.Id).FirstOrDefault(u => u.RoleId != 1)
                 ?? this.db.Set<DalUser>().OrderBy(u => u.Id).First();

        var p1 = this.db.Set<DalProduct>().OrderBy(p => p.Id).FirstOrDefault();
        var p2 = this.db.Set<DalProduct>().OrderBy(p => p.Id).Skip(1).FirstOrDefault();
        var p3 = this.db.Set<DalProduct>().OrderBy(p => p.Id).Skip(2).FirstOrDefault();

        if (p1 == null || p2 == null)
        {
            Console.WriteLine("Not enough products to seed.");
            Pause();
            return;
        }

        string now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // --- Order A: New Order for u1 with 2 lines ---
        int amtA1 = Math.Min(3, Math.Max(1, p1.StockQuantity - p1.ReservedQuantity));
        int amtA2 = Math.Min(2, Math.Max(1, p2.StockQuantity - p2.ReservedQuantity));

        var a = new DalCustomerOrder
        {
            UserId = u1.Id,
            OperationTime = now,
            OrderStateId = 1, // New
            Details =
            {
                new DalOrderDetail { ProductId = p1.Id, ProductAmount = amtA1, Price = p1.UnitPrice },
                new DalOrderDetail { ProductId = p2.Id, ProductAmount = amtA2, Price = p2.UnitPrice },
            },
        };

        this.db.Add(a);
        this.db.SaveChanges();

        foreach (var d in a.Details)
        {
            var prod = this.db.Set<DalProduct>().Find(d.ProductId);
            if (prod != null)
            {
                prod.ReservedQuantity += d.ProductAmount;
            }
        }

        this.db.SaveChanges();

        // --- Order B: Delivered to client for u2 with 1–2 lines ---
        var detailsB = new List<DalOrderDetail>
        {
            new DalOrderDetail
            {
                ProductId = p2.Id,
                ProductAmount = Math.Min(1, Math.Max(1, p2.StockQuantity - p2.ReservedQuantity)),
                Price = p2.UnitPrice,
            },
        };

        if (p3 != null)
        {
            detailsB.Add(new DalOrderDetail
            {
                ProductId = p3.Id,
                ProductAmount = Math.Min(1, Math.Max(1, p3.StockQuantity - p3.ReservedQuantity)),
                Price = p3.UnitPrice,
            });
        }

        var b = new DalCustomerOrder
        {
            UserId = u2.Id,
            OperationTime = now,
            OrderStateId = 7, // Delivered to client
            Details = detailsB,
        };

        this.db.Add(b);
        this.db.SaveChanges();

        // reduce stock for delivered order
        stockSvc.ConfirmOrderDelivery(b.Id);
        this.db.SaveChanges();

        Console.WriteLine($"Created orders: #{a.Id} (New), #{b.Id} (Delivered).");
        Console.WriteLine("Open Orders snapshot to verify.");
        Pause();
    }

    private void ClearAllOrders()
    {
        Console.Clear();
        Console.WriteLine("=== CLEAR ALL ORDERS ===\n");
        Console.Write("This will DELETE all orders and zero all reservations. Proceed? [y/N]: ");
        var key = Console.ReadKey(true).Key;
        Console.WriteLine();

        if (key != ConsoleKey.Y)
        {
            Console.WriteLine("Aborted.");
            Pause();
            return;
        }

        foreach (var p in this.db.Set<DalProduct>())
        {
            p.ReservedQuantity = 0;
        }

        this.db.Set<DalOrderDetail>().RemoveRange(this.db.Set<DalOrderDetail>());
        this.db.Set<DalCustomerOrder>().RemoveRange(this.db.Set<DalCustomerOrder>());
        this.db.SaveChanges();

        Console.WriteLine("All orders removed. Reservations reset to 0.");
        Pause();
    }
}
