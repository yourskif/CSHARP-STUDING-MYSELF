using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public void Add(OrderDetail entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(OrderDetail entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrderDetail> GetAll(int pageNumber, int rowCount)
        {
            throw new NotImplementedException();
        }

        public OrderDetail GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(OrderDetail entity)
        {
            throw new NotImplementedException();
        }
    }
}
