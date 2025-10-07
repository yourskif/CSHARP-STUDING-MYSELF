namespace StoreBLL.Interfaces;

using System.Collections.Generic;

using StoreBLL.Models;

/// <summary>
/// Generic CRUD interface for business logic services.
/// Provides basic Create, Read, Update, Delete operations for entities.
/// </summary>
public interface ICrud
{
    /// <summary>
    /// Retrieves all entities from the data source.
    /// </summary>
    /// <returns>Collection of all entities as <see cref="AbstractModel"/> instances.</returns>
    IEnumerable<AbstractModel> GetAll();

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity with the specified identifier.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when entity with specified id is not found.</exception>
    AbstractModel GetById(int id);

    /// <summary>
    /// Adds a new entity to the data source.
    /// </summary>
    /// <param name="model">The entity model to add.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="model"/> is null.</exception>
    /// <exception cref="System.ArgumentException">Thrown when model data is invalid.</exception>
    void Add(AbstractModel model);

    /// <summary>
    /// Updates an existing entity in the data source.
    /// </summary>
    /// <param name="model">The entity model with updated data.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="model"/> is null.</exception>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when entity with specified id is not found.</exception>
    void Update(AbstractModel model);

    /// <summary>
    /// Deletes an entity from the data source by its identifier.
    /// </summary>
    /// <param name="modelId">The unique identifier of the entity to delete.</param>
    void Delete(int modelId);
}
