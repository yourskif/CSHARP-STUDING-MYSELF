using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Entities;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Admin orders management (tabular view, details, cancel, advance).
    /// </summary>
    public sealed class AdminOrderController
    {
        private readonly StoreDbContext db;
        private readonly StockReservationService stockService;

        // Typical pipeline for demo
        private static readonly int[] Pipeline = { 1, 4, 5, 6, 7, 8 };

        public AdminOrderController(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.stockService = new StockReservationService(db);
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
                    Console.WriteLine("=== ADMIN: ORDERS ===\n");
                    Console.WriteLine("1. Orders snapshot (table)");
                    Console.WriteLine("2. View order details");
                    Console.WriteLine("3. Cancel order (admin)");
                    Console.WriteLine("4. Advance order status along pipeline");
                    Console.WriteLine();
                    Console.WriteLine("Esc: Back");

                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            this.ShowOrdersSnapshot();
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            this.ShowOrderDetails();
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            this.AdminCancelOrder();
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            this.AdvanceOrder();
                            break;
                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = prev;
            }
        }

        // For backward compatibility with any menu calling ShowOrders()
        public void ShowOrders() => this.ShowOrdersSnapshot();

        private void ShowOrdersSnapshot()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: ORDERS SNAPSHOT ===\n");

            var rows = this.db.CustomerOrders
                .AsNoTracking()
                .Include(o => o.User)
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.OperationTime,
                    o.OrderStateId,
                    o.User
                })
                .ToList();

            if (rows.Count == 0)
            {
                Console.WriteLine("No orders found.");
                this.Pause();
                return;
            }

            Console.WriteLine($"{"ID",4}  {"Date",19}  {"User",-20}  {"Status",-28}  {"Total",10}");
            Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 20 + 2 + 28 + 2 + 10));

            foreach (var r in rows)
            {
                var total = (decimal)this.db.OrderDetails
                    .Where(d => d.OrderId == r.Id)
                    .Select(d => (double)d.Price * d.ProductAmount)
                    .Sum();

                Console.WriteLine($"{r.Id,4}  {r.OperationTime,19}  {UserLabel(r.User),-20}  {StatusName(r.OrderStateId),-28}  {total,10:0.00}");
            }

            Console.WriteLine("\nTip: Use [3] to cancel by ID, [4] to advance status.");
            this.Pause();
        }

        private void ShowOrderDetails()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: ORDER DETAILS ===\n");
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                this.Pause();
                return;
            }

            var order = this.db.CustomerOrders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                Console.WriteLine("Order not found.");
                this.Pause();
                return;
            }

            Console.WriteLine($"\nID: {order.Id}");
            Console.WriteLine($"Date: {order.OperationTime}");
            Console.WriteLine($"User: {UserLabel(order.User)}");
            Console.WriteLine($"Status: {StatusName(order.OrderStateId)}");

            var details = this.db.OrderDetails
                .Include(d => d.Product).ThenInclude(p => p.Title)
                .Where(d => d.OrderId == id)
                .ToList();

            if (details.Count == 0)
            {
                Console.WriteLine("\nNo items.");
                this.Pause();
                return;
            }

            Console.WriteLine("\nItems:");
            Console.WriteLine($"{"Product",-30}  {"Price",10}  {"Qty",5}  {"Subtotal",10}");
            Console.WriteLine(new string('-', 30 + 2 + 10 + 2 + 5 + 2 + 10));

            decimal total = 0m;
            foreach (var d in details)
            {
                string name = d.Product?.Title?.Title ?? $"Product {d.ProductId}";
                decimal sub = d.Price * d.ProductAmount;
                total += sub;
                Console.WriteLine($"{Trunc(name, 30),-30}  {d.Price,10:0.00}  {d.ProductAmount,5}  {sub,10:0.00}");
            }

            Console.WriteLine(new string('-', 30 + 2 + 10 + 2 + 5 + 2 + 10));
            Console.WriteLine($"{"TOTAL",-30}  {"",10}  {"",5}  {total,10:0.00}");

            this.Pause();
        }

        private void AdminCancelOrder()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: CANCEL ORDER ===\n");
            Console.Write("Enter Order ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                this.Pause();
                return;
            }

            var order = this.db.CustomerOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                this.Pause();
                return;
            }

            if (order.OrderStateId is 3 or 2 or 8)
            {
                Console.WriteLine($"Order already final: {StatusName(order.OrderStateId)}");
                this.Pause();
                return;
            }

            this.stockService.ReleaseOrderReservations(id);
            order.OrderStateId = 3; // cancelled by admin
            this.db.SaveChanges();

            Console.WriteLine($"Order {id} cancelled by administrator. Reservations released.");
            this.Pause();
        }

        private void AdvanceOrder()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: ADVANCE ORDER ===\n");
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                this.Pause();
                return;
            }

            var order = this.db.CustomerOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                this.Pause();
                return;
            }

            var idx = Array.IndexOf(Pipeline, order.OrderStateId);
            if (idx < 0 || idx == Pipeline.Length - 1)
            {
                Console.WriteLine($"Nothing to advance. Current status: {StatusName(order.OrderStateId)}");
                this.Pause();
                return;
            }

            order.OrderStateId = Pipeline[idx + 1];

            // When client confirms delivery, we can finalize reservations if щось ще залишилось
            if (order.OrderStateId == 8)
            {
                var stockSvc = new StockReservationService(this.db);
                stockSvc.ConfirmOrderDelivery(order.Id);
            }

            this.db.SaveChanges();
            Console.WriteLine($"Order {id} moved to: {StatusName(order.OrderStateId)}");
            this.Pause();
        }

        // ---------- helpers ----------

        private static string StatusName(int id) =>
            id switch
            {
                1 => "New Order",
                2 => "Cancelled by user",
                3 => "Cancelled by administrator",
                4 => "Confirmed",
                5 => "Moved to delivery company",
                6 => "In delivery",
                7 => "Delivered to client",
                8 => "Delivery confirmed by client",
                _ => "Unknown"
            };

        private static string UserLabel(User? u)
        {
            if (u is null) return "unknown";
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
            if (pi is null || !pi.CanRead) return null;
            return pi.GetValue(obj) as string;
        }

        private static string Trunc(string? s, int max) =>
            string.IsNullOrEmpty(s) ? string.Empty : (s!.Length <= max ? s : s.Substring(0, max - 1) + "…");

        private void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
