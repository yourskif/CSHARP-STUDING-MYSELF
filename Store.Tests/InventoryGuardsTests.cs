// Path: Store.Tests/InventoryGuardsTests.cs
using System;
using System.Linq;

using StoreBLL.Services;

using StoreDAL.Entities;

using Xunit;

namespace Store.Tests;

public class InventoryGuardsTests
{
    [Fact]
    public void DoubleMoveTo8_IsRejected_AndCountersUnchanged()
    {
        var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
        try
        {
            var ctx = unitOfWork.Context;

            // Take a product without initial reservations
            var product = ctx.Products
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            int productId = product.Id;
            int q = 10;
            int stockBefore = product.StockQuantity;
            int reservedStart = product.ReservedQuantity;

            // Create New(1) order + detail
            var order = new CustomerOrder { UserId = 2, OperationTime = DateTime.UtcNow.ToString("u"), OrderStateId = 1 };
            ctx.CustomerOrders.Add(order);
            unitOfWork.SaveChanges();

            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            unitOfWork.SaveChanges();

            // Reserve for New order, as in UI
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q;
            unitOfWork.SaveChanges();

            var svc = new CustomerOrderService(unitOfWork);
            Assert.True(svc.TryChangeState(order.Id, 4, out var e1), e1);
            Assert.True(svc.TryChangeState(order.Id, 5, out var e2), e2);
            Assert.True(svc.TryChangeState(order.Id, 6, out var e3), e3);
            Assert.True(svc.TryChangeState(order.Id, 7, out var e4), e4);

            // First time to 8: ConfirmOrderDelivery inside service
            Assert.True(svc.TryChangeState(order.Id, 8, out var e5), e5);

            var after1 = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(reservedStart, after1.ReservedQuantity);        // 0
            Assert.Equal(stockBefore - q, after1.StockQuantity);         // -10

            // Second time to 8 should be rejected and nothing should change
            var ok2 = svc.TryChangeState(order.Id, 8, out var e6);
            Assert.False(ok2);

            var after2 = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(after1.ReservedQuantity, after2.ReservedQuantity);
            Assert.Equal(after1.StockQuantity, after2.StockQuantity);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void ReleaseTwice_NeverGoesNegative_AndKeepsStock()
    {
        var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
        try
        {
            var ctx = unitOfWork.Context;

            var product = ctx.Products
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 5);

            int productId = product.Id;
            int q = 3;
            int stockBefore = product.StockQuantity;
            int reservedStart = product.ReservedQuantity;

            var order = new CustomerOrder { UserId = 2, OperationTime = DateTime.UtcNow.ToString("u"), OrderStateId = 1 };
            ctx.CustomerOrders.Add(order);
            unitOfWork.SaveChanges();

            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            unitOfWork.SaveChanges();

            // Reserve for New
            var p = ctx.Products.First(p => p.Id == productId);
            p.ReservedQuantity += q; // 0 -> 3
            unitOfWork.SaveChanges();

            var stockSvc = new StockReservationService(unitOfWork);

            // Release twice
            stockSvc.ReleaseOrderReservations(order.Id);
            stockSvc.ReleaseOrderReservations(order.Id);

            var after = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(reservedStart, after.ReservedQuantity); // 0, not < 0
            Assert.Equal(stockBefore, after.StockQuantity);    // stock unchanged
        }
        finally { cleanup(); }
    }

    [Fact]
    public void ConfirmDeliveryTwice_DoesNotDecrementStockTwice()
    {
        var (unitOfWork, cleanup) = TestDbHelper.CreateUnitOfWork();
        try
        {
            var ctx = unitOfWork.Context;

            var product = ctx.Products.First(p => p.StockQuantity >= 20);
            int productId = product.Id;
            int q = 10;
            int stockBefore = product.StockQuantity;

            // Create order in "Delivered to client" state (7)
            var order = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 7, // Delivered
            };
            ctx.CustomerOrders.Add(order);
            unitOfWork.SaveChanges();

            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            unitOfWork.SaveChanges();

            var stockSvc = new StockReservationService(unitOfWork);

            // First confirmation (state is 7, should process)
            stockSvc.ConfirmOrderDelivery(order.Id);

            var after1 = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(stockBefore - q, after1.StockQuantity);

            // Change order state to 8 (confirmed)
            var orderToUpdate = ctx.CustomerOrders.First(o => o.Id == order.Id);
            orderToUpdate.OrderStateId = 8;
            unitOfWork.SaveChanges();

            // Second confirmation attempt (state is now 8, should skip)
            stockSvc.ConfirmOrderDelivery(order.Id);

            // Verify stock was NOT decremented again
            var after2 = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(after1.StockQuantity, after2.StockQuantity);
        }
        finally
        {
            cleanup();
        }
    }
}
