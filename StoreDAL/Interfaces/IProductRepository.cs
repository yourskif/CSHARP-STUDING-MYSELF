using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllWithIncludes();

        IEnumerable<Product> GetByCategoryId(int categoryId);

        Product? GetByIdWithIncludes(int id);
    }
}
