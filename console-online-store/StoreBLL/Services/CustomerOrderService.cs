// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\console-online-store\StoreBLL\Services\CustomerOrderService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Repository;

/// <summary>
/// Business logic service for customer orders (CRUD + state transitions)
/// Включає інвентарні побічні ефекти для відміни/підтвердження доставки.
/// </summary>
public class CustomerOrderService
{
    /// <summary>
    /// Дозволені переходи статусів.
    /// 1: New
    /// 2: Cancelled by user (terminal)
    /// 3: Cancelled by administrator (terminal)
    /// 4: Confirmed
    /// 5: Moved to delivery company
    /// 6: In delivery
    /// 7: Delivered to client
    /// 8: Delivery confirmed by client (terminal)
    /// </summary>
    private static readonly Dictionary<int, int[]> AllowedTransitions = new Dictionary<int, int[]>
    {
        { 1, new[] { 2, 3, 4 } }, // New -> Cancel(user/admin) or Confirmed
        { 4, new[] { 3, 5 } },    // Confirmed -> Cancel(admin) or Moved
        { 5, new[] { 6 } },       // Moved -> In delivery
        { 6, new[] { 7 } },       // In delivery -> Delivered to client
        { 7, new[] { 8 } },       // Delivered -> Delivery confirmed by client
    };

    private readonly StoreDbContext context;
    private readonly CustomerOrderRepository repository;
    private readonly StockReservationService stockService;

    public CustomerOrderService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.repository = new CustomerOrderRepository(context);
        this.stockService = new StockReservationService(context);
    }

    public static string StatusName(int id) =>
        id switch
        {
            1 => "New Order",
            2 => "Cancelled by user",
            3 => "Cancelled by administrator",
            4 => "Confirmed",
            5 => "Moved to delivery company",
            6 => "In delivery",
            7 => "Delivered to client",
            8 => "Delivery confirmed by client",
            _ => "Unknown",
        };

    public static IReadOnlyList<int> GetAllowedNextStates(int currentStateId) =>
        AllowedTransitions.TryGetValue(currentStateId, out var arr)
            ? Array.AsReadOnly(arr)
            : Array.Empty<int>();

    public static bool CanTransition(int fromStateId, int toStateId) =>
        AllowedTransitions.TryGetValue(fromStateId, out var allowed) && allowed.Contains(toStateId);

    // ---------- CRUD ----------

    public void Add(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
        }

        var entity = new CustomerOrder(
            id: 0,
            operationTime: m.OperationTime ?? DateTime.UtcNow.ToString("u"),
            userId: m.UserId,
            orderStateId: m.OrderStateId);

        this.repository.Add(entity);
        m.Id = entity.Id;
    }

    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    public IEnumerable<AbstractModel> GetAll() =>
        this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));

    public AbstractModel GetById(int id)
    {
        var o = this.repository.GetById(id)
            ?? throw new KeyNotFoundException($"Order with id {id} was not found.");

        return new CustomerOrderModel(
            id: o.Id,
            userId: o.UserId,
            operationTime: o.OperationTime,
            orderStateId: o.OrderStateId);
    }

    public void Update(AbstractModel model)
    {
        if (model is not CustomerOrderModel m)
        {
            throw new ArgumentException("Expected CustomerOrderModel", nameof(model));
        }

        var entity = this.repository.GetById(m.Id);
        if (entity == null)
        {
            return;
        }

        entity.UserId = m.UserId;
        entity.OperationTime = m.OperationTime ?? entity.OperationTime;
        entity.OrderStateId = m.OrderStateId;
        this.repository.Update(entity);
    }

    /// <summary>
    /// Безпечно змінює стан замовлення з валідацією дозволених переходів
    /// та інвентарними побічними ефектами:
    /// - на 3 (cancel admin) → звільняє резерв;
    /// - на 8 (confirm by client) → списує склад і обнуляє резерв.
    /// </summary>
    public bool TryChangeState(int orderId, int newStateId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (!CanTransition(entity.OrderStateId, newStateId))
        {
            var next = string.Join(", ", GetAllowedNextStates(entity.OrderStateId).Select(StatusName));
            error = $"Transition not allowed. Current: {StatusName(entity.OrderStateId)}. Allowed next: [{next}].";
            return false;
        }

        // 1) Save new state
        entity.OrderStateId = newStateId;
        this.repository.Update(entity);
        this.context.SaveChanges();

        // 2) Inventory side-effects
        if (newStateId == 3)
        {
            // Admin cancel → release reservations
            this.stockService.ReleaseOrderReservations(orderId);
        }
        else if (newStateId == 8)
        {
            // Confirm delivery → decrease stock & zero reservations
            this.stockService.ConfirmOrderDelivery(orderId);
        }

        return true;
    }

    /// <summary>
    /// Скасування власного замовлення користувачем (тільки якщо стан 1: New).
    /// Спочатку звільняємо резерви, потім міняємо стан на 2.
    /// </summary>
    public bool CancelOwnOrder(int orderId, int userId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (entity.UserId != userId)
        {
            error = "You can only cancel your own orders.";
            return false;
        }

        if (entity.OrderStateId != 1)
        {
            error = "Only new orders can be canceled.";
            return false;
        }

        this.stockService.ReleaseOrderReservations(orderId);

        entity.OrderStateId = 2;
        this.repository.Update(entity);
        this.context.SaveChanges();

        return true;
    }

    /// <summary>
    /// Підтвердження отримання користувачем (7 → 8) з інвентарними ефектами.
    /// </summary>
    public bool MarkAsReceived(int orderId, int userId, out string error)
    {
        error = string.Empty;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (entity.UserId != userId)
        {
            error = "You can only confirm receipt of your own orders.";
            return false;
        }

        if (entity.OrderStateId != 7)
        {
            error = "Only delivered orders can be marked as received.";
            return false;
        }

        entity.OrderStateId = 8;
        this.repository.Update(entity);
        this.context.SaveChanges();

        this.stockService.ConfirmOrderDelivery(orderId);
        return true;
    }

    public IEnumerable<CustomerOrderModel> GetOrdersByUser(int userId) =>
        this.repository.GetAll()
            .Where(o => o.UserId == userId)
            .Select(o => new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId))
            .OrderByDescending(o => o.Id);
}
