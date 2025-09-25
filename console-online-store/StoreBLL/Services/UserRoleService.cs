namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

public class UserRoleService : ICrud
{
    private readonly IUserRoleRepository repository;

    public UserRoleService(StoreDbContext context)
    {
        this.repository = new UserRoleRepository(context);
    }

    public void Add(AbstractModel model)
    {
        if (model is not UserRoleModel m)
        {
            throw new ArgumentException("Expected UserRoleModel", nameof(model));
        }

        // Map BLL -> DAL
        this.repository.Add(new UserRole(m.Id, m.RoleName));
    }

    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        // Map DAL -> BLL
        return this.repository
            .GetAll()
            .Select(x => (AbstractModel)new UserRoleModel(x.Id, x.RoleName));
    }

    public AbstractModel GetById(int id)
    {
        var res = this.repository.GetById(id)
                  ?? throw new InvalidOperationException($"UserRole with id={id} not found");

        // Map DAL -> BLL
        return new UserRoleModel(res.Id, res.RoleName);
    }

    public void Update(AbstractModel model)
    {
        if (model is not UserRoleModel m)
        {
            throw new ArgumentException("Expected UserRoleModel", nameof(model));
        }

        // Map BLL -> DAL
        this.repository.Update(new UserRole(m.Id, m.RoleName));
    }
}
