namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

public class CustomerOrderService : ICrud
{
    private const int StateNew = 1;
    private const int StateCanceledByUser = 2;

    private static readonly Dictionary<int, int[]> AllowedTransitions =
        new Dictionary<int, int[]>
        {
            [1] = new int[] { 2, 3, 4 }, // New -> Canceled(user/admin) or Confirmed
            [4] = new int[] { 3, 5 },    // Confirmed -> Canceled by admin or Moved
            [5] = new int[] { 6 },       // Moved -> In delivery
            [6] = new int[] { 7 },       // In delivery -> Delivered
            [7] = new int[] { 8 },       // Delivered -> Confirmed by client
        };

    private readonly ICustomerOrderRepository repository;
    private readonly IOrderStateRepository stateRepository;

    public CustomerOrderService(StoreDbContext context)
    {
        this.repository = new CustomerOrderRepository(context);
        this.stateRepository = new OrderStateRepository(context);
    }

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
            orderStateId: m.OrderStateId == 0 ? StateNew : m.OrderStateId);

        this.repository.Add(entity);
        m.Id = entity.Id;
    }

    public void Delete(int modelId) => this.repository.DeleteById(modelId);

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(o =>
            new CustomerOrderModel(
                id: o.Id,
                userId: o.UserId,
                operationTime: o.OperationTime,
                orderStateId: o.OrderStateId));
    }

    public AbstractModel? GetById(int id)
    {
        var o = this.repository.GetById(id);
        if (o == null)
        {
            return null;
        }

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

    public string GetStateName(int stateId) =>
        this.stateRepository.GetById(stateId)?.StateName ?? $"State #{stateId}";

    public bool TryChangeState(int orderId, int newStateId)
    {
        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            return false;
        }

        if (!AllowedTransitions.TryGetValue(entity.OrderStateId, out var allowed) ||
            !allowed.Contains(newStateId))
        {
            return false;
        }

        entity.OrderStateId = newStateId;
        entity.OperationTime = DateTime.UtcNow.ToString("u");
        this.repository.Update(entity);
        return true;
    }

    public bool CancelOwnOrder(int orderId, int currentUserId, out string? error)
    {
        error = null;

        var entity = this.repository.GetById(orderId);
        if (entity == null)
        {
            error = "Order not found.";
            return false;
        }

        if (entity.UserId != currentUserId)
        {
            error = "You can cancel only your own order.";
            return false;
        }

        if (!AllowedTransitions.TryGetValue(entity.OrderStateId, out var allowed) ||
            !allowed.Contains(StateCanceledByUser))
        {
            error = $"Can't cancel order in state '{this.GetStateName(entity.OrderStateId)}'.";
            return false;
        }

        entity.OrderStateId = StateCanceledByUser;
        entity.OperationTime = DateTime.UtcNow.ToString("u");
        this.repository.Update(entity);
        return true;
    }
}
