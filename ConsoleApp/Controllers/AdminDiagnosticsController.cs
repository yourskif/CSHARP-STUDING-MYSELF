using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

// Keep alias directives sorted to satisfy SA1211.
using DalCustomerOrder = StoreDAL.Entities.CustomerOrder;
using DalOrderDetail = StoreDAL.Entities.OrderDetail;
using DalProduct = StoreDAL.Entities.Product;
using DalUser = StoreDAL.Entities.User;
using StoreDbContext = StoreDAL.Data.StoreDbContext;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// Admin diagnostics screen.
    /// Reflection-friendly and defensive against missing properties.
    /// </summary>
    public sealed class AdminDiagnosticsController
    {
        // Diagnostics parameters (can be adjusted while running).
        private readonly StoreDbContext _db;
        private int _topDays = 30;            // Default period for "Top sellers".
        private int _lowStockThreshold = 2;   // Threshold for "Low stock" alert.

        public AdminDiagnosticsController(StoreDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// One-shot diagnostics (non-interactive).
        /// </summary>
        public void RunOnce()
        {
            Console.Clear();
            PrintHeader();
            PrintCounts();
            PrintOrdersByState();
            PrintTopSellers(_topDays);
            PrintLowStock(_lowStockThreshold);
            PrintDataIssues();
        }

        /// <summary>
        /// Interactive diagnostics loop with [R]efresh and [Q]/Esc back.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Clear();
                PrintHeader();

                try
                {
                    PrintCounts();
                    PrintOrdersByState();
                    PrintTopSellers(_topDays);
                    PrintLowStock(_lowStockThreshold);
                    PrintDataIssues();
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("An error occurred while computing diagnostics:");
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine();
                Console.WriteLine("[R] Refresh    [P] Period for Top sellers (7/30/60)    [Q]/Esc Back");

                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                {
                    return;
                }

                if (key == ConsoleKey.R)
                {
                    continue;
                }

                if (key == ConsoleKey.P)
                {
                    Console.WriteLine();
                    Console.Write("Enter days (e.g., 7 / 30 / 60): ");
                    if (int.TryParse(Console.ReadLine(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var days) && days > 0)
                    {
                        _topDays = days;
                    }
                }
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine("=== DIAGNOSTICS ===");
            Console.WriteLine($"UTC Now: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}  |  Local: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine(new string('-', 62));
        }

        private void PrintCounts()
        {
            var userCount = SafeCount<DalUser>();
            var productCount = SafeCount<DalProduct>();
            var orderCount = SafeCount<DalCustomerOrder>();

            Console.WriteLine("Overview");
            Console.WriteLine(Row("Users total", userCount.ToString(CultureInfo.InvariantCulture)));
            Console.WriteLine(Row("Products total", productCount.ToString(CultureInfo.InvariantCulture)));
            Console.WriteLine(Row("Orders total", orderCount.ToString(CultureInfo.InvariantCulture)));
            Console.WriteLine();
        }

        private void PrintOrdersByState()
        {
            Console.WriteLine("Orders by state");

            var orders = _db.Set<DalCustomerOrder>().ToList();
            var stateProp = FindProp(typeof(DalCustomerOrder), "State", "OrderState", "Status");

            if (stateProp == null)
            {
                Console.WriteLine("  (State property not found on CustomerOrder)");
                Console.WriteLine();
                return;
            }

            var grouped = orders
                .Select(o => stateProp.GetValue(o))
                .GroupBy(state => state?.ToString() ?? "(null)")
                .Select(g => new { State = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (grouped.Count == 0)
            {
                Console.WriteLine("  (No orders)");
            }
            else
            {
                foreach (var row in grouped)
                {
                    Console.WriteLine(Row(row.State, row.Count.ToString(CultureInfo.InvariantCulture)));
                }
            }

            Console.WriteLine();
        }

        private void PrintTopSellers(int days)
        {
            Console.WriteLine($"Top sellers (last {days} days)");

            var orders = _db.Set<DalCustomerOrder>().ToList();
            var orderDetails = _db.Set<DalOrderDetail>().ToList();
            var products = _db.Set<DalProduct>().ToList();

            var orderIdProp = FindProp(typeof(DalCustomerOrder), "Id", "CustomerOrderId", "OrderId");
            var orderDateProp = FindProp(typeof(DalCustomerOrder), "OperationTimeUtc", "CreatedAtUtc", "OrderDate", "CreatedAt", "CreatedOn", "Date");
            var detailOrderIdProp = FindProp(typeof(DalOrderDetail), "OrderId", "CustomerOrderId");
            var detailProductIdProp = FindProp(typeof(DalOrderDetail), "ProductId", "ItemId");
            var detailQtyProp = FindProp(typeof(DalOrderDetail), "Quantity", "Qty", "Count", "Amount");
            var productIdProp = FindProp(typeof(DalProduct), "Id", "ProductId");
            var productNameProp = FindProp(typeof(DalProduct), "Name", "Title", "ProductName");

            var nowUtc = DateTime.UtcNow;
            var fromUtc = nowUtc.AddDays(-days);

            if (orderIdProp == null || detailOrderIdProp == null || detailProductIdProp == null || detailQtyProp == null)
            {
                Console.WriteLine("  (Required properties not found: CustomerOrder.Id / OrderDetail.OrderId / OrderDetail.ProductId / OrderDetail.Quantity)");
                Console.WriteLine();
                return;
            }

            var allowedOrderIds = new HashSet<object>();
            foreach (var o in orders)
            {
                if (orderDateProp == null)
                {
                    var oidAny = orderIdProp.GetValue(o);
                    if (oidAny != null)
                    {
                        allowedOrderIds.Add(oidAny);
                    }
                }
                else
                {
                    var raw = orderDateProp.GetValue(o);
                    DateTime? dt = raw as DateTime?;
                    if (raw is DateTime d)
                    {
                        dt = d;
                    }

                    var oid = orderIdProp.GetValue(o);
                    if (dt.HasValue && dt.Value.ToUniversalTime() >= fromUtc && dt.Value.ToUniversalTime() <= nowUtc && oid != null)
                    {
                        allowedOrderIds.Add(oid);
                    }
                }
            }

            var productIdToName = new Dictionary<object, string>();
            foreach (var p in products)
            {
                var id = productIdProp?.GetValue(p);
                if (id == null)
                {
                    continue;
                }

                var nameRaw = productNameProp?.GetValue(p)?.ToString();
                var fallback = Convert.ToString(id, CultureInfo.InvariantCulture) ?? "(null)";
                var finalName = !string.IsNullOrWhiteSpace(nameRaw) ? nameRaw! : fallback;

                productIdToName[id] = finalName;
            }

            var totals = new Dictionary<object, double>();
            foreach (var d in orderDetails)
            {
                var oid = detailOrderIdProp.GetValue(d);
                if (oid == null || !allowedOrderIds.Contains(oid))
                {
                    continue;
                }

                var pid = detailProductIdProp.GetValue(d);
                if (pid == null)
                {
                    continue;
                }

                var q = ConvertToDouble(detailQtyProp.GetValue(d));
                if (q <= 0)
                {
                    continue;
                }

                if (!totals.TryGetValue(pid, out var cur))
                {
                    cur = 0;
                }

                totals[pid] = cur + q;
            }

            var top = totals
                .OrderByDescending(kv => kv.Value)
                .Take(10)
                .Select((kv, i) =>
                {
                    string productName;
                    var hasName = productIdToName.TryGetValue(kv.Key, out var nm);
                    if (!hasName || string.IsNullOrWhiteSpace(nm))
                    {
                        productName = kv.Key?.ToString() ?? "(null)";
                    }
                    else
                    {
                        productName = nm;
                    }

                    return new
                    {
                        Rank = i + 1,
                        Product = productName,
                        Quantity = kv.Value,
                    };
                })
                .ToList();

            if (top.Count == 0)
            {
                Console.WriteLine("  (No data)");
            }
            else
            {
                foreach (var row in top)
                {
                    Console.WriteLine(Row($"{row.Rank}. {row.Product}", row.Quantity.ToString(CultureInfo.CurrentCulture)));
                }
            }

            Console.WriteLine();
        }

        private void PrintLowStock(int threshold)
        {
            Console.WriteLine($"Stock alerts (<= {threshold})");

            var products = _db.Set<DalProduct>().ToList();
            var idProp = FindProp(typeof(DalProduct), "Id", "ProductId");
            var nameProp = FindProp(typeof(DalProduct), "Name", "Title", "ProductName");
            var stockProp = FindProp(typeof(DalProduct), "Stock", "QuantityInStock", "QtyInStock", "Count", "Available");

            if (idProp == null || stockProp == null)
            {
                Console.WriteLine("  (Required properties not found: Product.Id/ProductId and Product.Stock/QuantityInStock/...)");
                Console.WriteLine();
                return;
            }

            var lows = new List<(string Name, double Stock)>();
            foreach (var p in products)
            {
                var stock = ConvertToDouble(stockProp.GetValue(p));
                if (stock <= threshold)
                {
                    var name = nameProp?.GetValue(p)?.ToString();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = Convert.ToString(idProp.GetValue(p), CultureInfo.InvariantCulture) ?? "(null)";
                    }

                    lows.Add((name!, stock));
                }
            }

            if (lows.Count == 0)
            {
                Console.WriteLine("  (No low stock)");
            }
            else
            {
                foreach (var row in lows.OrderBy(x => x.Stock).ThenBy(x => x.Name))
                {
                    Console.WriteLine(Row(row.Name, row.Stock.ToString(CultureInfo.CurrentCulture)));
                }
            }

            Console.WriteLine();
        }

        private void PrintDataIssues()
        {
            Console.WriteLine("Data issues");

            var orders = _db.Set<DalCustomerOrder>().ToList();
            var orderDetails = _db.Set<DalOrderDetail>().ToList();
            var products = _db.Set<DalProduct>().ToList();

            var orderIdProp = FindProp(typeof(DalCustomerOrder), "Id", "CustomerOrderId", "OrderId");
            var detailOrderIdProp = FindProp(typeof(DalOrderDetail), "OrderId", "CustomerOrderId");
            var detailProductIdProp = FindProp(typeof(DalOrderDetail), "ProductId", "ItemId");
            var productIdProp = FindProp(typeof(DalProduct), "Id", "ProductId");

            if (detailOrderIdProp == null || detailProductIdProp == null)
            {
                Console.WriteLine("  (OrderDetail.OrderId/ProductId not found)");
                Console.WriteLine();
                return;
            }

            var orderIds = new HashSet<object>(
                orders.Select(o => orderIdProp?.GetValue(o)).OfType<object>());

            var productIds = new HashSet<object>(
                products.Select(p => productIdProp?.GetValue(p)).OfType<object>());

            var missingOrder = 0;
            var missingProduct = 0;

            foreach (var d in orderDetails)
            {
                var oid = detailOrderIdProp.GetValue(d);
                if (oid == null || !orderIds.Contains(oid))
                {
                    missingOrder++;
                }

                var pid = detailProductIdProp.GetValue(d);
                if (pid == null || !productIds.Contains(pid))
                {
                    missingProduct++;
                }
            }

            if (missingOrder == 0 && missingProduct == 0)
            {
                Console.WriteLine("  No issues detected.");
            }
            else
            {
                if (missingOrder > 0)
                {
                    Console.WriteLine(Row("OrderDetails referencing missing CustomerOrder", missingOrder.ToString(CultureInfo.InvariantCulture)));
                }

                if (missingProduct > 0)
                {
                    Console.WriteLine(Row("OrderDetails referencing missing Product", missingProduct.ToString(CultureInfo.InvariantCulture)));
                }
            }

            Console.WriteLine();
        }

        // ---------- Small helpers ----------

        private int SafeCount<TEntity>() where TEntity : class
        {
            return _db.Set<TEntity>().Count();
        }

        private static string Row(string left, string right, int width = 50)
        {
            left ??= string.Empty;
            right ??= string.Empty;

            if (left.Length >= width)
            {
                return left + " " + right;
            }

            return left + new string('.', Math.Max(1, width - left.Length)) + " " + right;
        }

        private static PropertyInfo? FindProp(Type type, params string[] candidates)
        {
            foreach (var name in candidates)
            {
                var p = type.GetProperty(
                    name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (p != null)
                {
                    return p;
                }
            }

            return null;
        }

        private static double ConvertToDouble(object? value)
        {
            if (value == null)
            {
                return 0;
            }

            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }
    }
}
