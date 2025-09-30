// Path: console-online-store/Store.Tests/ReportServiceTests.cs
using System;
using System.IO;
using System.Linq;

using StoreBLL.Services;

using Xunit;

namespace Store.Tests;

public class ReportServiceTests
{
    [Fact]
    public void ExportProductsToCsv_CreatesValidFile()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ReportService(ctx);
            var testFile = Path.GetTempFileName();

            // Act
            var result = service.ExportProductsToCsv(testFile);

            // Assert
            Assert.True(File.Exists(result));
            var content = File.ReadAllText(result);
            Assert.Contains("ID,Title,Category", content);
            Assert.Contains(",", content);

            // Cleanup
            File.Delete(testFile);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void ExportOrdersToCsv_CreatesValidFile()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ReportService(ctx);
            var testFile = Path.GetTempFileName();

            // Act
            var result = service.ExportOrdersToCsv(testFile);

            // Assert
            Assert.True(File.Exists(result));
            var content = File.ReadAllText(result);
            Assert.Contains("OrderID,UserID", content);

            // Cleanup
            File.Delete(testFile);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void GenerateSalesReport_ReturnsValidData()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ReportService(ctx);
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var report = service.GenerateSalesReport(startDate, endDate);

            // Assert
            Assert.NotNull(report);
            Assert.True(report.TotalOrders >= 0);
            Assert.True(report.TotalRevenue >= 0);
            Assert.NotNull(report.TopProducts);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void GenerateInventoryReport_ReturnsValidData()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ReportService(ctx);

            // Act
            var report = service.GenerateInventoryReport();

            // Assert
            Assert.NotNull(report);
            Assert.True(report.TotalProducts > 0);
            Assert.True(report.TotalStock >= 0);
            Assert.NotNull(report.LowStockProducts);
            Assert.NotNull(report.StockByCategory);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void ExportSalesReportToText_CreatesReadableFile()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new ReportService(ctx);
            var report = service.GenerateSalesReport(DateTime.Now.AddDays(-30), DateTime.Now);
            var testFile = Path.GetTempFileName();

            // Act
            var result = service.ExportSalesReportToText(report, testFile);

            // Assert
            Assert.True(File.Exists(result));
            var content = File.ReadAllText(result);
            Assert.Contains("SALES REPORT", content);
            Assert.Contains("Total Orders", content);
            Assert.Contains("Total Revenue", content);

            // Cleanup
            File.Delete(testFile);
        }
        finally { cleanup(); }
    }
}
