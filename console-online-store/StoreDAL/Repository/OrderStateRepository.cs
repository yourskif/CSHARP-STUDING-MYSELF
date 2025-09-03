namespace StoreDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

public class OrderStateRepository : AbstractRepository, IOrderStateRepository
{
    private readonly DbSet<OrderState> dbSet;

    public OrderStateRepository(StoreDbContext context)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.dbSet = context.Set<OrderState>();
    }

    public void Add(OrderState entity)
    {
        this.dbSet.Add(entity);
        this.context.SaveChanges();
    }

    public void Delete(OrderState entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteById(int id)
    {
        var entity = this.dbSet.Find(id);
        if (entity != null)
        {
            this.dbSet.Remove(entity);
            this.context.SaveChanges();
        }
    }

    public IEnumerable<OrderState> GetAll()
    {
        return this.dbSet.ToList();
    }

    public IEnumerable<OrderState> GetAll(int pageNumber, int rowCount)
    {
        throw new NotImplementedException();
    }

    public OrderState GetById(int id)
    {
        return this.dbSet.Find(id);
    }

    public void Update(OrderState entity)
    {
        throw new NotImplementedException();
    }
}