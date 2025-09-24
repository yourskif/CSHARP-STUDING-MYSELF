using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class ProductTitleRepository : IProductTitleRepository
    {
        private readonly StoreDbContext context;

        public ProductTitleRepository(StoreDbContext context) => this.context = context;

        public IEnumerable<ProductTitle> GetAll() =>
            this.context.ProductTitles.AsNoTracking().ToList();

        public ProductTitle? GetById(int id) =>
            this.context.ProductTitles.AsNoTracking().FirstOrDefault(t => t.Id == id);
    }
}
