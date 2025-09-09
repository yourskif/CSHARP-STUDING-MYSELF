using StoreDAL.Entities;
using System.Collections.Generic;

namespace StoreDAL.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAllWithIncludes();
        IEnumerable<Product> GetByCategoryId(int categoryId);
        Product? GetByIdWithIncludes(int id);
    }
}
