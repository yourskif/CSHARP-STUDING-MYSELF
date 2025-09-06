// StoreDAL/Interfaces/ICustomerOrderRepository.cs
using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for the CustomerOrder entity with explicit CRUD operations.
    /// </summary>
    public interface ICustomerOrderRepository
    {
        void Add(CustomerOrder entity);
        void Delete(CustomerOrder entity);
        void DeleteById(int id);
        IEnumerable<CustomerOrder> GetAll();
        IEnumerable<CustomerOrder> GetAll(int pageNumber, int rowCount);

        /// <summary>Returns a customer order by its identifier or null if not found.</summary>
        CustomerOrder? GetById(int id);

        void Update(CustomerOrder entity);
    }
}
