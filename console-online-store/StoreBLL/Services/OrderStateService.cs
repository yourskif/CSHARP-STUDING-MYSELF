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

/// <summary>
/// Service for managing order states with CRUD operations.
/// Provides business logic layer for order state entities.
/// </summary>
/// <remarks>
/// Order states represent the workflow status of customer orders (e.g., New Order, Confirmed, Delivered).
/// These are typically treated as read-only reference data, so the Update operation is not implemented.
/// Standard order states include:
/// <list type="bullet">
/// <item><description>1 - New Order</description></item>
/// <item><description>2 - Cancelled by user</description></item>
/// <item><description>3 - Cancelled by administrator</description></item>
/// <item><description>4 - Confirmed</description></item>
/// <item><description>5 - Moved to delivery company</description></item>
/// <item><description>6 - In delivery</description></item>
/// <item><description>7 - Delivered to client</description></item>
/// <item><description>8 - Delivery confirmed by client</description></item>
/// </list>
/// </remarks>
public class OrderStateService : ICrud
{
    private readonly IOrderStateRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStateService"/> class.
    /// </summary>
    /// <param name="context">Database context for order state operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    public OrderStateService(StoreDbContext context)
    {
        this.repository = new OrderStateRepository(context);
    }

    /// <summary>
    /// Adds a new order state to the database.
    /// </summary>
    /// <param name="model">Order state model containing data to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when <paramref name="model"/> is not of type <see cref="OrderStateModel"/>.</exception>
    /// <remarks>
    /// This operation is typically used only during initial database setup.
    /// Order states should be predefined and rarely modified in production.
    /// </remarks>
    public void Add(AbstractModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var x = (OrderStateModel)model;
        this.repository.Add(new OrderState(x.Id, x.StateName));
    }

    /// <summary>
    /// Deletes an order state by its identifier.
    /// </summary>
    /// <param name="modelId">The unique identifier of the order state to delete.</param>
    /// <remarks>
    /// Caution: Deleting order states that are referenced by existing orders may cause data integrity issues.
    /// Ensure no orders reference this state before deletion.
    /// </remarks>
    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    /// <summary>
    /// Retrieves all order states from the database.
    /// </summary>
    /// <returns>
    /// Collection of all order states as <see cref="AbstractModel"/> instances.
    /// Typically returns the complete set of workflow states defined in the system.
    /// </returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(x => new OrderStateModel(x.Id, x.StateName));
    }

    /// <summary>
    /// Retrieves a single order state by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order state.</param>
    /// <returns>The order state model with the specified identifier.</returns>
    /// <exception cref="InvalidOperationException">Thrown when order state with specified <paramref name="id"/> is not found.</exception>
    public AbstractModel GetById(int id)
    {
        var res = this.repository.GetById(id);
        return new OrderStateModel(res.Id, res.StateName);
    }

    /// <summary>
    /// Updates an order state. This operation is not supported.
    /// </summary>
    /// <param name="model">Order state model with updated data.</param>
    /// <exception cref="NotImplementedException">
    /// Always thrown. Order states are read-only reference data and cannot be updated through the service layer.
    /// </exception>
    /// <remarks>
    /// Order states are treated as system configuration data that should remain stable.
    /// If you need to modify an order state, consider using database migration scripts or direct database updates.
    /// </remarks>
    public void Update(AbstractModel model)
    {
        throw new NotImplementedException("Order states are read-only reference data and cannot be updated.");
    }
}
