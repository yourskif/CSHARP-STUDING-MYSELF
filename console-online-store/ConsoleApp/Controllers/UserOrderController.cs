// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Controllers\UserOrderController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Interfaces;
using StoreBLL.Services;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

/// <summary>
/// Controller for user order management operations.
/// </summary>
public class UserOrderController
{
    private readonly IStoreUnitOfWork unitOfWork;
    private readonly StockReservationService stockService;

#pragma warning disable CA1859 // Keep interface type for testability and loose coupling (intentional)
    private readonly ICustomerOrderService orderService;
#pragma warning restore CA1859

    /// <summary>
    /// Initializes a new instance of the <see cref="UserOrderController"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    public UserOrderController(IStoreUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        this.unitOfWork = unitOfWork;
        this.stockService = new StockReservationService(unitOfWork);
        this.orderService = new CustomerOrderService(unitOfWork);
    }

    /// <summary>
    /// Overloaded method for compatibility with UserMainMenu.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    public void ShowOrderMenu(IStoreUnitOfWork unitOfWork)
    {
        this.ShowOrderMenu();
    }

    /// <summary>
    /// Shows the order management menu.
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
    /// Creates a new order with atomic stock reservation.
    /// Uses database transaction to prevent race conditions.
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

        // Phase 1: Collect order items WITHOUT making any reservations
        while (true)
        {
            Console.Write("\nEnter Product ID (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int productId) || productId == 0)
            {
                break;
            }

            // Use AsNoTracking for read-only check
            var product = this.unitOfWork.Context.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                Console.WriteLine("Product not found!");
                continue;
            }

            var safeTitle = product.Title?.Title ?? $"Product {product.Id}";
            Console.WriteLine($"Product: {safeTitle} - Price: ${product.UnitPrice:F2}");
            Console.WriteLine($"Available stock: {product.AvailableQuantity}");

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity!");
                continue;
            }

            orderDetails.Add(new OrderDetail
            {
                ProductId = productId,
                ProductAmount = quantity,
                Price = product.UnitPrice,
            });

            totalAmount += product.UnitPrice * quantity;
            Console.WriteLine($"Added to order. Current total: ${totalAmount:F2}");
        }

        if (orderDetails.Count == 0)
        {
            Console.WriteLine("No items in order. Order cancelled.");
            Pause();
            return;
        }

        // Phase 2: ATOMIC order creation with stock reservation
        // Using transaction to ensure all-or-nothing operation
        this.unitOfWork.BeginTransaction();
        try
        {
            // Check and reserve ALL products atomically
            foreach (var detail in orderDetails)
            {
                // Get fresh product data within transaction
                var product = this.unitOfWork.Context.Products
                    .FirstOrDefault(p => p.Id == detail.ProductId);

                if (product == null)
                {
                    this.unitOfWork.Rollback();
                    Console.WriteLine($"✖ Product {detail.ProductId} no longer exists.");
                    Console.WriteLine("Order creation aborted.");
                    Pause();
                    return;
                }

                // Critical check: verify stock availability
                if (detail.ProductAmount > product.AvailableQuantity)
                {
                    this.unitOfWork.Rollback();
                    Console.WriteLine($"✖ Product {product.Id}: requested {detail.ProductAmount}, " +
                                    $"but only {product.AvailableQuantity} available now.");
                    Console.WriteLine("Order creation aborted. No changes were applied.");
                    Pause();
                    return;
                }

                // Reserve stock
                product.ReservedQuantity += detail.ProductAmount;
                Console.WriteLine($"Reserved {detail.ProductAmount} unit(s) of product {product.Id}.");
            }

            // Create order entity
            var order = new CustomerOrder
            {
                UserId = user.Id,
                OperationTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                OrderStateId = 1, // New Order
                Details = orderDetails,
            };

            this.unitOfWork.Context.CustomerOrders.Add(order);
            this.unitOfWork.SaveChanges();
            this.unitOfWork.Commit();

            Console.WriteLine($"\n✓ Order created successfully!");
            Console.WriteLine($"Order ID: {order.Id}");
            Console.WriteLine($"Total amount: ${totalAmount:F2}");
        }
        catch (Exception ex)
        {
            this.unitOfWork.Rollback();
            Console.WriteLine($"✗ Error creating order: {ex.Message}");
        }

        Pause();
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
            Pause();
            return;
        }

        var orders = this.unitOfWork.Context.CustomerOrders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.Id)
            .ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders.");
            Pause();
            return;
        }

        // Header: ID(4) | Date(19) | Status(28) | Total(10)
        Console.WriteLine($"{"ID",4}  {"Date",19}  {"Status",-28}  {"Total",10}");
        Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 28 + 2 + 10));

        foreach (var order in orders)
        {
            // SQLite cannot aggregate decimal directly: double -> decimal
            var total = (decimal)this.unitOfWork.Context.OrderDetails
                .Where(d => d.OrderId == order.Id)
                .Select(d => (double)d.Price * d.ProductAmount)
                .Sum();

            var status = GetOrderStatusName(order.OrderStateId);
            Console.WriteLine($"{order.Id,4}  {order.OperationTime,19}  {status,-28}  {total,10:0.00}");
        }

        Console.WriteLine("\nTip: To cancel, open 'Cancel Order' and enter the ID from the first column.");
        Pause();
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
            Pause();
            return;
        }

        Console.Write("Enter Order ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            Pause();
            return;
        }

        // Delegate business rules to service; release reservations on success.
        if (this.orderService.CancelOwnOrder(orderId, user.Id, out var error))
        {
            this.stockService.ReleaseOrderReservations(orderId);
            Console.WriteLine("Order cancelled successfully. Stock reservations released.");
        }
        else
        {
            Console.WriteLine(error);
        }

        Pause();
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
            Pause();
            return;
        }

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid order ID!");
            Pause();
            return;
        }

        // Delegate business rules to service; confirm stock on success.
        if (this.orderService.MarkAsReceived(orderId, user.Id, out var error))
        {
            this.stockService.ConfirmOrderDelivery(orderId);
            Console.WriteLine("Order marked as received. Thank you for your purchase!");
        }
        else
        {
            Console.WriteLine(error);
        }

        Pause();
    }

    /// <summary>
    /// Gets order status name by ID.
    /// </summary>
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
    /// Pauses for user input.
    /// </summary>
    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}
