// Path: console-online-store/StoreBLL/Patterns/CommandPattern.cs
namespace StoreBLL.Patterns;

using System;
using System.Collections.Generic;

/// <summary>
/// Command pattern interface for encapsulating operations.
/// </summary>
public interface ICommand
{
    string Description { get; }

    void Execute();

    void Undo();
}

/// <summary>
/// Command history manager supporting undo operations.
/// </summary>
public class CommandHistory
{
    private readonly Stack<ICommand> executedCommands = new();
    private readonly int maxHistorySize;

    public CommandHistory(int maxHistorySize = 10)
    {
        this.maxHistorySize = maxHistorySize;
    }

    public bool CanUndo => this.executedCommands.Count > 0;

    public void Execute(ICommand command)
    {
        command.Execute();
        this.executedCommands.Push(command);

        while (this.executedCommands.Count > this.maxHistorySize)
        {
            this.executedCommands.TryPop(out _);
        }
    }

    public void Undo()
    {
        if (this.executedCommands.TryPop(out var command))
        {
            command.Undo();
        }
    }

    public IEnumerable<string> GetHistory()
    {
        var result = new List<string>();
        foreach (var cmd in this.executedCommands)
        {
            result.Add(cmd.Description);
        }

        return result;
    }
}

/// <summary>
/// Example: Create Order Command.
/// </summary>
public class CreateOrderCommand : ICommand
{
    private readonly StoreBLL.Services.CustomerOrderService orderService;
    private readonly StoreBLL.Models.CustomerOrderModel order;
    private int createdOrderId;

    public CreateOrderCommand(StoreBLL.Services.CustomerOrderService service, StoreBLL.Models.CustomerOrderModel order)
    {
        this.orderService = service;
        this.order = order;
    }

    public string Description => $"Create Order for User {this.order.UserId}";

    public void Execute()
    {
        this.orderService.Add(this.order);
        this.createdOrderId = this.order.Id;
    }

    public void Undo()
    {
        if (this.createdOrderId > 0)
        {
            this.orderService.Delete(this.createdOrderId);
        }
    }
}
