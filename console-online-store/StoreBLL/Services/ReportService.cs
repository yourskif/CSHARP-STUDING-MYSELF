''// Path: console-online-store/StoreBLL/Services/ReportService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;

/// <summary>
/// Service for generating reports and exporting data.
/// </summary>
public class ReportService
{
    private readonly StoreDbContext context;

    public ReportService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Exports all products to CSV file.
    /// </summary>
    public string ExportProductsToCsv(string filePath)
    {
        var products = this.context.Products
            .Include(p => p.Title)
            .ThenInclude(t => t!.Category)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("ID,Title,Category,Manufacturer,Price,Stock,Reserved,Available");

        foreach (var p in products)
        {
            var title = p.Title?.Title ?? "N/A";
            var category = p.Title?.Category?.Name ?? "N/A";
            var manufacturer = p.Manufacturer?.Name ?? "N/A";
            var available = p.StockQuantity - p.ReservedQuantity;

            sb.AppendLine($"{p.Id},\"{title}\",\"{category}\",\"{manufacturer}\",{p.UnitPrice},{p.StockQuantity},{p.ReservedQuantity},{available}");
        }

        File.WriteAllText(filePath, sb.ToString());
        return filePath;
    }

    /// <summary>
    /// Exports all orders to CSV file.
    /// </summary>
    public string ExportOrdersToCsv(string filePath)
    {
        var orders = this.context.CustomerOrders
            .Include(o => o.User)
            .Include(o => o.Details)
            .AsNoTracking()
            .ToList();

        var sb = new StringBuilder();
        sb.AppendLine("OrderID,UserID,UserName,OrderDate,StateID,TotalAmount");

        foreach (var order in orders)
        {
            var userName = $"{order.User?.Name} {order.User?.LastName}".Trim();
            var total = order.Details.Sum(d => d.Price * d.ProductAmount);

            sb.AppendLine($"{order.Id},{order.UserId},\"{userName}\",{order.OperationTime},{order.OrderStateId},{total:F2}");
        }

        File.WriteAllText(filePath, sb.ToString());
        return filePath;
    }

    /// <summary>
    /// Generates sales report for specified date range.
    /// </summary>
    public SalesReport GenerateSalesReport(DateTime startDate, DateTime endDate)
    {
        var orders = this.context.CustomerOrders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .ThenInclude(p => p!.Title)
            .AsNoTracking()
            .ToList()
            .Where(o => DateTime.TryParse(o.OperationTime, out var date) && date >= startDate && date <= endDate)
            .ToList();

        var totalOrders = orders.Count;
        var totalRevenue = orders.Sum(o => o.Details.Sum(d => d.Price * d.ProductAmount));
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

        var topProducts = orders
            .SelectMany(o => o.Details)
            .GroupBy(d => new { d.ProductId, ProductName = d.Product?.Title?.Title ?? "Unknown" })
            .Select(g => new ProductSalesData
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(d => d.ProductAmount),
                Revenue = g.Sum(d => d.Price * d.ProductAmount),
            })
            .OrderByDescending(p => p.Revenue)
            .Take(10)
            .ToList();

        return new SalesReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue,
            TopProducts = topProducts,
        };
    }

    /// <summary>
    /// Exports sales report to text file.
    /// </summary>
    public string ExportSalesReportToText(SalesReport report, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════");
        sb.AppendLine("              SALES REPORT");
        sb.AppendLine("═══════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine($"Period: {report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("SUMMARY");
        sb.AppendLine("───────────────────────────────────────────────────────────");
        sb.AppendLine($"Total Orders:         {report.TotalOrders}");
        sb.AppendLine($"Total Revenue:        ${report.TotalRevenue:F2}");
        sb.AppendLine($"Average Order Value:  ${report.AverageOrderValue:F2}");
        sb.AppendLine();
        sb.AppendLine("TOP 10 PRODUCTS");
        sb.AppendLine("───────────────────────────────────────────────────────────");
        sb.AppendLine($"{"Rank",-6}{"Product",-30}{"Qty",-8}{"Revenue",-12}");
        sb.AppendLine(new string('─', 60));

        int rank = 1;
        foreach (var product in report.TopProducts)
        {
            var name = product.ProductName.Length > 28 ? product.ProductName[..28] : product.ProductName;
            sb.AppendLine($"{rank,-6}{name,-30}{product.QuantitySold,-8}${product.Revenue,-11:F2}");
            rank++;
        }

        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════════");
        sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        File.WriteAllText(filePath, sb.ToString());
        return filePath;
    }

    /// <summary>
    /// Generates inventory status report.
    /// </summary>
    public InventoryReport GenerateInventoryReport()
    {
        var products = this.context.Products
            .Include(p => p.Title)
            .ThenInclude(t => t!.Category)
            .AsNoTracking()
            .ToList();

        var totalProducts = products.Count;
        var totalStock = products.Sum(p => p.StockQuantity);
        var totalReserved = products.Sum(p => p.ReservedQuantity);
        var totalAvailable = totalStock - totalReserved;

        var lowStock = products
            .Where(p => (p.StockQuantity - p.ReservedQuantity) < 10)
            .Select(p => new ProductStockData
            {
                ProductId = p.Id,
                ProductName = p.Title?.Title ?? "Unknown",
                Stock = p.StockQuantity,
                Reserved = p.ReservedQuantity,
                Available = p.StockQuantity - p.ReservedQuantity,
            })
            .OrderBy(p => p.Available)
            .ToList();

        var byCategory = products
            .GroupBy(p => p.Title?.Category?.Name ?? "Uncategorized")
            .Select(g => new CategoryStockData
            {
                CategoryName = g.Key,
                ProductCount = g.Count(),
                TotalStock = g.Sum(p => p.StockQuantity),
                TotalReserved = g.Sum(p => p.ReservedQuantity),
                TotalAvailable = g.Sum(p => p.StockQuantity - p.ReservedQuantity),
            })
            .OrderByDescending(c => c.TotalStock)
            .ToList();

        return new InventoryReport
        {
            TotalProducts = totalProducts,
            TotalStock = totalStock,
            TotalReserved = totalReserved,
            TotalAvailable = totalAvailable,
            LowStockProducts = lowStock,
            StockByCategory = byCategory,
        };
    }
}

// Report data models
public class SalesReport
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int TotalOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal AverageOrderValue { get; set; }

    public List<ProductSalesData> TopProducts { get; set; } = new();
}

public class ProductSalesData
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int QuantitySold { get; set; }

    public decimal Revenue { get; set; }
}

public class InventoryReport
{
    public int TotalProducts { get; set; }

    public int TotalStock { get; set; }

    public int TotalReserved { get; set; }

    public int TotalAvailable { get; set; }

    public List<ProductStockData> LowStockProducts { get; set; } = new();

    public List<CategoryStockData> StockByCategory { get; set; } = new();
}

public class ProductStockData
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Stock { get; set; }

    public int Reserved { get; set; }

    public int Available { get; set; }
}

public class CategoryStockData
{
    public string CategoryName { get; set; } = string.Empty;

    public int ProductCount { get; set; }

    public int TotalStock { get; set; }

    public int TotalReserved { get; set; }

    public int TotalAvailable { get; set; }
}
