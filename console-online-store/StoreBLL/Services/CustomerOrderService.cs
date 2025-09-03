namespace StoreBLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBLL.Interfaces;
using StoreBLL.Models;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
public class CustomerOrderService : ICrud
{
    public CustomerOrderService(StoreDbContext context)
    {
    }

    public void Add(AbstractModel model)
    {
        throw new NotImplementedException();
    }

    public void Delete(int modelId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public AbstractModel GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(AbstractModel model)
    {
        throw new NotImplementedException();
    }
}