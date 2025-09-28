// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Controllers\AdminDiagnosticsController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StoreBLL.Services;
using StoreDAL.Data;
using DalCustomerOrder = StoreDAL.Entities.CustomerOrder;
using DalOrderDetail = StoreDAL.Entities.OrderDetail;
using DalProduct = StoreDAL.Entities.Product;
using DalUser = StoreDAL.Entities.User;

// Use ConsoleApp.Helpers factory (has hashing + EnsureDefaultAdmin)
using AppStoreDbFactory = ConsoleApp.Helpers.StoreDbFactory;

/// <summary>
/// Enhanced admin diagnostics controller with comprehensive analytics and reporting capabilities.
/// Provides inventory management, sales analytics, stock reports, and user activity monitoring.
/// </summary>
public sealed class AdminDiagnosticsController
{
    /// <summary>
    /// Open order states for inventory tracking.
    /// </summary>
    private static readonly int[] OpenStates = { 1, 4, 5, 6 };

    /// <summary>
    /// Database context for operations.
    /// </summary>
    private readonly StoreDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminDiagnosticsController"/> class.
    /// </summary>
    /// <param name="db">Database context for operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    public AdminDiagnosticsController(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Main entry point for diagnostics with enhanced analytics menu.
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
                Console.WriteLine("=== ADMIN: DIAGNOSTICS & ANALYTICS ===");
                Console.WriteLine($"UTC Now: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine(new string('-', 78));

                try
                {
                    this.PrintCounts();
                    this.PrintLowAvailable(5);
                    Console.WriteLine(new string('-', 78));
                    Console.WriteLine("INVENTORY MANAGEMENT:");
                    Console.WriteLine("[1] Products snapshot");
                    Console.WriteLine("[2] Rebuild Reserved from OPEN orders");
                    Console.WriteLine("[3] CLEAR ALL reservations (demo reset)");
                    Console.WriteLine("[4] Find stock anomalies (Reserved>Stock / Available<0)");
                    Console.WriteLine("[5] RESET DEMO (close open orders + zero reservations)");
                    Console.WriteLine();
                    Console.WriteLine("ORDER MANAGEMENT:");
                    Console.WriteLine("[6] Orders snapshot (table)");
                    Console.WriteLine("[7] Admin cancel order by ID");
                    Console.WriteLine();
                    Console.WriteLine("USER MANAGEMENT:");
                    Console.WriteLine("[8] Users: Show (hash check)");
                    Console.WriteLine("[9] Users: Reset default admin (admin / Admin@123)");
                    Console.WriteLine();
                    Console.WriteLine("ANALYTICS & REPORTS:");
                    Console.WriteLine("[A] Sales Analytics");
                    Console.WriteLine("[S] Stock Report");
                    Console.WriteLine("[U] User Activity Report");
                    Console.WriteLine("[P] Product Performance Report");
                    Console.WriteLine();
                    Console.WriteLine("DATA MANAGEMENT:");
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

                    case ConsoleKey.A:
                        this.ShowSalesAnalytics();
                        break;

                    case ConsoleKey.S:
                        this.ShowStockReport();
                        break;

                    case ConsoleKey.U:
                        this.ShowUserActivityReport();
                        break;

                    case ConsoleKey.P:
                        this.ShowProductPerformanceReport();
                        break;

                    case ConsoleKey.C:
                        this.ClearAllOrders();
                        break;
                }
            }
        }
        finally
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = prev;
        }
    }

    /// <summary>
    /// Displays comprehensive sales analytics with revenue breakdown and trends.
    /// </summary>
    private void ShowSalesAnalytics()
    {
        Console.Clear();
        Console.WriteLine("=== SALES ANALYTICS DASHBOARD ===\n");

        try
        {
            var completedOrders = this.db.Set<DalCustomerOrder>()
                .Where(o => o.OrderStateId == 8) // Delivery confirmed
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .ThenInclude(p => p.Title)
                .ToList();

            if (!completedOrders.Any())
            {
                Console.WriteLine("❌ No completed orders found.");
                Console.WriteLine("💡 Complete some orders first to see analytics.");
                Pause();
                return;
            }

            var totalOrders = completedOrders.Count;
            var totalRevenue = completedOrders.SelectMany(o => o.Details).Sum(d => d.Price * d.ProductAmount);
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Product sales analysis
            var productSales = new Dictionary<int, (string name, int quantity, decimal revenue)>();

            foreach (var order in completedOrders)
            {
                foreach (var detail in order.Details)
                {
                    var productName = detail.Product?.Title?.Title ?? $"Product {detail.ProductId}";
                    var detailRevenue = detail.Price * detail.ProductAmount;

                    if (productSales.ContainsKey(detail.ProductId))
                    {
                        var current = productSales[detail.ProductId];
                        productSales[detail.ProductId] = (current.name,
                            current.quantity + detail.ProductAmount,
                            current.revenue + detailRevenue);
                    }
                    else
                    {
                        productSales[detail
