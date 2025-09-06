// StoreDAL/Interfaces/IOrderDetailRepository.cs
using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    /// <summary>
    /// Repository contract for the OrderDetail entity with explicit CRUD operations.
    /// </summary>
    public interface IOrderDetailRepository
    {
        void Add(OrderDetail entity);
        void Delete(OrderDetail entity);
        void DeleteById(int id);
        IEnumerable<OrderDetail> GetAll();
        IEnumerable<OrderDetail> GetAll(int pageNumber, int rowCount);

        /// <summary>Returns an order detail by its identifier or null if not found.</summary>
        OrderDetail? GetById(int id);

        void Update(OrderDetail entity);
    }
}
