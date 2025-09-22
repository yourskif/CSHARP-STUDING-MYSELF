// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\ConsoleApp\Controllers\UserOrderController.cs

namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using StoreBLL.Services;
using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Controller for user order management operations.
/// </summary>
public class UserOrderController
{
    private readonly StoreDbContext context;
    private readonly StockReservationService stockService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserOrderController"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public UserOrderController(StoreDbContext context)
    {
        this.context = context;
        this.stockService = new StockReservationService(context);
    }

    /// <summary>
    /// Overloaded method for compatibility with UserMainMenu.
    /// </summary>
    /// <param name="context">Database context.</param>
    public void ShowOrderMenu(StoreDbContext context)
    {
        this.ShowOrderMenu();
    }

    /// <summary>
    /// Shows the order management menu.
    /// </summary>
    public void ShowOrderMenu()
    {
        // fix culture for aligned numbers
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
    /// Creates a new order.
    /// </summary>
    public void CreateOrder()
    {
        Console.Clear();
        Console.WriteLine("=== CREATE NEW ORDER ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            this.Pause();
            return;
        }

        var orderDetails = new List<OrderDetail>();
        decimal totalAmount = 0;

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
                Console.WriteLine("Product not found!");
                continue;
            }

            // Safe title
            string titleName = product.Title?.Title ?? $"Product {product.Id}";

            Console.WriteLine($"Product: {titleName} - Price: ${product.UnitPrice:F2}");
            Console.WriteLine($"Available stock: {product.AvailableQuantity}");

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity!");
                continue;
            }

            if (quantity > product.AvailableQuantity)
            {
                Console.WriteLine($"Insufficient stock! Available: {product.AvailableQuantity}");
                continue;
            }

            orderDetails.Add(new OrderDetail
            {
                ProductId = productId,
                ProductAmount = quantity,
                Price = product.UnitPrice,
                Product = product
            });

            totalAmount += product.UnitPrice * quantity;
            Console.WriteLine($"Added to order. Current total: ${totalAmount:F2}");
        }

        if (orderDetails.Count == 0)
        {
            Console.WriteLine("No items in order. Order cancelled.");
            this.Pause();
            return;
        }

        // Create the order
        var order = new CustomerOrder
        {
            UserId = user.Id,
            OperationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            OrderStateId = 1, // New Order
            Details = orderDetails
        };

        this.context.CustomerOrders.Add(order);
        this.context.SaveChanges();

        // Reserve stock for each detail and update AvailableQuantity (Reserved only; Available is computed)
        foreach (var detail in orderDetails)
        {
            var product = this.context.Products.Find(detail.ProductId);
            if (product == null)
                continue;

            // Re-check availability
            if (detail.ProductAmount > product.AvailableQuantity)
            {
                Console.WriteLine($"⚠ Product {product.Id}: requested {detail.ProductAmount}, but only {product.AvailableQuantity} available now.");
                Console.WriteLine("Order creation aborted. No changes were applied.");
                this.context.CustomerOrders.Remove(order);
                this.context.SaveChanges();
                this.Pause();
                return;
            }

            product.ReservedQuantity += detail.ProductAmount;
            Console.WriteLine($"Reserved {detail.ProductAmount} unit(s) of product {product.Id}.");
        }

        this.context.SaveChanges();

        Console.WriteLine($"\nOrder created successfully!");
        Console.WriteLine($"Order ID: {order.Id}");
        Console.WriteLine($"Total amount: ${totalAmount:F2}");

        this.Pause();
    }

    /// <summary>
    /// Shows user's orders (tabular, fixed-width).
    /// </summary>
    public void ShowMyOrders()
    {
        Console.Clear();
        Console.WriteLine("=== MY ORDERS ===\n");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            this.Pause();
            return;
        }

        var orders = this.context.CustomerOrders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.Id)
            .ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders.");
            this.Pause();
            return;
        }

        // Header: ID(4) | Date(19) | Status(28) | Total(10)
        Console.WriteLine($"{"ID",4}  {"Date",19}  {"Status",-28}  {"Total",10}");
        Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 28 + 2 + 10));

        foreach (var order in orders)
        {
            // SQLite cannot aggregate decimal directly: double->decimal
            var total = (decimal)this.context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Select(d => (double)d.Price * d.ProductAmount)
                .Sum();

            var status = this.GetOrderStatusName(order.OrderStateId);
            Console.WriteLine($"{order.Id,4}  {order.OperationTime,19}  {status,-28}  {total,10:0.00}");
        }

        Console.WriteLine("\nTip: To cancel, open 'Cancel Order' and enter the ID from the first column.");
        this.Pause();
    }

    /// <summary>
    /// Cancels an order.
    /// </summary>
    public void CancelOrder()
    {
        Console.Clear();
        Console.WriteLine("=== CANCEL ORDER ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            this.Pause();
            return;
        }

        Console.Write("Enter Order ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            this.Pause();
            return;
        }

        var order = this.context.CustomerOrders
            .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

        if (order == null)
        {
            Console.WriteLine("Order not found or doesn't belong to you!");
            this.Pause();
            return;
        }

        if (order.OrderStateId != 1) // Only new orders can be cancelled by user
        {
            Console.WriteLine("This order cannot be cancelled (already processed).");
            this.Pause();
            return;
        }

        // Release stock reservations
        this.stockService.ReleaseOrderReservations(orderId);

        // Update order status
        order.OrderStateId = 2; // Cancelled by user
        this.context.SaveChanges();

        Console.WriteLine("Order cancelled successfully. Stock reservations released.");
        this.Pause();
    }

    /// <summary>
    /// Marks order as received.
    /// </summary>
    public void MarkOrderAsReceived()
    {
        Console.Clear();
        Console.WriteLine("=== MARK ORDER AS RECEIVED ===");

        var user = UserMenuController.CurrentUser;
        if (user == null)
        {
            Console.WriteLine("Please login first.");
            this.Pause();
            return;
        }

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            this.Pause();
            return;
        }

        var order = this.context.CustomerOrders
            .FirstOrDefault(o => o.Id == orderId && o.UserId == user.Id);

        if (order == null)
        {
            Console.WriteLine("Order not found or doesn't belong to you!");
            this.Pause();
            return;
        }

        if (order.OrderStateId != 7) // Only "Delivered to client" can be marked as received
        {
            Console.WriteLine("Order cannot be marked as received. It must be delivered first.");
            Console.WriteLine($"Current status: {this.GetOrderStatusName(order.OrderStateId)}");
            this.Pause();
            return;
        }

        // Confirm the sale (reduce stock)
        this.stockService.ConfirmOrderDelivery(orderId);

        // Update order status
        order.OrderStateId = 8; // Delivery confirmed by client
        this.context.SaveChanges();

        Console.WriteLine("Order marked as received. Thank you for your purchase!");
        this.Pause();
    }

    /// <summary>
    /// Gets order status name by ID.
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