// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Services\StockReservationService.csnamespace StoreBLL.Services
{
    using System;
    using System.Linq;
    using StoreDAL.Data;
    using StoreDAL.Entities;

    /// <summary>
    /// Service for managing product stock reservations.
    /// </summary>
    public class StockReservationService
    {
        private readonly StoreDbContext context;

        public StockReservationService(StoreDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Reserve stock when order is created.
        /// </summary>
        /// <returns></returns>
        public bool ReserveStock(int productId, int quantity)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return false;
            }

            // Check if we have enough available stock
            if (product.AvailableQuantity < quantity)
            {
                Console.WriteLine($"Not enough stock available. Requested: {quantity}, Available: {product.AvailableQuantity}");
                return false;
            }

            // Reserve the quantity
            product.ReservedQuantity += quantity;
            this.context.SaveChanges();

            Console.WriteLine($"Reserved {quantity} units of product {productId}. Available: {product.AvailableQuantity}");
            return true;
        }

        /// <summary>
        /// Release reservation when order is cancelled.
        /// </summary>
        /// <returns></returns>
        public bool ReleaseReservation(int productId, int quantity)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return false;
            }

            // Release the reservation
            product.ReservedQuantity = Math.Max(0, product.ReservedQuantity - quantity);
            this.context.SaveChanges();

            Console.WriteLine($"Released {quantity} units of product {productId}. Available: {product.AvailableQuantity}");
            return true;
        }

        /// <summary>
        /// Confirm reservation when order is delivered (actually decrease stock).
        /// </summary>
        /// <returns></returns>
        public bool ConfirmReservation(int productId, int quantity)
        {
            var product = this.context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return false;
            }

            // Decrease stock and reserved quantity
            product.StockQuantity -= quantity;
            product.ReservedQuantity = Math.Max(0, product.ReservedQuantity - quantity);
            this.context.SaveChanges();

            Console.WriteLine($"Confirmed sale of {quantity} units of product {productId}. Stock: {product.StockQuantity}, Reserved: {product.ReservedQuantity}");
            return true;
        }

        /// <summary>
        /// Reserve stock for entire order.
        /// </summary>
        /// <returns></returns>
        public bool ReserveOrderStock(int orderId)
        {
            var order = this.context.CustomerOrders
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            var orderDetails = this.context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToList();

            // Try to reserve all items
            foreach (var detail in orderDetails)
            {
                if (!this.ReserveStock(detail.ProductId, detail.ProductAmount))
                {
                    // Rollback if any item can't be reserved
                    Console.WriteLine($"Failed to reserve stock for order {orderId}");
                    return false;
                }
            }

            Console.WriteLine($"Successfully reserved stock for order {orderId}");
            return true;
        }

        /// <summary>
        /// Release all reservations for an order.
        /// </summary>
        /// <returns></returns>
        public bool ReleaseOrderReservations(int orderId)
        {
            var orderDetails = this.context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToList();

            foreach (var detail in orderDetails)
            {
                this.ReleaseReservation(detail.ProductId, detail.ProductAmount);
            }

            Console.WriteLine($"Released all reservations for order {orderId}");
            return true;
        }

        /// <summary>
        /// Confirm all reservations when order is delivered.
        /// </summary>
        /// <returns></returns>
        public bool ConfirmOrderDelivery(int orderId)
        {
            var order = this.context.CustomerOrders
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            var orderDetails = this.context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToList();

            foreach (var detail in orderDetails)
            {
                this.ConfirmReservation(detail.ProductId, detail.ProductAmount);
            }

            // Update order status to delivered
            order.OrderStateId = 8; // "Delivery confirmed by client"
            this.context.SaveChanges();

            Console.WriteLine($"Confirmed delivery for order {orderId}. Stock updated.");
            return true;
        }
    }
}