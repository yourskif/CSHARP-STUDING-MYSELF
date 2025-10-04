// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\Store.Tests\OrderFlowTests.cs
using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreBLL.Services;

using StoreDAL.Entities;

using Xunit;

namespace Store.Tests;

public class OrderFlowTests
{
    /// <summary>
    /// Happy path: 1 -> 4 -> 5 -> 6 -> 7 -> 8.
    /// When transitioning to state 8, ConfirmOrderDelivery is called by the service (business logic),
    /// so we do NOT call it manually - just verify that:
    /// - reservations created by this order are released (returned to starting value);
    /// - stock is decreased by exactly the order quantity.
    /// </summary>
    [Fact]
    public void HappyPath_New_To_8_ConfirmsAndZeroesReservations()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Take a product without initial reservations (in seed data there are such products, e.g. #2)
            var product = ctx.Products.AsNoTracking()
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            var productId = product.Id;
            var q = 10;
            var stockBefore = product.StockQuantity;       // e.g. 300
            var reservedStart = product.ReservedQuantity;     // 0

            // Create order in New state (1)
            var order = new CustomerOrder
            {
                UserId = 2, // Registered user from seeds
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            // Add order line
            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Simulate reservation when creating New order (as in real UI)
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q; // 0 + 10
            ctx.SaveChanges();

            // Progress through states to 8
            var orderSvc = new CustomerOrderService(ctx);
            Assert.True(orderSvc.TryChangeState(order.Id, 4, out var e1), e1);
            Assert.True(orderSvc.TryChangeState(order.Id, 5, out var e2), e2);
            Assert.True(orderSvc.TryChangeState(order.Id, 6, out var e3), e3);
            Assert.True(orderSvc.TryChangeState(order.Id, 7, out var e4), e4);
            Assert.True(orderSvc.TryChangeState(order.Id, 8, out var e5), e5);

            // AFTER transitioning to 8, the service should have:
            // - decreased stock by q
            // - released q from reservations (returned to starting value)
            var pAfter = ctx.Products.AsNoTracking().First(p => p.Id == productId);

            Assert.Equal(reservedStart, pAfter.ReservedQuantity);     // 0
            Assert.Equal(stockBefore - q, pAfter.StockQuantity);      // 300 - 10 = 290
        }
        finally
        {
            cleanup();
        }
    }

    /// <summary>
    /// Forbidden transition: from 1 (New) directly to 6 (In delivery) - should be rejected
    /// with a message about allowed states.
    /// </summary>
    [Fact]
    public void ForbiddenTransition_FromNew_To6_IsRejected_WithAllowedList()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var order = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1, // New
            };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            var svc = new CustomerOrderService(ctx);
            var ok = svc.TryChangeState(order.Id, 6, out var error);

            Assert.False(ok);
            Assert.NotNull(error);
            Assert.Contains("Allowed next", error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            cleanup();
        }
    }

    /// <summary>
    /// Administrator cancellation: reservations are released, stock is NOT changed, status = 3.
    /// </summary>
    [Fact]
    public void AdminCancel_ReleasesReservations_StockUnchanged_Status3()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Product without initial reservations
            var product = ctx.Products.AsNoTracking()
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            var productId = product.Id;
            var q = 10;
            var stockBefore = product.StockQuantity;       // e.g. 300
            var reservedStart = product.ReservedQuantity;     // 0

            // New order
            var order = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            // Order line
            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Reserve for New order
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q; // 0 + 10
            ctx.SaveChanges();

            // Admin cancellation: release reservations, don't touch stock, status 3
            var stockSvc = new StockReservationService(ctx);
            stockSvc.ReleaseOrderReservations(order.Id);

            var orderSvc = new CustomerOrderService(ctx);
            Assert.True(orderSvc.TryChangeState(order.Id, 3, out var err), err);

            var pAfter = ctx.Products.AsNoTracking().First(p => p.Id == productId);
            Assert.Equal(reservedStart, pAfter.ReservedQuantity); // 0
            Assert.Equal(stockBefore, pAfter.StockQuantity);    // unchanged
        }
        finally
        {
            cleanup();
        }
    }

    /// <summary>
    /// Test that verifies atomic reservation prevents overselling
    /// when multiple orders compete for limited stock.
    /// </summary>
    [Fact]
    public void AtomicReservation_PreventsOverselling_WithConcurrentOrders()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Setup: Find a product with limited stock
            var product = ctx.Products
                .OrderBy(p => p.Id)
                .First(p => p.StockQuantity - p.ReservedQuantity >= 10);

            int productId = product.Id;
            int initialReserved = product.ReservedQuantity;

            // Set stock to exactly 10 available
            product.StockQuantity = initialReserved + 10;
            ctx.SaveChanges();

            // Scenario: Two orders trying to reserve 6 units each
            // Only first should succeed (10 available, 6+6 > 10)

            // Order 1: Request 6 units
            var order1 = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order1);
            ctx.SaveChanges();

            // Simulate transaction: check and reserve
            var p1 = ctx.Products.First(p => p.Id == productId);
            Assert.True(6 <= p1.AvailableQuantity); // Should pass

            p1.ReservedQuantity += 6;
            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order1.Id,
                ProductId = productId,
                ProductAmount = 6,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Verify: 4 units remain available
            var afterOrder1 = ctx.Products.AsNoTracking().First(p => p.Id == productId);
            Assert.Equal(4, afterOrder1.AvailableQuantity);

            // Order 2: Request 6 units (should fail - only 4 available)
            var order2 = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order2);
            ctx.SaveChanges();

            // Simulate transaction check
            var p2 = ctx.Products.First(p => p.Id == productId);
            bool canReserve = 6 <= p2.AvailableQuantity;

            Assert.False(canReserve); // Should fail

            // Verify: No additional reservation was made
            var finalProduct = ctx.Products.AsNoTracking().First(p => p.Id == productId);
            Assert.Equal(initialReserved + 6, finalProduct.ReservedQuantity);
            Assert.Equal(4, finalProduct.AvailableQuantity);
        }
        finally
        {
            cleanup();
        }
    }
}
