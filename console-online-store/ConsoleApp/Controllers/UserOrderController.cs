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
    /// User's order management: create order, list own orders, cancel own order.
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
        /// Show menu for order management.
        /// </summary>
        public static void ShowOrderMenu(StoreDbContext context)
        {
            var controller = new UserOrderController(context);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MY ORDERS ===");
                Console.WriteLine("1. Create New Order");
                Console.WriteLine("2. View My Orders");
                Console.WriteLine("3. Cancel My Order");
                Console.WriteLine("4. Mark Order as Received");
                Console.WriteLine("----------------------");
                Console.WriteLine("Esc: Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        controller.CreateOrder();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        controller.ShowMyOrders();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        controller.CancelMyOrder();
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        controller.MarkOrderAsReceived();
                        break;
                    case ConsoleKey.Escape:
                        return;
                }
            }
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
            Console.WriteLine("=== Available Products ===");

            var products = this.productService
                .GetAll()
                .OfType<ProductModel>()
                .Where(p => p.Stock > 0)
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
                Console.WriteLine($"{p.Id,3}: {p.Title,-25} | Price: {p.Price,8:C} | Stock: {p.Stock,4}");
            }

            Console.Write("Enter product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID.");
                Pause();
                return;
            }

            var product = this.productService.GetById(productId) as ProductModel;
            if (product is null)
            {
                Console.WriteLine("Product not found.");
                Pause();
                return;
            }

            if (product.Stock <= 0)
            {
                Console.WriteLine("Product is out of stock.");
                Pause();
                return;
            }

            Console.Write($"Enter quantity (max {product.Stock}): ");
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

            var order = new CustomerOrderModel
            {
                UserId = UserMenuController.CurrentUser.Id,
                OrderStateId = 1, // New Order
                OperationTime = DateTime.UtcNow.ToString("u"),
            };

            this.orderService.Add(order);

            var detail = new OrderDetailModel
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.Price,
            };

            this.detailService.Add(detail);

            decimal totalAmount = product.Price * quantity;
            Console.WriteLine();
            Console.WriteLine($"✅ Order #{order.Id} created successfully!");
            Console.WriteLine($"Product: {product.Title}");
            Console.WriteLine($"Quantity: {quantity}");
            Console.WriteLine($"Unit Price: {product.Price:C}");
            Console.WriteLine($"Total Amount: {totalAmount:C}");
            Console.WriteLine($"Status: {OrderStateHelper.GetStateName(1)}");

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
                .OrderByDescending(o => o.Id)
                .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                Pause();
                return;
            }

            Console.WriteLine();
            foreach (var order in orders)
            {
                Console.WriteLine($"Order #{order.Id}");
                Console.WriteLine($"  Status: {OrderStateHelper.GetStateName(order.OrderStateId)}");
                Console.WriteLine($"  Date: {order.OperationTime}");

                var details = this.detailService
                    .GetAll()
                    .OfType<OrderDetailModel>()
                    .Where(d => d.OrderId == order.Id)
                    .ToList();

                if (details.Any())
                {
                    Console.WriteLine("  Items:");
                    decimal orderTotal = 0;
                    foreach (var detail in details)
                    {
                        var product = this.productService.GetById(detail.ProductId);
                        string productName = product?.Title ?? $"Product #{detail.ProductId}";
                        decimal lineTotal = detail.UnitPrice * detail.Quantity;
                        orderTotal += lineTotal;

                        Console.WriteLine($"    - {productName} x{detail.Quantity} @ {detail.UnitPrice:C} = {lineTotal:C}");
                    }
                    Console.WriteLine($"  Total: {orderTotal:C}");
                }
                Console.WriteLine();
            }

            Pause();
        }

        /// <summary>
        /// Cancel user's own order if allowed.
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
            Console.Write("Enter Order ID to cancel: ");

            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                Pause();
                return;
            }

            int userId = UserMenuController.CurrentUser.Id;

            var success = this.orderService.CancelOwnOrder(orderId, userId, out string error);
            if (success)
            {
                Console.WriteLine($"✅ Order #{orderId} has been cancelled.");
            }
            else
            {
                Console.WriteLine($"❌ Cannot cancel order: {error}");
            }

            Pause();
        }

        /// <summary>
        /// Mark delivered order as received by user.
        /// </summary>
        public void MarkOrderAsReceived()
        {
            if (UserMenuController.CurrentUser == null)
            {
                Console.WriteLine("No user is logged in.");
                Pause();
                return;
            }

            Console.WriteLine("=== Mark Order as Received ===");
            Console.Write("Enter Order ID to mark as received: ");

            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                Pause();
                return;
            }

            int userId = UserMenuController.CurrentUser.Id;

            var success = this.orderService.MarkAsReceived(orderId, userId, out string error);
            if (success)
            {
                Console.WriteLine($"✅ Order #{orderId} marked as received.");
            }
            else
            {
                Console.WriteLine($"❌ Cannot mark as received: {error}");
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
