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
    /// Admin orders management (tabular view, details, cancel, change status).
    /// </summary>
    public sealed class AdminOrderController
    {
        // ---------- instance fields ----------
        private readonly StoreDbContext db;
        private readonly StockReservationService stockService;
        private readonly CustomerOrderService orderService;

        // ---------- ctor ----------
        public AdminOrderController(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.stockService = new StockReservationService(db);
            this.orderService = new CustomerOrderService(db);
        }

        // ---------- PUBLIC methods (SA1202: public before private) ----------
        /// <summary>
        /// Backward-compat alias used by existing menus.
        /// </summary>
        public void ShowOrders() => this.ShowOrdersSnapshot();

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
                    Console.WriteLine("4. Change order status (choose allowed)");
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
                            this.ChangeOrderStatusInteractive();
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

        // ---------- PRIVATE static helpers (before private instance methods; SA1204 within 'private') ----------
        private static string UserLabel(User? u)
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

        private static string? ReadString(object? obj, string propName)
        {
            if (obj is null)
            {
                return null;
            }

            var pi = obj.GetType().GetProperty(
                propName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (pi is null || !pi.CanRead)
            {
                return null;
            }

            return pi.GetValue(obj) as string;
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

            // ASCII "..." to avoid any encoding surprises
            return string.Concat(s.AsSpan(0, take), "...");
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        // ---------- PRIVATE instance methods ----------
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
                    o.User,
                })
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
                var total = (decimal)this.db.OrderDetails
                    .Where(d => d.OrderId == r.Id)
                    .Select(d => (double)d.Price * d.ProductAmount)
                    .Sum();

                Console.WriteLine($"{r.Id,4}  {r.OperationTime ?? string.Empty,19}  {UserLabel(r.User),-20}  {CustomerOrderService.StatusName(r.OrderStateId),-28}  {total,10:0.00}");
            }

            Console.WriteLine("\nTip: Use [3] to cancel by ID, [4] to change status.");
            Pause();
        }

        private void ShowOrderDetails()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: ORDER DETAILS ===\n");
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            var order = this.db.CustomerOrders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Pause();
                return;
            }

            Console.WriteLine($"\nID: {order.Id}");
            Console.WriteLine($"Date: {order.OperationTime ?? string.Empty}");
            Console.WriteLine($"User: {UserLabel(order.User)}");
            Console.WriteLine($"Status: {CustomerOrderService.StatusName(order.OrderStateId)}");

            var details = this.db.OrderDetails
                .Include(d => d.Product)
                .ThenInclude(p => p!.Title) // null-forgiving to avoid CS8602 on ThenInclude
                .Where(d => d.OrderId == id)
                .ToList();

            if (details.Count == 0)
            {
                Console.WriteLine("\nNo items.");
                Pause();
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
            Console.WriteLine($"{"TOTAL",-30}  {string.Empty,10}  {string.Empty,5}  {total,10:0.00}");

            Pause();
        }

        private void AdminCancelOrder()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: CANCEL ORDER ===\n");
            Console.Write("Enter Order ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            var order = this.db.CustomerOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Pause();
                return;
            }

            if (order.OrderStateId is 2 or 3 or 8)
            {
                Console.WriteLine($"Order already final: {CustomerOrderService.StatusName(order.OrderStateId)}");
                Pause();
                return;
            }

            if (!CustomerOrderService.CanTransition(order.OrderStateId, 3))
            {
                var next = string.Join(", ", CustomerOrderService
                    .GetAllowedNextStates(order.OrderStateId)
                    .Select(CustomerOrderService.StatusName));

                Console.WriteLine($"Cancel is not allowed from current state ({CustomerOrderService.StatusName(order.OrderStateId)}). Allowed next: [{next}].");
                Pause();
                return;
            }

            if (this.orderService.TryChangeState(id, 3, out var error))
            {
                this.stockService.ReleaseOrderReservations(id);
                Console.WriteLine($"Order {id} cancelled by administrator. Reservations released.");
            }
            else
            {
                Console.WriteLine(error);
            }

            Pause();
        }

        private void ChangeOrderStatusInteractive()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: CHANGE ORDER STATUS ===\n");
            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                Pause();
                return;
            }

            var order = this.db.CustomerOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                Pause();
                return;
            }

            var allowed = CustomerOrderService.GetAllowedNextStates(order.OrderStateId);
            if (allowed.Count == 0)
            {
                Console.WriteLine($"No allowed transitions from current state: {CustomerOrderService.StatusName(order.OrderStateId)}.");
                Pause();
                return;
            }

            Console.WriteLine($"\nCurrent: {CustomerOrderService.StatusName(order.OrderStateId)}");
            Console.WriteLine("Allowed next states:");
            for (int i = 0; i < allowed.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {CustomerOrderService.StatusName(allowed[i])} (#{allowed[i]})");
            }

            Console.Write("\nChoose option: ");
            if (!int.TryParse(Console.ReadLine(), out int opt) || opt < 1 || opt > allowed.Count)
            {
                Console.WriteLine("Invalid option.");
                Pause();
                return;
            }

            int target = allowed[opt - 1];

            if (this.orderService.TryChangeState(id, target, out var error))
            {
                if (target == 8)
                {
                    var stockSvc = new StockReservationService(this.db);
                    stockSvc.ConfirmOrderDelivery(id);
                }

                Console.WriteLine($"Order {id} moved to: {CustomerOrderService.StatusName(target)}");
            }
            else
            {
                Console.WriteLine(error);
            }

            Pause();
        }
    }
}
