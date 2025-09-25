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
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Візьмемо товар без стартового резерву
            var product = ctx.Products
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            int productId = product.Id;
            int q = 10;
            int stockBefore = product.StockQuantity;
            int reservedStart = product.ReservedQuantity;

            // Створити New(1) замовлення + деталь
            var order = new CustomerOrder { UserId = 2, OperationTime = DateTime.UtcNow.ToString("u"), OrderStateId = 1 };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Резервуємо під New, як у UI
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q;
            ctx.SaveChanges();

            var svc = new CustomerOrderService(ctx);
            Assert.True(svc.TryChangeState(order.Id, 4, out var e1), e1);
            Assert.True(svc.TryChangeState(order.Id, 5, out var e2), e2);
            Assert.True(svc.TryChangeState(order.Id, 6, out var e3), e3);
            Assert.True(svc.TryChangeState(order.Id, 7, out var e4), e4);

            // 1-й раз у 8: ConfirmOrderDelivery всередині сервісу
            Assert.True(svc.TryChangeState(order.Id, 8, out var e5), e5);

            var after1 = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(reservedStart, after1.ReservedQuantity);        // 0
            Assert.Equal(stockBefore - q, after1.StockQuantity);         // -10

            // 2-й раз у 8 має відхилитися та нічого не змінити
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
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var product = ctx.Products
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 5);

            int productId = product.Id;
            int q = 3;
            int stockBefore = product.StockQuantity;
            int reservedStart = product.ReservedQuantity;

            var order = new CustomerOrder { UserId = 2, OperationTime = DateTime.UtcNow.ToString("u"), OrderStateId = 1 };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Резерв під New
            var p = ctx.Products.First(p => p.Id == productId);
            p.ReservedQuantity += q; // 0 -> 3
            ctx.SaveChanges();

            var stockSvc = new StockReservationService(ctx);

            // Двічі релізимо
            stockSvc.ReleaseOrderReservations(order.Id);
            stockSvc.ReleaseOrderReservations(order.Id);

            var after = ctx.Products.First(p => p.Id == productId);
            Assert.Equal(reservedStart, after.ReservedQuantity); // 0, не < 0
            Assert.Equal(stockBefore, after.StockQuantity);    // склад без змін
        }
        finally { cleanup(); }
    }
}
