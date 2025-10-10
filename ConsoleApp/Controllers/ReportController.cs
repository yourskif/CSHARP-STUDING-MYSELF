// Path: console-online-store/ConsoleApp/Controllers/ReportController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Globalization;
using System.IO;

using ConsoleApp.UI;

using StoreBLL.Services;

using StoreDAL.Data;

/// <summary>
/// Controller for report generation and data export.
/// </summary>
public sealed class ReportController
{
    private readonly StoreDbContext context;
    private readonly ReportService reportService;

    public ReportController(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.reportService = new ReportService(context);
    }

    public void Run()
    {
        var menu = new ConsoleMenuBuilder()
            .WithTitle("REPORTS & EXPORT")
            .WithHeaderColor(ConsoleColor.Magenta)
            .AddItem(ConsoleKey.D1, "Export Products to CSV", this.ExportProducts)
            .AddItem(ConsoleKey.D2, "Export Orders to CSV", this.ExportOrders)
            .AddSeparator()
            .AddItem(ConsoleKey.D3, "Sales Report", this.ShowSalesReport)
            .AddItem(ConsoleKey.D4, "Inventory Report", this.ShowInventoryReport)
            .AddSeparator()
            .AddItem(ConsoleKey.D5, "Export Sales Report to File", this.ExportSalesReport);

        menu.Run();
    }

    private void ExportProducts()
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PrintHeader("Export Products to CSV", ConsoleColor.Cyan);
        Console.WriteLine();

        try
        {
            var reportsDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportsDir);

            var fileName = $"products_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(reportsDir, fileName);

            ConsoleHelper.PrintInfo("Exporting products...");
            var result = this.reportService.ExportProductsToCsv(filePath);

            ConsoleHelper.PrintSuccess($"Products exported successfully!");
            ConsoleHelper.PrintInfo($"File location: {result}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Export failed: {ex.Message}");
        }

        ConsoleHelper.Pause();
    }

    private void ExportOrders()
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PrintHeader("Export Orders to CSV", ConsoleColor.Cyan);
        Console.WriteLine();

        try
        {
            var reportsDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportsDir);

            var fileName = $"orders_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(reportsDir, fileName);

            ConsoleHelper.PrintInfo("Exporting orders...");
            var result = this.reportService.ExportOrdersToCsv(filePath);

            ConsoleHelper.PrintSuccess($"Orders exported successfully!");
            ConsoleHelper.PrintInfo($"File location: {result}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Export failed: {ex.Message}");
        }

        ConsoleHelper.Pause();
    }

    private void ShowSalesReport()
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PrintHeader("Sales Report", ConsoleColor.Green);
        Console.WriteLine();

        Console.Write("Start date (yyyy-MM-dd) or Enter for 30 days ago: ");
        var startInput = Console.ReadLine();
        DateTime startDate;

        if (string.IsNullOrWhiteSpace(startInput))
        {
            startDate = DateTime.Now.AddDays(-30);
        }
        else if (!DateTime.TryParseExact(startInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
        {
            ConsoleHelper.PrintError("Invalid date format");
            ConsoleHelper.Pause();
            return;
        }

        Console.Write("End date (yyyy-MM-dd) or Enter for today: ");
        var endInput = Console.ReadLine();
        DateTime endDate;

        if (string.IsNullOrWhiteSpace(endInput))
        {
            endDate = DateTime.Now;
        }
        else if (!DateTime.TryParseExact(endInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        {
            ConsoleHelper.PrintError("Invalid date format");
            ConsoleHelper.Pause();
            return;
        }

        Console.WriteLine();
        ConsoleHelper.PrintInfo("Generating report...");

        var report = this.reportService.GenerateSalesReport(startDate, endDate);

        Console.WriteLine();
        ConsoleHelper.PrintSeparator();
        Console.WriteLine($"Period: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
        ConsoleHelper.PrintSeparator();
        Console.WriteLine();

        Console.WriteLine("SUMMARY");
        ConsoleHelper.PrintSeparator(40, '─');
        Console.WriteLine($"Total Orders:         {report.TotalOrders}");
        Console.WriteLine($"Total Revenue:        ${report.TotalRevenue:F2}");
        Console.WriteLine($"Average Order Value:  ${report.AverageOrderValue:F2}");
        Console.WriteLine();

        if (report.TopProducts.Count > 0)
        {
            Console.WriteLine("TOP 10 PRODUCTS");
            ConsoleHelper.PrintSeparator(60, '─');
            ConsoleHelper.PrintTableRow(
                ("Rank", 6),
                ("Product", 30),
                ("Quantity", 10),
                ("Revenue", 12));
            ConsoleHelper.PrintSeparator(60, '─');

            int rank = 1;
            foreach (var product in report.TopProducts)
            {
                ConsoleHelper.PrintTableRow(
                    (rank.ToString(CultureInfo.InvariantCulture), 6),
                    (product.ProductName, 30),
                    (product.QuantitySold.ToString(CultureInfo.InvariantCulture), 10),
                    ($"${product.Revenue:F2}", 12));
                rank++;
            }
        }

        Console.WriteLine();
        ConsoleHelper.Pause();
    }

    private void ShowInventoryReport()
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PrintHeader("Inventory Report", ConsoleColor.Yellow);
        Console.WriteLine();

        ConsoleHelper.PrintInfo("Generating inventory report...");
        var report = this.reportService.GenerateInventoryReport();

        Console.WriteLine();
        ConsoleHelper.PrintSeparator();
        Console.WriteLine("INVENTORY SUMMARY");
        ConsoleHelper.PrintSeparator(40, '─');
        Console.WriteLine($"Total Products:    {report.TotalProducts}");
        Console.WriteLine($"Total Stock:       {report.TotalStock}");
        Console.WriteLine($"Total Reserved:    {report.TotalReserved}");
        Console.WriteLine($"Total Available:   {report.TotalAvailable}");
        Console.WriteLine();

        if (report.LowStockProducts.Count > 0)
        {
            ConsoleHelper.PrintWarning($"LOW STOCK ALERT ({report.LowStockProducts.Count} products)");
            ConsoleHelper.PrintSeparator(70, '─');
            ConsoleHelper.PrintTableRow(
                ("ID", 5),
                ("Product", 30),
                ("Stock", 8),
                ("Reserved", 10),
                ("Available", 10));
            ConsoleHelper.PrintSeparator(70, '─');

            foreach (var product in report.LowStockProducts)
            {
                Console.ForegroundColor = product.Available < 5 ? ConsoleColor.Red : ConsoleColor.Yellow;
                ConsoleHelper.PrintTableRow(
                    (product.ProductId.ToString(CultureInfo.InvariantCulture), 5),
                    (product.ProductName, 30),
                    (product.Stock.ToString(CultureInfo.InvariantCulture), 8),
                    (product.Reserved.ToString(CultureInfo.InvariantCulture), 10),
                    (product.Available.ToString(CultureInfo.InvariantCulture), 10));
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine("STOCK BY CATEGORY");
        ConsoleHelper.PrintSeparator(70, '─');
        ConsoleHelper.PrintTableRow(
            ("Category", 20),
            ("Products", 10),
            ("Stock", 10),
            ("Reserved", 10),
            ("Available", 10));
        ConsoleHelper.PrintSeparator(70, '─');

        foreach (var category in report.StockByCategory)
        {
            ConsoleHelper.PrintTableRow(
                (category.CategoryName, 20),
                (category.ProductCount.ToString(CultureInfo.InvariantCulture), 10),
                (category.TotalStock.ToString(CultureInfo.InvariantCulture), 10),
                (category.TotalReserved.ToString(CultureInfo.InvariantCulture), 10),
                (category.TotalAvailable.ToString(CultureInfo.InvariantCulture), 10));
        }

        Console.WriteLine();
        ConsoleHelper.Pause();
    }

    private void ExportSalesReport()
    {
        ConsoleHelper.ClearScreen();
        ConsoleHelper.PrintHeader("Export Sales Report", ConsoleColor.Cyan);
        Console.WriteLine();

        Console.Write("Start date (yyyy-MM-dd) or Enter for 30 days ago: ");
        var startInput = Console.ReadLine();
        DateTime startDate;

        if (string.IsNullOrWhiteSpace(startInput))
        {
            startDate = DateTime.Now.AddDays(-30);
        }
        else if (!DateTime.TryParseExact(startInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
        {
            ConsoleHelper.PrintError("Invalid date format");
            ConsoleHelper.Pause();
            return;
        }

        Console.Write("End date (yyyy-MM-dd) or Enter for today: ");
        var endInput = Console.ReadLine();
        DateTime endDate;

        if (string.IsNullOrWhiteSpace(endInput))
        {
            endDate = DateTime.Now;
        }
        else if (!DateTime.TryParseExact(endInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        {
            ConsoleHelper.PrintError("Invalid date format");
            ConsoleHelper.Pause();
            return;
        }

        try
        {
            var reportsDir = Path.Combine(AppContext.BaseDirectory, "Reports");
            Directory.CreateDirectory(reportsDir);

            ConsoleHelper.PrintInfo("Generating report...");
            var report = this.reportService.GenerateSalesReport(startDate, endDate);

            var fileName = $"sales_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(reportsDir, fileName);

            ConsoleHelper.PrintInfo("Exporting to file...");
            var result = this.reportService.ExportSalesReportToText(report, filePath);

            ConsoleHelper.PrintSuccess($"Sales report exported successfully!");
            ConsoleHelper.PrintInfo($"File location: {result}");
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintError($"Export failed: {ex.Message}");
        }

        ConsoleHelper.Pause();
    }
}
