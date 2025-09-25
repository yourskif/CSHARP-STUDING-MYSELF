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
    /// При переході в 8 ConfirmOrderDelivery викликається в сервісі (бізнес-логіка),
    /// тому вручну НЕ викликаємо — просто перевіряємо, що:
    /// - резерв, створений цим замовленням, знято (повернувся до стартового);
    /// - склад зменшився рівно на кількість із замовлення.
    /// </summary>
    [Fact]
    public void HappyPath_New_To_8_ConfirmsAndZeroesReservations()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Беремо товар без стартового резерву (у сиді є такі, напр. #2)
            var product = ctx.Products.AsNoTracking()
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            var productId = product.Id;
            var q = 10;
            var stockBefore = product.StockQuantity;       // напр. 300
            var reservedStart = product.ReservedQuantity;     // 0

            // Створюємо замовлення у стані New (1)
            var order = new CustomerOrder
            {
                UserId = 2, // Registered user із сидів
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            // Додаємо позицію
            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Імітуємо резервування при створенні New (як у реальному UI)
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q; // 0 + 10
            ctx.SaveChanges();

            // Проганяємо стани до 8
            var orderSvc = new CustomerOrderService(ctx);
            Assert.True(orderSvc.TryChangeState(order.Id, 4, out var e1), e1);
            Assert.True(orderSvc.TryChangeState(order.Id, 5, out var e2), e2);
            Assert.True(orderSvc.TryChangeState(order.Id, 6, out var e3), e3);
            Assert.True(orderSvc.TryChangeState(order.Id, 7, out var e4), e4);
            Assert.True(orderSvc.TryChangeState(order.Id, 8, out var e5), e5);

            // ПІСЛЯ переходу в 8 сервіс уже повинен був:
            // - зменшити склад на q
            // - зняти q із резерву (повернувши його до стартового значення)
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
    /// Заборонений перехід: з 1 (New) одразу в 6 (In delivery) — має відхилитися
    /// з повідомленням про дозволені стани.
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
    /// Адміністративне скасування: резерв знімається, склад НЕ змінюється, статус = 3.
    /// </summary>
    [Fact]
    public void AdminCancel_ReleasesReservations_StockUnchanged_Status3()
    {
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            // Продукт без стартового резерву
            var product = ctx.Products.AsNoTracking()
                .OrderBy(p => p.Id)
                .First(p => p.ReservedQuantity == 0 && p.StockQuantity >= 20);

            var productId = product.Id;
            var q = 10;
            var stockBefore = product.StockQuantity;       // напр. 300
            var reservedStart = product.ReservedQuantity;     // 0

            // Замовлення New
            var order = new CustomerOrder
            {
                UserId = 2,
                OperationTime = DateTime.UtcNow.ToString("u"),
                OrderStateId = 1,
            };
            ctx.CustomerOrders.Add(order);
            ctx.SaveChanges();

            // Позиція
            ctx.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.Id,
                ProductId = productId,
                ProductAmount = q,
                Price = product.UnitPrice,
            });
            ctx.SaveChanges();

            // Резервуємо під New
            var pForReserve = ctx.Products.First(p => p.Id == productId);
            pForReserve.ReservedQuantity += q; // 0 + 10
            ctx.SaveChanges();

            // Скасування адміністратором: резерв знімаємо, склад не чіпаємо, статус 3
            var stockSvc = new StockReservationService(ctx);
            stockSvc.ReleaseOrderReservations(order.Id);

            var orderSvc = new CustomerOrderService(ctx);
            Assert.True(orderSvc.TryChangeState(order.Id, 3, out var err), err);

            var pAfter = ctx.Products.AsNoTracking().First(p => p.Id == productId);
            Assert.Equal(reservedStart, pAfter.ReservedQuantity); // 0
            Assert.Equal(stockBefore, pAfter.StockQuantity);    // без змін
        }
        finally
        {
            cleanup();
        }
    }
}
