// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\AdminOrderController.cs

namespace ConsoleApp.Controllers;

using System;
using System.Linq;
using StoreBLL.Services;
using StoreDAL.Data;

/// <summary>
/// Controller for admin order management operations.
/// </summary>
public class AdminOrderController
{
    private readonly StoreDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminOrderController"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public AdminOrderController(StoreDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Shows all orders.
    /// </summary>
    public void ShowOrders()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ORDER MANAGEMENT ===");
            Console.WriteLine();
            Console.WriteLine("1. View All Orders");
            Console.WriteLine("2. View Order Details");
            Console.WriteLine("3. Update Order Status");
            Console.WriteLine("4. Cancel Order");
            Console.WriteLine();
            Console.WriteLine("Esc: Back to Admin Menu");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    this.ViewAllOrders();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    this.ViewOrderDetails();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    this.UpdateOrderStatus();
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    this.CancelOrder();
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    /// <summary>
    /// Views all orders.
    /// </summary>
    private void ViewAllOrders()
    {
        Console.Clear();
        Console.WriteLine("=== ALL ORDERS ===");

        var orders = this.context.CustomerOrders
            .OrderByDescending(o => o.Id)
            .ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
        }
        else
        {
            Console.WriteLine($"{"ID",-6} {"User",-15} {"Date",-20} {"Status",-25} {"Total",-10}");
            Console.WriteLine(new string('-', 80));

            foreach (var order in orders)
            {
                // Get user info
                var user = this.context.Users.FirstOrDefault(u => u.Id == order.UserId);
                var userName = user != null ? $"{user.Name} {user.LastName}" : "Unknown";

                // Calculate total
                var orderDetails = this.context.OrderDetails.Where(d => d.OrderId == order.Id).ToList();
                decimal total = 0;
                foreach (var detail in orderDetails)
                {
                    total += detail.Price * detail.ProductAmount;
                }

                // Get status name
                var statusName = this.GetOrderStatusName(order.OrderStateId);

                Console.WriteLine($"{order.Id,-6} {userName,-15} {order.OperationTime,-20} {statusName,-25} ${total,-10:F2}");
            }
        }

        this.Pause();
    }

    /// <summary>
    /// Views order details.
    /// </summary>
    private void ViewOrderDetails()
    {
        Console.Clear();
        Console.WriteLine("=== ORDER DETAILS ===");

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            this.Pause();
            return;
        }

        var order = this.context.CustomerOrders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            Console.WriteLine("Order not found!");
            this.Pause();
            return;
        }

        var user = this.context.Users.FirstOrDefault(u => u.Id == order.UserId);
        var orderDetails = this.context.OrderDetails.Where(d => d.OrderId == orderId).ToList();

        Console.WriteLine($"\n=== Order #{order.Id} ===");
        Console.WriteLine($"Customer: {user?.Name} {user?.LastName} ({user?.Login})");
        Console.WriteLine($"Date: {order.OperationTime}");
        Console.WriteLine($"Status: {this.GetOrderStatusName(order.OrderStateId)}");
        Console.WriteLine("\nOrder Items:");
        Console.WriteLine($"{"Product",-30} {"Price",-10} {"Qty",-8} {"Subtotal",-10}");
        Console.WriteLine(new string('-', 60));

        decimal total = 0;
        foreach (var detail in orderDetails)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == detail.ProductId);

            // Get product title name - using Title property
            var productTitle = "Unknown Product";
            if (product?.Title != null)
            {
                productTitle = product.Title.Title ?? $"Product {product.Id}";
            }

            var subtotal = detail.Price * detail.ProductAmount;
            total += subtotal;

            Console.WriteLine($"{productTitle,-30} ${detail.Price,-10:F2} {detail.ProductAmount,-8} ${subtotal,-10:F2}");
        }

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"TOTAL:",-48} ${total:F2}");

        this.Pause();
    }

    /// <summary>
    /// Updates order status.
    /// </summary>
    public void UpdateOrderStatus()
    {
        Console.Clear();
        Console.WriteLine("=== UPDATE ORDER STATUS ===");

        Console.Write("Enter Order ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            this.Pause();
            return;
        }

        var order = this.context.CustomerOrders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            Console.WriteLine("Order not found.");
            this.Pause();
            return;
        }

        Console.WriteLine($"\nOrder #{orderId} - Current status: {this.GetOrderStatusName(order.OrderStateId)}");
        Console.WriteLine("\nAvailable statuses:");
        Console.WriteLine("1. New Order");
        Console.WriteLine("2. Cancelled by user");
        Console.WriteLine("3. Cancelled by administrator");
        Console.WriteLine("4. Confirmed");
        Console.WriteLine("5. Moved to delivery company");
        Console.WriteLine("6. In delivery");
        Console.WriteLine("7. Delivered to client");
        Console.WriteLine("8. Delivery confirmed by client");

        Console.Write("\nSelect new status (1-8): ");
        if (!int.TryParse(Console.ReadLine(), out int newStatus) || newStatus < 1 || newStatus > 8)
        {
            Console.WriteLine("Invalid status selected.");
            this.Pause();
            return;
        }

        var stockService = new StockReservationService(this.context);

        // If changing from active to cancelled status, release reservations
        if ((order.OrderStateId == 1 || order.OrderStateId == 4 || order.OrderStateId == 5 || order.OrderStateId == 6)
            && (newStatus == 2 || newStatus == 3))
        {
            stockService.ReleaseOrderReservations(orderId);
            Console.WriteLine("Stock reservations have been released.");
        }

        // If changing to delivered status
        if (newStatus == 7 && order.OrderStateId != 7)
        {
            Console.WriteLine("Order marked as delivered to client. User can now confirm receipt.");
        }

        order.OrderStateId = newStatus;
        this.context.SaveChanges();

        Console.WriteLine($"Order status updated to: {this.GetOrderStatusName(newStatus)}");
        this.Pause();
    }

    /// <summary>
    /// Cancels an order.
    /// </summary>
    private void CancelOrder()
    {
        Console.Clear();
        Console.WriteLine("=== CANCEL ORDER ===");

        Console.Write("Enter Order ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            this.Pause();
            return;
        }

        var order = this.context.CustomerOrders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            Console.WriteLine("Order not found.");
            this.Pause();
            return;
        }

        if (order.OrderStateId == 2 || order.OrderStateId == 3 || order.OrderStateId == 8)
        {
            Console.WriteLine("Order is already cancelled or completed.");
            this.Pause();
            return;
        }

        var stockService = new StockReservationService(this.context);
        stockService.ReleaseOrderReservations(orderId);

        order.OrderStateId = 3; // Cancelled by administrator
        this.context.SaveChanges();

        Console.WriteLine("Order cancelled successfully. Stock reservations released.");
        this.Pause();
    }

    /// <summary>
    /// Gets order status name.
    /// </summary>
    private string GetOrderStatusName(int statusId)
    {
        return statusId switch
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
    }

    /// <summary>
    /// Pauses for user input.
    /// </summary>
    private void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}