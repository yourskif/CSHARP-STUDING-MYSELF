// StoreBLL/Interfaces/ICrud.cs
namespace StoreBLL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StoreBLL.Models;

    /// <summary>
    /// Minimal CRUD contract for BLL services working with AbstractModel.
    /// Concrete services can expose richer, strongly-typed APIs in addition.
    /// </summary>
    public interface ICrud
    {
        /// <summary>Returns all items.</summary>
        IEnumerable<AbstractModel> GetAll();

        /// <summary>Returns an item by id or null if not found.</summary>
        AbstractModel? GetById(int id);

        /// <summary>
        /// Returns a page of items. Default implementation pages over GetAll().
        /// Implementors can override if they have a more efficient data-source paging.
        /// </summary>
        IEnumerable<AbstractModel> GetAll(int pageNumber, int rowCount)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }

            if (rowCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }

            var all = this.GetAll() ?? Enumerable.Empty<AbstractModel>();
            return all.Skip((pageNumber - 1) * rowCount).Take(rowCount);
        }
    }
}
