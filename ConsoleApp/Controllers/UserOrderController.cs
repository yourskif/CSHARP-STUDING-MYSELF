// Path: console-online-store/ConsoleApp/Controllers/UserOrderController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Interfaces;
using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Enhanced controller for user order management operations with order tracking capabilities.
/// Provides comprehensive order lifecycle management including progress tracking and status visualization.
/// </summary>
public class UserOrderController
{
    /// <summary>
    /// Database context for data operations.
    /// </summary>
    private readonly StoreDbContext context;

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
    /// Initializes a new instance of the <see cref="UserOrderController"/> class.
    /// </summary>
    /// <param name="context">Database context for operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public UserOrderController(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.stockService = new StockReservationService(context);
        this.orderService = new CustomerOrderService(context); // keep concrete creation, typed via interface
    }

    /// <summary>
    /// Overloaded method for compatibility with UserMainMenu.
    /// </summary>
    /// <param name="context">Database context for operations.</param>
    public void ShowOrderMenu(StoreDbContext context)
    {
        this.ShowOrderMenu();
    }

    /// <summary>
    /// Shows the enhanced order management menu with tracking capabilities.
    /// </summary>
    public void ShowOrderMenu()
    {
        // Fix culture for aligned numbers
        var prev = System.Threading.Thread.CurrentThread.CurrentCulture;
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        try
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MY ORDERS ===");
                Console.WriteLine();
                Console.WriteLine("1. View My Orders");
                Console.WriteLine("2. Create New Order");
                Console.WriteLine("3. Cancel Order");
                Console.WriteLine("4. Mark Order as Received");
                Console.WriteLine("5. Track Order Progress");
                Console.WriteLine("6. Order History Summary");
                Console.WriteLine();
                Console.WriteLine("Esc: Back to Main Menu");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        this.ShowMyOrders();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        this.CreateOrder();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        this.CancelOrder();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        this.MarkOrderAsReceived();
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        this.ShowOrderProgress();
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        this.ShowOrderHistorySummary();
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
    /// Creates a new order with enhanced product selection and validation.
    /// </summary>
    public void CreateOrder()
    {
        Console.Clear();
        Console.WriteLine("=== CREATE NEW ORDER ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        var orderDetails = new List<OrderDetail>();
        decimal totalAmount = 0;

        Console.WriteLine("Available products:");
        var availableProducts = this.context.Products
            .Include(p => p.Title)
            .Where(p => p.AvailableQuantity > 0)
            .Take(10)
            .ToList();

        foreach (var product in availableProducts)
        {
            var title = product.Title?.Title ?? $"Product {product.Id}";
            Console.WriteLine($"  ID: {product.Id} - {title} - ${product.UnitPrice:F2} (Available: {product.AvailableQuantity})");
        }

        Console.WriteLine();

        while (true)
        {
            Console.Write("\nEnter Product ID (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
            {
                break;
            }

            var product = this.context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("❌ Product not found!");
                continue;
            }

            var safeTitle = product.Title?.Title ?? $"Product {product.Id}";
            Console.WriteLine($"Product: {safeTitle} - Price: ${product.UnitPrice:F2}");
            Console.WriteLine($"Available stock: {product.AvailableQuantity}");

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("❌ Invalid quantity!");
                continue;
            }

            if (quantity > product.AvailableQuantity)
            {
                Console.WriteLine($"❌ Insufficient stock! Available: {product.AvailableQuantity}");
                continue;
            }

            orderDetails.Add(new OrderDetail
            {
                ProductId = productId,
                ProductAmount = quantity,
                Price = product.UnitPrice,
                Product = product,
            });

            totalAmount += product.UnitPrice * quantity;
            Console.WriteLine($"✅ Added to order. Current total: ${totalAmount:F2}");
        }

        if (orderDetails.Count == 0)
        {
            Console.WriteLine("No items in order. Order cancelled.");
            Pause();
            return;
        }

        // Show order summary before confirmation
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("ORDER SUMMARY");
        Console.WriteLine(new string('=', 50));
        foreach (var detail in orderDetails)
        {
            var productName = detail.Product?.Title?.Title ?? $"Product {detail.ProductId}";
            Console.WriteLine($"{productName} x{detail.ProductAmount} @ ${detail.Price:F2} = ${detail.Price * detail.ProductAmount:F2}");
        }

        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"TOTAL: ${totalAmount:F2}");
        Console.WriteLine(new string('=', 50));

        Console.Write("Confirm order? (y/n): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(confirm, "yes", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Order cancelled by user.");
            Pause();
            return;
        }

        // Use invariant culture for deterministic formatting (CA1305)
        var order = new CustomerOrder
        {
            UserId = user.Id,
            OperationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            OrderStateId = 1, // New Order
            Details = orderDetails,
        };

        this.context.CustomerOrders.Add(order);
        this.context.SaveChanges();

        // Reserve stock for each detail and update reserved counts
        foreach (var detail in orderDetails)
        {
            var product = this.context.Products.Find(detail.ProductId);
            if (product == null)
            {
                continue;
            }

            // Re-check availability right before reservation
            if (detail.ProductAmount > product.AvailableQuantity)
            {
                Console.WriteLine($"⚠ Product {product.Id}: requested {detail.ProductAmount}, but only {product.AvailableQuantity} available now.");
                Console.WriteLine("Order creation aborted. No changes were applied.");
                this.context.CustomerOrders.Remove(order);
                this.context.SaveChanges();
                Pause();
                return;
            }

            product.ReservedQuantity += detail.ProductAmount;
            Console.WriteLine($"✅ Reserved {detail.ProductAmount} unit(s) of product {product.Id}.");
        }

        this.context.SaveChanges();

        Console.WriteLine($"\n🎉 Order created successfully!");
        Console.WriteLine($"Order ID: {order.Id}");
        Console.WriteLine($"Total amount: ${totalAmount:F2}");
        Console.WriteLine($"Status: New Order");

        Pause();
    }

    /// <summary>
    /// Shows user's orders in a tabular, fixed-width format with enhanced information.
    /// </summary>
    public void ShowMyOrders()
    {
        Console.Clear();
        Console.WriteLine("=== MY ORDERS ===\n");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        var orders = this.context.CustomerOrders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.Id)
            .ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders yet.");
            Console.WriteLine("Use option 2 to create your first order!");
            Pause();
            return;
        }

        // Header: ID(4) | Date(19) | Status(28) | Items(6) | Total(10)
        Console.WriteLine($"{"ID",4}  {"Date",19}  {"Status",-28}  {"Items",6}  {"Total",10}");
        Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 28 + 2 + 6 + 2 + 10));

        foreach (var order in orders)
        {
            // SQLite cannot aggregate decimal directly: double -> decimal
            var total = (decimal)this.context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Select(d => (double)d.Price * d.ProductAmount)
                .Sum();

            var itemCount = this.context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Sum(d => d.ProductAmount);

            var status = GetOrderStatusName(order.OrderStateId);
            var statusIcon = GetOrderStatusIcon(order.OrderStateId);

            Console.WriteLine($"{order.Id,4}  {order.OperationTime,19}  {statusIcon} {status,-26}  {itemCount,6}  ${total,8:0.00}");
        }

        Console.WriteLine();
        Console.WriteLine("💡 Tips:");
        Console.WriteLine("   • Use option 5 to track specific order progress");
        Console.WriteLine("   • Use option 3 to cancel orders in 'New Order' status");
        Console.WriteLine("   • Use option 4 to confirm delivery of delivered orders");
        Pause();
    }

    /// <summary>
    /// Shows detailed order progress tracking with visual status progression.
    /// </summary>
    public void ShowOrderProgress()
    {
        Console.Clear();
        Console.WriteLine("=== ORDER PROGRESS TRACKING ===");
        Console.Write("Enter Order ID: ");

        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("❌ Invalid Order ID.");
            Pause();
            return;
        }

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        var order = this.context.CustomerOrders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .ThenInclude(p => p != null ? p.Title : null)
            .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

        if (order == null)
        {
            Console.WriteLine("❌ Order not found or access denied.");
            Pause();
            return;
        }

        // Calculate order total
        var total = order.Details.Sum(d => d.Price * d.ProductAmount);

        Console.WriteLine();
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"ORDER #{orderId} TRACKING");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"Order Date: {order.OperationTime}");
        Console.WriteLine($"Total Value: ${total:F2}");
        Console.WriteLine($"Items Count: {order.Details.Sum(d => d.ProductAmount)}");
        Console.WriteLine();

        // Show order items
        Console.WriteLine("Order Items:");
        Console.WriteLine($"{"Product",-25} {"Qty",5} {"Price",10} {"Subtotal",10}");
        Console.WriteLine(new string('-', 52));
        foreach (var detail in order.Details)
        {
            var productName = detail.Product?.Title?.Title ?? $"Product {detail.ProductId}";
            var subtotal = detail.Price * detail.ProductAmount;
            Console.WriteLine($"{Trunc(productName, 25),-25} {detail.ProductAmount,5} ${detail.Price,7:F2} ${subtotal,7:F2}");
        }

        Console.WriteLine();
        ShowOrderStatusProgress(order.OrderStateId);
        Console.WriteLine($"\nCurrent Status: {GetOrderStatusName(order.OrderStateId)}");

        var nextStates = CustomerOrderService.GetAllowedNextStates(order.OrderStateId);
        if (nextStates.Count > 0)
        {
            Console.WriteLine("\n🔮 What's Next:");
            foreach (var stateId in nextStates)
            {
                Console.WriteLine($"  → {GetOrderStatusName(stateId)}");
            }

            if (order.OrderStateId == 1)
            {
                Console.WriteLine("\n💡 You can cancel this order using option 3 in the main menu.");
            }
            else if (order.OrderStateId == 7)
            {
                Console.WriteLine("\n💡 You can mark this order as received using option 4 in the main menu.");
            }
        }
        else
        {
            Console.WriteLine("\n🏁 This order is in final state - no further actions available.");
        }

        Pause();
    }

    /// <summary>
    /// Shows order history summary with statistics.
    /// </summary>
    public void ShowOrderHistorySummary()
    {
        Console.Clear();
        Console.WriteLine("=== ORDER HISTORY SUMMARY ===\n");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        var orders = this.context.CustomerOrders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.Details)
            .ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("No order history available.");
            Pause();
            return;
        }

        var totalOrders = orders.Count;
        var completedOrders = orders.Count(o => o.OrderStateId == 8);
        var cancelledOrders = orders.Count(o => o.OrderStateId == 2 || o.OrderStateId == 3);
        var activeOrders = orders.Count(o => o.OrderStateId >= 1 && o.OrderStateId <= 7 && o.OrderStateId != 2 && o.OrderStateId != 3);

        var totalSpent = orders
            .SelectMany(o => o.Details)
            .Sum(d => d.Price * d.ProductAmount);

        var averageOrderValue = totalOrders > 0 ? totalSpent / totalOrders : 0;

        Console.WriteLine("📊 ORDER STATISTICS");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Total Orders:        {totalOrders}");
        Console.WriteLine($"Completed Orders:    {completedOrders}");
        Console.WriteLine($"Active Orders:       {activeOrders}");
        Console.WriteLine($"Cancelled Orders:    {cancelledOrders}");
        Console.WriteLine($"Total Spent:         ${totalSpent:F2}");
        Console.WriteLine($"Average Order Value: ${averageOrderValue:F2}");
        Console.WriteLine();

        if (completedOrders > 0)
        {
            var completionRate = (double)completedOrders / totalOrders * 100;
            Console.WriteLine($"Order Completion Rate: {completionRate:F1}%");
        }

        Console.WriteLine();
        Console.WriteLine("📈 ORDER STATUS BREAKDOWN");
        Console.WriteLine(new string('=', 50));
        var statusGroups = orders.GroupBy(o => o.OrderStateId)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in statusGroups)
        {
            var statusName = GetOrderStatusName(group.Key);
            var count = group.Count();
            var percentage = (double)count / totalOrders * 100;
            Console.WriteLine($"{GetOrderStatusIcon(group.Key)} {statusName,-28}: {count,3} ({percentage,4:F1}%)");
        }

        if (orders.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("📅 RECENT ACTIVITY");
            Console.WriteLine(new string('=', 50));
            var recentOrders = orders
                .OrderByDescending(o => o.Id)
                .Take(5)
                .ToList();

            foreach (var order in recentOrders)
            {
                var orderTotal = order.Details.Sum(d => d.Price * d.ProductAmount);
                Console.WriteLine($"Order #{order.Id} - {order.OperationTime} - ${orderTotal:F2} - {GetOrderStatusName(order.OrderStateId)}");
            }
        }

        Pause();
    }

    /// <summary>
    /// Cancels an order with enhanced user feedback.
    /// </summary>
    public void CancelOrder()
    {
        Console.Clear();
        Console.WriteLine("=== CANCEL ORDER ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        // Show cancellable orders first
        var cancellableOrders = this.context.CustomerOrders
            .Where(o => o.UserId == user.Id && o.OrderStateId == 1)
            .OrderByDescending(o => o.Id)
            .ToList();

        if (cancellableOrders.Count == 0)
        {
            Console.WriteLine("❌ You have no orders that can be cancelled.");
            Console.WriteLine("💡 Only orders with 'New Order' status can be cancelled.");
            Pause();
            return;
        }

        Console.WriteLine("Cancellable orders:");
        foreach (var order in cancellableOrders)
        {
            var total = this.context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Sum(d => d.Price * d.ProductAmount);
            Console.WriteLine($"  Order #{order.Id} - {order.OperationTime} - ${total:F2}");
        }

        Console.WriteLine();

        Console.Write("Enter Order ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("❌ Invalid order ID!");
            Pause();
            return;
        }

        // Delegate business rules to service; release reservations on success.
        if (this.orderService.CancelOwnOrder(orderId, user.Id, out var error))
        {
            this.stockService.ReleaseOrderReservations(orderId);
            Console.WriteLine("✅ Order cancelled successfully. Stock reservations released.");
        }
        else
        {
            Console.WriteLine($"❌ {error}");
        }

        Pause();
    }

    /// <summary>
    /// Marks order as received with enhanced validation.
    /// </summary>
    public void MarkOrderAsReceived()
    {
        Console.Clear();
        Console.WriteLine("=== MARK ORDER AS RECEIVED ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            Pause();
            return;
        }

        // Show deliverable orders first
        var deliveredOrders = this.context.CustomerOrders
            .Where(o => o.UserId == user.Id && o.OrderStateId == 7)
            .OrderByDescending(o => o.Id)
            .ToList();

        if (deliveredOrders.Count == 0)
        {
            Console.WriteLine("❌ You have no orders ready for confirmation.");
            Console.WriteLine("💡 Only orders with 'Delivered to client' status can be marked as received.");
            Pause();
            return;
        }

        Console.WriteLine("Orders ready for confirmation:");
        foreach (var order in deliveredOrders)
        {
            var total = this.context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Sum(d => d.Price * d.ProductAmount);
            Console.WriteLine($"  Order #{order.Id} - {order.OperationTime} - ${total:F2}");
        }

        Console.WriteLine();

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("❌ Invalid order ID!");
            Pause();
            return;
        }

        // Delegate business rules to service; confirm stock on success.
        if (this.orderService.MarkAsReceived(orderId, user.Id, out var error))
        {
            this.stockService.ConfirmOrderDelivery(orderId);
            Console.WriteLine("🎉 Order marked as received. Thank you for your purchase!");
            Console.WriteLine("💝 We hope you enjoyed your shopping experience!");
        }
        else
        {
            Console.WriteLine($"❌ {error}");
        }

        Pause();
    }

    /// <summary>
    /// Displays visual order status progression.
    /// </summary>
    /// <param name="currentStateId">Current order state ID.</param>
    private static void ShowOrderStatusProgress(int currentStateId)
    {
        var states = new[]
        {
            (1, "New Order", "📝"),
            (4, "Confirmed", "✅"),
            (5, "Moved to delivery", "🚚"),
            (6, "In delivery", "🚛"),
            (7, "Delivered", "📦"),
            (8, "Confirmed by client", "🎉"),
        };

        Console.WriteLine("📋 Order Progress:");
        Console.WriteLine(new string('═', 60));

        for (int i = 0; i < states.Length; i++)
        {
            var (id, name, icon) = states[i];
            string status;
            string connector = i < states.Length - 1 ? " ──→ " : string.Empty;

            if (id == currentStateId)
            {
                status = $"{icon} {name} [CURRENT] ";
            }
            else if (ShouldShowAsCompleted(id, currentStateId))
            {
                status = $"✓ {name} ";
            }
            else
            {
                status = $"○ {name} ";
            }

            Console.Write(status);
            if (i < states.Length - 1)
            {
                Console.WriteLine();
                Console.WriteLine("        │");
                Console.WriteLine("        ▼");
            }
        }

        Console.WriteLine();
        Console.WriteLine(new string('═', 60));

        // Handle cancelled states
        if (currentStateId == 2)
        {
            Console.WriteLine("❌ Order was cancelled by user");
        }
        else if (currentStateId == 3)
        {
            Console.WriteLine("❌ Order was cancelled by administrator");
        }
    }

    /// <summary>
    /// Determines if a state should be shown as completed based on current state.
    /// </summary>
    /// <param name="stateId">State ID to check.</param>
    /// <param name="currentStateId">Current order state ID.</param>
    /// <returns>True if state should be shown as completed.</returns>
    private static bool ShouldShowAsCompleted(int stateId, int currentStateId)
    {
        // States in order: 1 -> 4 -> 5 -> 6 -> 7 -> 8
        var stateOrder = new Dictionary<int, int>
        {
            { 1, 1 }, { 4, 2 }, { 5, 3 }, { 6, 4 }, { 7, 5 }, { 8, 6 },
        };

        return stateOrder.ContainsKey(stateId) && stateOrder.ContainsKey(currentStateId) &&
               stateOrder[stateId] < stateOrder[currentStateId];
    }

    /// <summary>
    /// Gets order status name by ID.
    /// </summary>
    /// <param name="statusId">Order status ID.</param>
    /// <returns>Human-readable status name.</returns>
    private static string GetOrderStatusName(int statusId) => statusId switch
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

    /// <summary>
    /// Gets visual icon for order status.
    /// </summary>
    /// <param name="statusId">Order status ID.</param>
    /// <returns>Icon character for the status.</returns>
    private static string GetOrderStatusIcon(int statusId) => statusId switch
    {
        1 => "📝",
        2 => "❌",
        3 => "🛑",
        4 => "✅",
        5 => "🚚",
        6 => "🚛",
        7 => "📦",
        8 => "🎉",
        _ => "❓",
    };

    /// <summary>
    /// Truncates a string to specified length with ellipsis.
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

        return string.Concat(s.AsSpan(0, Math.Max(0, max - 1)), "…");
    }

    /// <summary>
    /// Pauses for user input.
    /// </summary>
    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}
