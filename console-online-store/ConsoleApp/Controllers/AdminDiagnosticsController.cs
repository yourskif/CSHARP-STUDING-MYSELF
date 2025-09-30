// Path: console-online-store/ConsoleApp/Controllers/AdminDiagnosticsController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Globalization;
using System.Linq;

using StoreBLL.Services;

using StoreDAL.Data;

/// <summary>
/// Admin diagnostics controller (refactored).
/// Thin UI layer that delegates business logic to specialized services.
/// Provides menu navigation and output formatting for diagnostic operations.
/// </summary>
public sealed class AdminDiagnosticsController
{
    private readonly StoreDbContext db;
    private readonly InventoryDiagnosticsService inventoryService;
    private readonly OrderDiagnosticsService orderService;
    private readonly UserDiagnosticsService userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminDiagnosticsController"/> class.
    /// </summary>
    /// <param name="db">Database context for diagnostics operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    public AdminDiagnosticsController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
        this.inventoryService = new InventoryDiagnosticsService(db);
        this.orderService = new OrderDiagnosticsService(db);
        this.userService = new UserDiagnosticsService(db);
    }

    /// <summary>
    /// Main menu loop for diagnostics operations.
    /// Displays menu and handles user navigation.
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
                Console.WriteLine("=== ADMIN: DIAGNOSTICS ===");
                Console.WriteLine($"UTC Now: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine(new string('-', 78));

                try
                {
                    this.PrintOverview();
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

                this.HandleMenuChoice(key);
            }
        }
        finally
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = prev;
        }
    }

    // -------- Private UI methods --------

    private void HandleMenuChoice(ConsoleKey key)
    {
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

    private void PrintOverview()
    {
        var (open, closed) = this.orderService.GetOrderCounts();

        Console.WriteLine("Overview");
        Console.WriteLine($"Users total........ {this.userService.GetTotalUserCount()}");
        Console.WriteLine($"Products total..... {this.inventoryService.GetTotalProductCount()}");
        Console.WriteLine($"Orders total....... {open + closed}");
        Console.WriteLine($"Open / Closed...... {open} / {closed}");
        Console.WriteLine();

        Console.WriteLine($"Low availability (threshold: 5)");
        var lowStock = this.inventoryService.GetLowAvailability(5).ToList();
        if (lowStock.Count == 0)
        {
            Console.WriteLine("  No low availability alerts");
        }
        else
        {
            foreach (var p in lowStock)
            {
                Console.WriteLine($"  #{p.Id,-3} {Trunc(p.Title, 30),-30} | Stock:{p.Stock,5} | Reserved:{p.Reserved,5} | Available:{p.Available,5}");
            }
        }

        Console.WriteLine();
    }

    private void ShowProductsSnapshot()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: PRODUCTS SNAPSHOT ===\n");

        var products = this.inventoryService.GetProductsSnapshot().ToList();

        Console.WriteLine($"{"ID",3}  {"Title",-30}  {"SKU",-14}  {"Price",12}  {"Stock",7}  {"Reserved",9}  {"Available",10}");
        Console.WriteLine(new string('-', 3 + 2 + 30 + 2 + 14 + 2 + 12 + 2 + 7 + 2 + 9 + 2 + 10));

        foreach (var p in products)
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
            int updated = this.inventoryService.RebuildReservedFromOpenOrders();
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
            int updated = this.inventoryService.ClearAllReservations();
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

        var anomalies = this.inventoryService.GetAnomalies().ToList();

        if (anomalies.Count == 0)
        {
            Console.WriteLine("No anomalies found.");
        }
        else
        {
            foreach (var p in anomalies)
            {
                Console.WriteLine($"#{p.Id,3}  {Trunc(p.Title, 30),-30} | Stock:{p.Stock,5} | Reserved:{p.Reserved,5} | Available:{p.Available,5}");
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

        var (closed, zeroed) = this.orderService.ResetDemo();
        Console.WriteLine($"Closed open orders: {closed}");
        Console.WriteLine($"Zeroed reservations for products: {zeroed}");
        Console.WriteLine("Demo state has been reset.");

        Pause();
    }

    private void OrdersSnapshot()
    {
        Console.Clear();
        Console.WriteLine("=== DIAGNOSTICS: ORDERS SNAPSHOT ===\n");

        var orders = this.orderService.GetOrdersSnapshot().ToList();

        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
            Pause();
            return;
        }

        Console.WriteLine($"{"ID",4}  {"Date",19}  {"User",-20}  {"Status",-28}  {"Total",10}");
        Console.WriteLine(new string('-', 4 + 2 + 19 + 2 + 20 + 2 + 28 + 2 + 10));

        foreach (var o in orders)
        {
            Console.WriteLine($"{o.Id,4}  {o.Date,19}  {o.UserName,-20}  {StatusName(o.OrderStateId),-28}  {o.Total,10:0.00}");
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

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            Pause();
            return;
        }

        if (this.orderService.CancelOrderById(id))
        {
            Console.WriteLine($"Order {id} cancelled by admin and reservations released.");
        }
        else
        {
            Console.WriteLine("Order not found or cannot be cancelled (already in final state).");
        }

        Pause();
    }

    private void ShowUsersHash()
    {
        Console.Clear();
        Console.WriteLine("=== USERS (hash check) ===\n");

        var users = this.userService.GetUsersWithHashInfo().ToList();

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
            Console.WriteLine($"{u.Id,4}  {u.Login,-16}  {u.RoleId,4}  {(u.IsHashed ? "YES" : "NO"),-6}  {u.PasswordPreview,-28}");
        }

        Pause();
    }

    private void ResetDefaultAdmin()
    {
        Console.Clear();
        Console.WriteLine("=== RESET DEFAULT ADMIN ===\n");

        var admin = this.userService.ResetDefaultAdmin();

        Console.WriteLine($"Admin fixed: Id={admin.Id}, RoleId={admin.RoleId}, Hash={(admin.IsHashed ? "YES" : "NO")}");
        Console.WriteLine("Use credentials: admin / Admin@123");

        Pause();
    }

    private void SeedDemoOrders()
    {
        Console.Clear();
        Console.WriteLine("=== SEED DEMO ORDERS ===\n");

        int created = this.orderService.SeedDemoOrders();

        if (created == 0)
        {
            Console.WriteLine("Not enough products to seed.");
        }
        else
        {
            Console.WriteLine($"Created {created} demo order(s).");
            Console.WriteLine("Open Orders snapshot to verify.");
        }

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

        this.orderService.ClearAllOrders();
        Console.WriteLine("All orders removed. Reservations reset to 0.");

        Pause();
    }

    // -------- Static helpers --------

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
        return string.Concat(s.AsSpan(0, take), "…");
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

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
