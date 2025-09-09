using StoreDAL.Entities;
using System.Collections.Generic;

namespace StoreDAL.Interfaces
{
    public interface IProductTitleRepository
    {
        IEnumerable<ProductTitle> GetAll();
        ProductTitle? GetById(int id);
    }
}
