using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        public void Add(CustomerOrder entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(CustomerOrder entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerOrder> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerOrder> GetAll(int pageNumber, int rowCount)
        {
            throw new NotImplementedException();
        }

        public CustomerOrder GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(CustomerOrder entity)
        {
            throw new NotImplementedException();
        }
    }
}
