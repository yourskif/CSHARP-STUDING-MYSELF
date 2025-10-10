namespace StoreDAL.Interfaces;

using System.Collections.Generic;

using StoreDAL.Entities;

/// <summary>
/// Generic repository interface for data access operations.
/// Provides standard CRUD (Create, Read, Update, Delete) operations for entities that inherit from <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by this repository. Must inherit from <see cref="BaseEntity"/>.</typeparam>
/// <remarks>
/// <para>
/// This interface implements the Repository pattern, providing an abstraction layer between
/// the business logic and data access logic. It encapsulates the logic required to access
/// data sources and provides a more object-oriented view of the persistence layer.
/// </para>
/// <para>
/// Benefits of using this repository pattern:
/// <list type="bullet">
/// <item><description>Separation of concerns: Business logic is decoupled from data access implementation</description></item>
/// <item><description>Testability: Easy to mock for unit testing</description></item>
/// <item><description>Centralized data access logic: Reduces code duplication</description></item>
/// <item><description>Flexibility: Easy to switch data sources without affecting business logic</description></item>
/// </list>
/// </para>
/// </remarks>
public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    /// <summary>
    /// Retrieves all entities from the data source.
    /// </summary>
    /// <returns>
    /// A collection containing all entities of type <typeparamref name="TEntity"/> in the data source.
    /// Returns an empty collection if no entities exist.
    /// </returns>
    /// <remarks>
    /// Warning: This method loads all entities into memory. For large data sets, consider using
    /// the paginated version <see cref="GetAll(int, int)"/> to avoid performance issues.
    /// </remarks>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Retrieves a paginated subset of entities from the data source.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based index). First page is 1.</param>
    /// <param name="rowCount">The number of entities to return per page.</param>
    /// <returns>
    /// A collection containing the specified page of entities.
    /// Returns an empty collection if the page is beyond the available data.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method implements server-side pagination to efficiently handle large data sets.
    /// </para>
    /// <para>
    /// Example: To get the second page with 10 items per page, call <c>GetAll(2, 10)</c>.
    /// This will skip the first 10 items and return items 11-20.
    /// </para>
    /// </remarks>
    IEnumerable<TEntity> GetAll(int pageNumber, int rowCount);

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified identifier.
    /// </returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">
    /// Thrown when no entity with the specified <paramref name="id"/> exists in the data source.
    /// </exception>
    /// <remarks>
    /// This method performs a direct lookup by primary key, which is typically the most efficient
    /// way to retrieve a single entity.
    /// </remarks>
    TEntity GetById(int id);

    /// <summary>
    /// Adds a new entity to the data source.
    /// </summary>
    /// <param name="entity">The entity to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="entity"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// After calling this method, changes may need to be explicitly saved depending on the implementation.
    /// Some implementations auto-save, while others require calling a SaveChanges method.
    /// </para>
    /// <para>
    /// The entity's ID property is typically assigned by the data source after the add operation completes.
    /// </para>
    /// </remarks>
    void Add(TEntity entity);

    /// <summary>
    /// Deletes an entity from the data source by entity reference.
    /// </summary>
    /// <param name="entity">The entity to delete. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="entity"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// This method requires the full entity object. If you only have the ID, use <see cref="DeleteById(int)"/> instead.
    /// </para>
    /// <para>
    /// After calling this method, changes may need to be explicitly saved depending on the implementation.
    /// </para>
    /// </remarks>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes an entity from the data source by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <remarks>
    /// <para>
    /// This method is idempotent - calling it multiple times with the same ID has no additional effect
    /// after the first successful deletion. It does not throw an exception if the entity doesn't exist.
    /// </para>
    /// <para>
    /// After calling this method, changes may need to be explicitly saved depending on the implementation.
    /// </para>
    /// </remarks>
    void DeleteById(int id);

    /// <summary>
    /// Updates an existing entity in the data source with new values.
    /// </summary>
    /// <param name="entity">The entity with updated values. Must not be <see langword="null"/> and must have a valid ID.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="entity"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">
    /// Thrown when no entity with the specified ID exists in the data source.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The entity's ID must match an existing entity in the data source. All other properties
    /// will be updated to match the values in the provided entity object.
    /// </para>
    /// <para>
    /// After calling this method, changes may need to be explicitly saved depending on the implementation.
    /// </para>
    /// </remarks>
    void Update(TEntity entity);
}
