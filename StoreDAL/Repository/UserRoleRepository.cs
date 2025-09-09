using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly StoreDbContext context;

        public UserRoleRepository(StoreDbContext context) => this.context = context;

        public UserRole? GetById(int id) =>
            this.context.UserRoles.FirstOrDefault(r => r.Id == id);

        // Якщо у сутності немає властивості Name — EF.Property читає однойменну колонку.
        public UserRole? GetByName(string name) =>
            this.context.UserRoles.FirstOrDefault(r => EF.Property<string>(r, "Name") == name);

        public IEnumerable<UserRole> GetAll() =>
            this.context.UserRoles.AsNoTracking().ToList();

        public void Add(UserRole role) =>
            this.context.UserRoles.Add(role);

        public void Update(UserRole role) =>
            this.context.UserRoles.Update(role);

        public void DeleteById(int id)
        {
            var entity = this.context.UserRoles.FirstOrDefault(r => r.Id == id);
            if (entity != null)
            {
                this.context.UserRoles.Remove(entity);
            }
        }

        public void SaveChanges() => this.context.SaveChanges();
    }
}
