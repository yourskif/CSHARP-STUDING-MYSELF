// StoreDAL/Interfaces/IRepository.cs
using System.Collections.Generic;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Generic repository contract for basic CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>Returns all entities.</summary>
        IEnumerable<T> GetAll();

        /// <summary>Returns a page of entities.</summary>
        IEnumerable<T> GetAll(int pageNumber, int rowCount);

        /// <summary>Returns an entity by id or null if not found.</summary>
        T? GetById(int id);

        /// <summary>Creates a new entity. Returns number of affected rows.</summary>
        int Create(T entity);

        /// <summary>Updates an existing entity. Returns true if changes were saved.</summary>
        bool Update(T entity);

        /// <summary>Deletes an entity by id. Returns true if changes were saved.</summary>
        bool Delete(int id);
    }
}
