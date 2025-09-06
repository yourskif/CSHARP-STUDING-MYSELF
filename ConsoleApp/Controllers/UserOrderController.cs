using System;
using System.Linq;

using ConsoleApp.Helpers;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.Data;
using StoreDAL.Repository;

namespace ConsoleApp.Controllers
{
    /// <summary>
    /// User's order flow: create order, list own orders, cancel own order.
    /// </summary>
    public class UserOrderController
    {
        private readonly CustomerOrderService orderService;
        private readonly OrderDetailService detailService;
        private readonly ProductService productService;

        public UserOrderController(StoreDbContext context)
        {
            this.orderService = new CustomerOrderService(context);
            this.detailService = new OrderDetailService(context);
            this.productService = new ProductService(new ProductRepository(context));
        }

        /// <summary>
        /// Create a new order for the currently logged-in user.
        /// </summary>
        public void CreateOrder()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                Pause();
                return;
            }

            Console.WriteLine("=== Create New Order ===");
            Console.WriteLine("=== Products ===");

            // Show a compact product list (Id / Title / Price / Stock).
            var products = this.productService
                .GetAll()
                .OfType<ProductModel>()
                .OrderBy(p => p.Id)
                .ToList();

            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                Pause();
                return;
            }

            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id,3}: {p.Title,-20} | SKU: {p.Sku,-10} | Price: {p.Price,8} | Stock: {p.Stock,4}");
            }

            Console.Write("Enter product Id: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product Id.");
                Pause();
                return;
            }

            // Load product again to make sure we have the latest price/stock.
            var product = this.productService.GetById(productId) as ProductModel;
            if (product is null)
            {
                Console.WriteLine("Product not found.");
                Pause();
                return;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                Pause();
                return;
            }

            if (product.Stock < quantity)
            {
                Console.WriteLine($"Not enough stock. Available: {product.Stock}");
                Pause();
                return;
            }

            // Create order in state "New" (OrderStateId = 1).
            var order = new CustomerOrderModel
            {
                UserId = UserMenuController.CurrentUser.Id,
                OrderStateId = 1,
                OperationTime = DateTime.UtcNow.ToString("u"),
            };
            this.orderService.Add(order); // service sets generated Id back to model

            // Create order line with actual unit price.
            var detail = new OrderDetailModel
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price,
            };
            this.detailService.Add(detail);

            Console.WriteLine(
                $"✅ Order #{order.Id} created by {UserMenuController.CurrentUser.Login} | " +
                $"Product: {product.Title} (#{product.Id}), Qty: {quantity}, Unit price: {product.Price}");

            // NOTE: Stock decrement can be added later if needed.
            Pause();
        }

        /// <summary>
        /// Show all orders for the currently logged-in user.
        /// </summary>
        public void ShowMyOrders()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                Pause();
                return;
            }

            Console.WriteLine("=== My Orders ===");

            int userId = UserMenuController.CurrentUser.Id;

            var orders = this.orderService
                .GetAll()
                .OfType<CustomerOrderModel>()
                .Where(o => o.UserId == userId)
                .OrderBy(o => o.Id)
                .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                Pause();
                return;
            }

            foreach (var o in orders)
            {
                Console.WriteLine($"Order #{o.Id} | State: {OrderStateHelper.GetStateName(o.OrderStateId)} | Time: {o.OperationTime}");
            }

            Pause();
        }

        /// <summary>
        /// Cancel user's own order if allowed by business rules.
        /// </summary>
        public void CancelMyOrder()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                Pause();
                return;
            }

            Console.WriteLine("=== Cancel My Order ===");
            Console.Write("Enter Order Id to cancel: ");

            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order Id.");
                Pause();
                return;
            }

            int userId = UserMenuController.CurrentUser.Id;

            // FIX: pass the required out parameter
            var ok = this.orderService.CancelOwnOrder(orderId, userId, out string? error);
            if (ok)
            {
                Console.WriteLine($"✅ Order #{orderId} canceled (state: {OrderStateHelper.GetStateName(2)}).");
            }
            else
            {
                Console.WriteLine($"❌ Cancel failed. {error ?? "Only your own 'New' orders can be canceled."}");
            }

            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
