// Path: console-online-store/ConsoleApp/Controllers/AdminOrderController.cs
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Interfaces;
using StoreBLL.Services;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Admin orders management controller.
    /// Provides comprehensive order management including viewing, canceling, status changes, and creation.
    /// </summary>
    public sealed class AdminOrderController
    {
        // ---------- instance fields ----------
        private readonly IStoreUnitOfWork unitOfWork;
        private readonly StockReservationService stockService;

#pragma warning disable CA1859 // Keep interface type for testability and loose coupling (intentional)
        private readonly ICustomerOrderService orderService;
#pragma warning restore CA1859

        // ---------- ctor ----------
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminOrderController"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
        public AdminOrderController(IStoreUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.stockService = new StockReservationService(unitOfWork);
            this.orderService = new CustomerOrderService(unitOfWork);
        }

        // ---------- PUBLIC methods (SA1202: public before private) ----------

        /// <summary>
        /// Backward-compatible alias used by existing menus.
        /// </summary>
        public void ShowOrders() => this.ShowOrdersSnapshot();

        /// <summary>
        /// Main menu loop for admin order operations.
        /// Displays options and handles navigation for order management.
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
                    Console.WriteLine("=== ADMIN: ORDERS ===\n");
                    Console.WriteLine("1. Orders snapshot (table)");
                    Console.WriteLine("2. View order details");
                    Console.WriteLine("3. Cancel order (admin)");
                    Console.WriteLine("4. Change order status (choose allowed)");
                    Console.WriteLine("5. Create new order (as admin)");
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
                            this.CreateOrderAsAdmin();
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

        /// <summary>
        /// Gets user display label from user entity.
        /// Tries multiple property names for compatibility.
        /// </summary>
        /// <param name="u">User entity.</param>
        /// <returns>Display label or fallback string.</returns>
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

        /// <summary>
        /// Reads string property value from object using reflection.
        /// </summary>
        /// <param name="obj">Object to read from.</param>
        /// <param name="propName">Property name (case-insensitive).</param>
        /// <returns>String value or null if not found.</returns>
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

        /// <summary>
        /// Truncates string to maximum length with ellipsis.
        /// </summary>
        /// <param name="s">String to truncate.</param>
        /// <param name="max">Maximum length.</param>
        /// <returns>Truncated string.</returns>
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
            return string.Concat(s.AsSpan(0, take), "...");
        }

        /// <summary>
        /// Pauses execution and waits for user input.
        /// </summary>
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        // ---------- PRIVATE instance methods ----------

        /// <summary>
        /// Displays tabular snapshot of all orders.
        /// Shows order ID, date, user, status, and total amount.
        /// </summary>
        private void ShowOrdersSnapshot()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: ORDERS SNAPSHOT ===\n");

            var rows = this.unitOfWork.Context.CustomerOrders
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
                var total = (decimal)this.unitOfWork.Context.OrderDetails
                    .Where(d => d.OrderId == r.Id)
                    .Select(d => (double)d.Price * d.ProductAmount)
                    .Sum();

                Console.WriteLine($"{r.Id,4}  {r.OperationTime ?? string.Empty,19}  {UserLabel(r.User),-20}  {CustomerOrderService.StatusName(r.OrderStateId),-28}  {total,10:0.00}");
            }

            Console.WriteLine("\nTip: Use [3] to cancel by ID, [4] to change status, [5] to create order.");
            Pause();
        }

        /// <summary>
        /// Displays detailed information about specific order.
        /// Shows order header and all line items with products and prices.
        /// </summary>
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

            var order = this.unitOfWork.Context.CustomerOrders
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

            var details = this.unitOfWork.Context.OrderDetails
                .Include(d => d.Product)
                .ThenInclude(p => p!.Title)
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

        /// <summary>
        /// Cancels order by administrator.
        /// Validates permissions and releases stock reservations.
        /// </summary>
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

            var order = this.unitOfWork.Context.CustomerOrders.FirstOrDefault(o => o.Id == id);
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

        /// <summary>
        /// Interactive order status change with validation.
        /// Shows allowed transitions and validates before changing.
        /// </summary>
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

            var order = this.unitOfWork.Context.CustomerOrders.FirstOrDefault(o => o.Id == id);
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
                    var stockSvc = new StockReservationService(this.unitOfWork);
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

        /// <summary>
        /// Creates new order as administrator.
        /// Delegates to UserOrderController for order creation process.
        /// </summary>
        private void CreateOrderAsAdmin()
        {
            Console.Clear();
            Console.WriteLine("=== ADMIN: CREATE NEW ORDER ===\n");
            Console.WriteLine("Note: You are creating an order with admin privileges.");
            Console.WriteLine("The order will be assigned to your admin account.");
            Console.WriteLine();

            // Check if admin is logged in
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("Error: No user is logged in.");
                Pause();
                return;
            }

            // Check if current user is admin
            if (UserMenuController.CurrentUser.RoleId != 1)
            {
                Console.WriteLine("Error: Only administrators can use this feature.");
                Pause();
                return;
            }

            // Delegate order creation to UserOrderController
            var orderController = new UserOrderController(this.unitOfWork);
            orderController.CreateOrder();

            Console.WriteLine("\nOrder created successfully!");
            Console.WriteLine("You can view and manage this order in the orders list.");
            Pause();
        }
    }
}
