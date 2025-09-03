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
using static System.Runtime.InteropServices.JavaScript.JSType;

public class UserRoleRepository : AbstractRepository, IUserRoleRepository
{
    private readonly DbSet<UserRole> dbSet;

    public UserRoleRepository(StoreDbContext context)
        : base(context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.dbSet = context.Set<UserRole>();
    }

    public void Add(UserRole entity)
    {
        this.dbSet.Add(entity);
        this.context.SaveChanges();
    }

    public void Delete(UserRole entity)
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

    public IEnumerable<UserRole> GetAll()
    {
        return this.dbSet.ToList();
    }

    public IEnumerable<UserRole> GetAll(int pageNumber, int rowCount)
    {
        throw new NotImplementedException();
    }

    public UserRole GetById(int id)
    {
        return this.dbSet.Find(id);
    }

    public void Update(UserRole entity)
    {
        throw new NotImplementedException();
    }
}