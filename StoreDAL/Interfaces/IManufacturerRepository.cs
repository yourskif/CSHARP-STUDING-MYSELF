// StoreDAL/Interfaces/IManufacturerRepository.cs
using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for the Manufacturer entity with explicit CRUD operations.
    /// </summary>
    public interface IManufacturerRepository
    {
        /// <summary>Adds a new manufacturer and persists changes.</summary>
        void Add(Manufacturer entity);

        /// <summary>Deletes the specified manufacturer and persists changes.</summary>
        void Delete(Manufacturer entity);

        /// <summary>Deletes a manufacturer by its identifier and persists changes.</summary>
        void DeleteById(int id);

        /// <summary>Returns all manufacturers.</summary>
        IEnumerable<Manufacturer> GetAll();

        /// <summary>Returns a page of manufacturers.</summary>
        IEnumerable<Manufacturer> GetAll(int pageNumber, int rowCount);

        /// <summary>Returns a manufacturer by its identifier or null if not found.</summary>
        Manufacturer GetById(int id);

        /// <summary>Updates the specified manufacturer and persists changes.</summary>
        void Update(Manufacturer entity);
    }
}
