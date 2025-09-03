using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDAL.Data;

namespace StoreDAL.Repository
{
    public abstract class AbstractRepository
    {
        protected readonly StoreDbContext context;

        protected AbstractRepository(StoreDbContext context)
        {
            this.context = context;
        }
    }
}
