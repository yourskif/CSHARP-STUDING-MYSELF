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
/// Note: Update operation is not implemented as order states are typically read-only reference data.
/// </summary>
public class OrderStateService : ICrud
{
    private readonly IOrderStateRepository repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderStateService"/> class.
    /// </summary>
    /// <param name="context">Database context for order state operations.</param>
    public OrderStateService(StoreDbContext context)
    {
        this.repository = new OrderStateRepository(context);
    }

    /// <summary>
    /// Adds a new order state to the database.
    /// </summary>
    /// <param name="model">Order state model to add.</param>
    /// <exception cref="InvalidCastException">Thrown when model is not OrderStateModel.</exception>
    public void Add(AbstractModel model)
    {
        var x = (OrderStateModel)model;
        this.repository.Add(new OrderState(x.Id, x.StateName));
    }

    /// <summary>
    /// Deletes an order state by its identifier.
    /// </summary>
    /// <param name="modelId">Order state identifier to delete.</param>
    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    /// <summary>
    /// Gets all order states from the database.
    /// </summary>
    /// <returns>Collection of order state models.</returns>
    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(x => new OrderStateModel(x.Id, x.StateName));
    }

    /// <summary>
    /// Gets an order state by its unique identifier.
    /// </summary>
    /// <param name="id">Order state identifier.</param>
    /// <returns>Order state model.</returns>
    public AbstractModel GetById(int id)
    {
        var res = this.repository.GetById(id);
        return new OrderStateModel(res.Id, res.StateName);
    }

    /// <summary>
    /// Updates an order state (not implemented).
    /// Order states are typically treated as read-only reference data.
    /// </summary>
    /// <param name="model">Order state model with updated data.</param>
    /// <exception cref="NotImplementedException">This operation is not supported for order states.</exception>
    public void Update(AbstractModel model)
    {
        throw new NotImplementedException("Order states are read-only reference data and cannot be updated.");
    }
}
