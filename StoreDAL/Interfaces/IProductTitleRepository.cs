using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    public interface IProductTitleRepository
    {
        IEnumerable<ProductTitle> GetAll();

        ProductTitle? GetById(int id);
    }
}
