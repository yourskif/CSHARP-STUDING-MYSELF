using System.Collections.Generic;
using System.Linq;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly StoreDbContext context;

        public ManufacturerRepository(StoreDbContext context)
        {
            this.context = context;
        }

        public void Add(Manufacturer entity)
        {
            this.context.Manufacturers.Add(entity);
            this.context.SaveChanges();
        }

        public void Delete(Manufacturer entity)
        {
            this.context.Manufacturers.Remove(entity);
            this.context.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var manufacturer = this.context.Manufacturers.Find(id);
            if (manufacturer != null)
            {
                this.context.Manufacturers.Remove(manufacturer);
                this.context.SaveChanges();
            }
        }

        public IEnumerable<Manufacturer> GetAll()
        {
            return this.context.Manufacturers.ToList();
        }

        public IEnumerable<Manufacturer> GetAll(int pageNumber, int rowCount)
        {
            return this.context.Manufacturers
                .Skip((pageNumber - 1) * rowCount)
                .Take(rowCount)
                .ToList();
        }

        public Manufacturer GetById(int id)
        {
            return this.context.Manufacturers.Find(id);
        }

        public void Update(Manufacturer entity)
        {
            this.context.Manufacturers.Update(entity);
            this.context.SaveChanges();
        }
    }
}
