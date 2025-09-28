// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Controllers\AdminOrderController.cs
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Interfaces;
using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Entities;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Enhanced admin orders management controller with advanced status visualization and transition management.
    /// Provides comprehensive order lifecycle control with visual status flow representation.
    /// </summary>
    public sealed class AdminOrderController
    {
        /// <summary>
        /// Database context for data operations.
        /// </summary>
        private readonly StoreDbContext db;

        /// <summary>
        /// Service for managing stock reservations.
        /// </summary>
        private readonly StockReservationService stockService;

        /// <summary>
        /// Service for customer order operations.
        /// </summary>
#pragma warning disable CA1859 // Keep interface type for testability and loose coupling (intentional)
        private readonly ICustomerOrderService orderService;
#pragma warning restore CA1859

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminOrderController"/> class.
        /// </summary>
        /// <param name="db">Database context for operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
        public AdminOrderController(StoreDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.stockService = new StockReservationService(db);
            this.orderService = new CustomerOrderService(db); // concrete creation; typed via interface
        }

        /// <summary>
        /// Backward-compat alias used by existing menus.
        /// </summary>
        public void ShowOrders() => this.ShowOrdersSnapshot();

        /// <summary>
        /// Main entry point for admin order management with enhanced features.
        /// </summary>
        public void Run()
        {
            var prev = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== ADMIN: ORDERS MANAGEMENT ===\n");
                    Console.WriteLine("1. Orders snapshot (table)");
                    Console.WriteLine("2. View order details");
                    Console.WriteLine("3. Cancel order (admin)");
                    Console.WriteLine("4. Change order status (guided)");
                    Console.WriteLine("5. Bulk status operations");
                    Console.WriteLine("6. Order analytics");
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
                        case ConsoleKey.D5:
                        case ConsoleKey.NumPad5:
                            this.ShowBulkOperations();
                            break;
                        case ConsoleKey.D6:
                        case ConsoleKey.NumPad6:
                            this.ShowOrderAnalytics();
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

        /// <summary>
        /// Shows enhanced orders snapshot with filtering options.
        /// </summary>
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

            // Show summary statistics
            var totalOrders = rows.Count;
            var activeOrders = rows.Count(r => r.OrderStateId >= 1 && r.OrderStateId <= 7 && r.OrderStateId != 2 && r.OrderStateId != 3);
            var completedOrders = rows.Count(r => r.OrderStateId == 8);
            var cancelledOrders = rows.Count(r => r.OrderStateId == 2 || r.OrderStateId == 3);

            Console.WriteLine("📊 ORDERS OVERVIEW");
            Console.WriteLine($"Total: {totalOrders} | Active: {activeOrders} | Completed: {completedOrders} | Cancelled: {cancelledOrders}");
            Console.WriteLine();

            Console.WriteLine($"{"ID",4}  {"Date",19}  {"User",-20}  {"Status",-28}  {"Total",10}");
            Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 20 + 2 + 28 + 2 + 10));

            foreach (var r in rows.Take(20)) // Limit to prevent screen overflow
            {
                var total = (decimal)this.db.OrderDetails
                    .Where(d => d.OrderId == r.Id)
                    .Select(d => (double)d.Price * d.ProductAmount)
                    .Sum();

                var statusIcon = GetOrderStatusIcon(r.OrderStateId);
                var statusName = CustomerOrderService.StatusName(r.OrderStateId);

                Console.WriteLine($"{r.Id,4}  {r.OperationTime ?? string.Empty,19}  {UserLabel(r.User),-20}  {statusIcon} {statusName,-25}  {total,10:0.00}");
            }

            if (rows.Count > 20)
            {
                Console.WriteLine($"\n... and {rows.Count - 20} more orders");
            }

            Console.WriteLine("\n💡 Tips:");
            Console.WriteLine("   • Use option 4 to change order status with guided workflow");
            Console.WriteLine("   • Use option 2 to view detailed order information");
            Console.WriteLine
