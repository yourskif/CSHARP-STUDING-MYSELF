// StoreDAL/Interfaces/ICategoryRepository.cs
using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for the Category entity with explicit CRUD operations.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>Adds a new category and persists changes.</summary>
        void Add(Category entity);

        /// <summary>Deletes the specified category and persists changes.</summary>
        void Delete(Category entity);

        /// <summary>Deletes a category by its identifier and persists changes.</summary>
        void DeleteById(int id);

        /// <summary>Returns all categories.</summary>
        IEnumerable<Category> GetAll();

        /// <summary>Returns a page of categories.</summary>
        IEnumerable<Category> GetAll(int pageNumber, int rowCount);

        /// <summary>
        /// Returns a category by its identifier or null if not found.
        /// </summary>
        Category GetById(int id);

        /// <summary>Updates the specified category and persists changes.</summary>
        void Update(Category entity);
    }
}
