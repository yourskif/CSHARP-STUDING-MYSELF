// Path: console-online-store/Store.Tests/CommandPatternTests.cs
using System;
using StoreBLL.Patterns;
using StoreBLL.Services;
using StoreBLL.Models;
using Xunit;

namespace Store.Tests;

public class CommandPatternTests
{
    [Fact]
    public void CommandHistory_ExecuteAndUndo_Works()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var orderService = new CustomerOrderService(ctx);
            var history = new CommandHistory();

            var order = new CustomerOrderModel
            {
                UserId = 2,
                OrderStateId = 1,
                OperationTime = DateTime.UtcNow.ToString("u"),
            };

            var command = new CreateOrderCommand(orderService, order);

            // Act - Execute
            history.Execute(command);
            Assert.True(order.Id > 0);

            var createdOrder = orderService.GetById(order.Id);
            Assert.NotNull(createdOrder);

            // Act - Undo
            Assert.True(history.CanUndo);
            history.Undo();

            // Assert - Order should be deleted
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>
                orderService.GetById(order.Id));
        }
        finally { cleanup(); }
    }

    [Fact]
    public void CommandHistory_GetHistory_ReturnsDescriptions()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var orderService = new CustomerOrderService(ctx);
            var history = new CommandHistory();

            var order1 = new CustomerOrderModel { UserId = 2, OrderStateId = 1, OperationTime = DateTime.UtcNow.ToString("u") };
            var order2 = new CustomerOrderModel { UserId = 2, OrderStateId = 1, OperationTime = DateTime.UtcNow.ToString("u") };

            // Act
            history.Execute(new CreateOrderCommand(orderService, order1));
            history.Execute(new CreateOrderCommand(orderService, order2));

            var historyList = history.GetHistory();

            // Assert
            Assert.NotEmpty(historyList);
            Assert.Contains("Create Order", string.Join(" ", historyList));
        }
        finally { cleanup(); }
    }

    [Fact]
    public void CommandHistory_MaxSize_LimitsHistory()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var orderService = new CustomerOrderService(ctx);
            var history = new CommandHistory(maxHistorySize: 2);

            // Act - Add 3 commands
            for (int i = 0; i < 3; i++)
            {
                var order = new CustomerOrderModel { UserId = 2, OrderStateId = 1, OperationTime = DateTime.UtcNow.ToString("u") };
                history.Execute(new CreateOrderCommand(orderService, order));
            }

            var historyList = history.GetHistory();

            // Assert - Only 2 commands should be in history
            Assert.Equal(2, historyList.Count());
        }
        finally { cleanup(); }
    }
}
