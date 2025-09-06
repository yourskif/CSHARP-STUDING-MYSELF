// ConsoleApp/Controllers/AdminOrderController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;

public sealed class AdminOrderController
{
    private readonly StoreDbContext db;

    // Allowed transitions (names must match DB):
    // New → Confirmed | Canceled by User | Canceled by Admin
    // Confirmed → Moved to Delivery | Canceled by Admin
    // Moved to Delivery → In Delivery
    // In Delivery → Delivered to Client
    // Delivered to Client → Confirmed by Client
    // Canceled* and Confirmed by Client are terminal
    private static readonly Dictionary<string, string[]> AllowedTransitions =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["New"] = new[] { "Confirmed", "Canceled by User", "Canceled by Admin" },
            ["Confirmed"] = new[] { "Moved to Delivery", "Canceled by Admin" },
            ["Moved to Delivery"] = new[] { "In Delivery" },
            ["In Delivery"] = new[] { "Delivered to Client" },
            ["Delivered to Client"] = new[] { "Confirmed by Client" },
            ["Canceled by User"] = Array.Empty<string>(),
            ["Canceled by Admin"] = Array.Empty<string>(),
            ["Confirmed by Client"] = Array.Empty<string>(),
        };

    public AdminOrderController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    // ===================== MAIN LIST =====================

    public void ShowOrders()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== All Orders ===");

            var orders = this.db.CustomerOrders
                .Include(o => o.User)
                .Include(o => o.State)
                .OrderBy(o => o.Id)
                .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                Console.WriteLine("\n[C] Create order   [P] Products   [Q/Esc] Back");
                var key0 = Console.ReadKey(true).Key;
                switch (key0)
                {
                    case ConsoleKey.C:
                        this.CreateOrderInteractive();
                        break;
                    case ConsoleKey.P:
                        this.ShowProducts();
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        return;
                }

                continue;
            }

            Console.WriteLine("# | OrderId | Customer | State | Created (UTC)");
            Console.WriteLine("------------------------------------------------");
            var iRow = 1;
            foreach (var o in orders)
            {
                var login = o.User?.Login ?? "(unknown)";
                var state = o.State?.Name ?? "(unknown)";
                Console.WriteLine($"{iRow,2} | {o.Id,7} | {login,-8} | {state,-17} | {o.OperationTimeUtc:yyyy-MM-dd HH:mm:ss}");
                iRow++;
            }

            Console.WriteLine();
            Console.WriteLine("[V] View details   [S] Change status   [X] Cancel by admin   [C] Create order   [P] Products   [Esc] Back");

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.V:
                    this.ViewDetailsInteractive();
                    break;

                case ConsoleKey.S:
                    this.ChangeOrderStatus();
                    break;

                case ConsoleKey.X:
                    this.CancelOrder();
                    break;

                case ConsoleKey.C:
                    this.CreateOrderInteractive();
                    break;

                case ConsoleKey.P:
                    this.ShowProducts();
                    break;

                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    // ===================== COMPATIBILITY WRAPPERS (used by handlers) =====================

    // Старі виклики з хендлера без параметрів
    public void ShowOrderDetails() => this.ViewDetailsInteractive();

    // Старий виклик створення без параметрів
    public void CreateOrder() => this.CreateOrderInteractive();

    // Нові перевантаження для сумісності з OrderContextMenuHandler:
    // Показ деталей за конкретним Id (без запиту Id у користувача)
    public void ShowOrderDetails(int orderId)
    {
        var order = this.db.CustomerOrders
            .Include(o => o.User)
            .Include(o => o.State)
            .FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            Console.WriteLine($"Order #{orderId} not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var items = this.db.OrderDetails
            .Include(d => d.Product)
            .ThenInclude(p => p.Title)
            .Where(d => d.CustomerOrderId == order.Id)
            .OrderBy(d => d.Id)
            .ToList();

        Console.WriteLine();
        Console.WriteLine($"=== Order #{order.Id} details (state: {order.State?.Name ?? "?"}) ===");
        if (items.Count == 0)
        {
            Console.WriteLine("No items.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("# | Product | UnitPrice | Qty | Line Total");
        Console.WriteLine("-------------------------------------------");
        var i = 1;
        decimal total = 0;
        foreach (var it in items)
        {
            var title = it.Product?.Title?.Title ?? it.Product?.Description ?? "(no title)";
            var line = it.UnitPrice * it.Quantity;
            total += line;
            Console.WriteLine($"{i,2} | {title,-20} | {it.UnitPrice,9} | {it.Quantity,3} | {line,10}");
            i++;
        }

        Console.WriteLine("-------------------------------------------");
        Console.WriteLine($"TOTAL: {total}");
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    // Створити замовлення за вхідними параметрами, повернути Id (0 при помилці)
    public int CreateOrder(int productId, int quantity, string login)
    {
        if (quantity <= 0) quantity = 1;

        var product = this.db.Products.FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            Console.WriteLine($"Product #{productId} not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return 0;
        }

        if (product.Stock < quantity)
        {
            Console.WriteLine($"Not enough stock for product #{productId}. Available: {product.Stock}.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return 0;
        }

        if (string.IsNullOrWhiteSpace(login)) login = "user";
        var customer = this.db.Users.FirstOrDefault(u => u.Login == login);
        if (customer is null)
        {
            Console.WriteLine($"User '{login}' not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return 0;
        }

        var newStateId = this.db.OrderStates
            .Where(s => s.Name == "New")
            .Select(s => s.Id)
            .FirstOrDefault();

        if (newStateId == 0)
        {
            Console.WriteLine("State 'New' not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return 0;
        }

        var order = new CustomerOrder
        {
            UserId = customer.Id,
            StateId = newStateId,
            OperationTimeUtc = DateTime.UtcNow,
        };
        this.db.CustomerOrders.Add(order);
        this.db.SaveChanges();

        var detail = new OrderDetail
        {
            CustomerOrderId = order.Id,
            ProductId = product.Id,
            UnitPrice = product.UnitPrice,
            Quantity = quantity,
        };
        this.db.OrderDetails.Add(detail);

        product.Stock -= quantity;
        this.db.SaveChanges();

        Console.WriteLine($"Order #{order.Id} created for user '{customer.Login}' with {quantity} item(s) of product #{product.Id}.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);

        return order.Id;
    }

    // ===================== INTERACTIVE CREATE / DETAILS =====================

    private void CreateOrderInteractive()
    {
        Console.Clear();
        Console.WriteLine("=== Products ===");

        var products = this.db.Products
            .Include(p => p.Title)
            .OrderBy(p => p.Id)
            .ToList();

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("# | Id | Title                | Price | Stock");
        Console.WriteLine("----------------------------------------------");
        var i = 1;
        foreach (var p in products)
        {
            var titleText = p.Title?.Title ?? p.Description ?? "(no title)";
            Console.WriteLine($"{i,2} | {p.Id,2} | {titleText,-20} | {p.UnitPrice,5} | {p.Stock,5}");
            i++;
        }

        Console.Write("\nEnter Product Id (blank = back): ");
        var rawId = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(rawId)) return;

        if (!int.TryParse(rawId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var productId))
        {
            Console.WriteLine("Invalid product Id.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.Write("Quantity [1]: ");
        var rawQty = Console.ReadLine();
        var qty = 1;
        if (!string.IsNullOrWhiteSpace(rawQty) &&
            !int.TryParse(rawQty, NumberStyles.Integer, CultureInfo.InvariantCulture, out qty))
        {
            Console.WriteLine("Invalid quantity.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.Write("Customer login [user]: ");
        var login = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(login)) login = "user";

        _ = this.CreateOrder(productId, qty, login);
    }

    private void ViewDetailsInteractive()
    {
        Console.Write("Enter Order Id (blank = back): ");
        var raw = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(raw)) return;

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderId))
        {
            Console.WriteLine("Invalid Id.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        this.ShowOrderDetails(orderId);
    }

    // ===================== STATUS UPDATE / CANCEL =====================

    public void ChangeOrderStatus()
    {
        Console.Write("Enter Order Id (blank = back): ");
        var raw = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(raw)) return;

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderId))
        {
            Console.WriteLine("Invalid Id.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        this.ChangeOrderStatus(orderId);
    }

    public void ChangeOrderStatus(int orderId)
    {
        var order = this.db.CustomerOrders
            .Include(o => o.State)
            .FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            Console.WriteLine($"Order #{orderId} not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var current = order.State?.Name ?? string.Empty;
        var allowedNames = GetAllowedNextStates(current);
        if (allowedNames.Length == 0)
        {
            Console.WriteLine($"No transitions allowed from '{current}'.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var allowedStates = this.db.OrderStates
            .Where(s => allowedNames.Contains(s.Name))
            .OrderBy(s => s.Id)
            .ToList();

        Console.WriteLine($"Current state: {current}\n");
        Console.WriteLine("Allowed next states:");
        foreach (var s in allowedStates)
        {
            Console.WriteLine($"  {s.Id}. {s.Name}");
        }

        Console.Write("\nEnter new state Id: ");
        if (!int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var newStateId))
        {
            Console.WriteLine("Invalid state Id.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var target = allowedStates.FirstOrDefault(s => s.Id == newStateId);
        if (target is null)
        {
            Console.WriteLine("Selected state is not allowed from the current state.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        order.StateId = target.Id;
        order.OperationTimeUtc = DateTime.UtcNow;
        this.db.SaveChanges();

        Console.WriteLine($"Order #{order.Id} state changed to '{target.Name}'.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    public void CancelOrder()
    {
        Console.Write("Enter Order Id to cancel (blank = back): ");
        var raw = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(raw)) return;

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var orderId))
        {
            Console.WriteLine("Invalid Id.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        this.CancelOrder(orderId);
    }

    public void CancelOrder(int orderId)
    {
        var order = this.db.CustomerOrders
            .Include(o => o.State)
            .FirstOrDefault(o => o.Id == orderId);

        if (order is null)
        {
            Console.WriteLine($"Order #{orderId} not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var current = order.State?.Name ?? string.Empty;
        var targetName = "Canceled by Admin";

        if (!IsTransitionAllowed(current, targetName))
        {
            Console.WriteLine($"Transition '{current}' → '{targetName}' is not allowed.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        var target = this.db.OrderStates.FirstOrDefault(s => s.Name == targetName);
        if (target is null)
        {
            Console.WriteLine($"State '{targetName}' not found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            return;
        }

        order.StateId = target.Id;
        order.OperationTimeUtc = DateTime.UtcNow;
        this.db.SaveChanges();

        Console.WriteLine($"Order #{order.Id} has been canceled by admin.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }

    // ===================== AUX =====================

    public void ShowProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Products ===");

        var products = this.db.Products
            .Include(p => p.Title)
            .OrderBy(p => p.Id)
            .ToList();

        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
            return;
        }

        Console.WriteLine("# | Id | Title                | Price | Stock");
        Console.WriteLine("----------------------------------------------");
        var i = 1;
        foreach (var p in products)
        {
            var titleText = p.Title?.Title ?? p.Description ?? "(no title)";
            Console.WriteLine($"{i,2} | {p.Id,2} | {titleText,-20} | {p.UnitPrice,5} | {p.Stock,5}");
            i++;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    private static string[] GetAllowedNextStates(string currentStateName)
        => AllowedTransitions.TryGetValue(currentStateName ?? string.Empty, out var next)
           ? next
           : Array.Empty<string>();

    private static bool IsTransitionAllowed(string currentStateName, string targetStateName)
        => GetAllowedNextStates(currentStateName).Contains(targetStateName, StringComparer.OrdinalIgnoreCase);
}
