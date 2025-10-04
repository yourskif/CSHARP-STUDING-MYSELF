using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    public interface IUserRoleRepository
    {
        UserRole? GetById(int id);

        UserRole? GetByName(string name);

        // РџРѕС‚СЂС–Р±РЅРѕ РґР»СЏ РІР°С€РѕРіРѕ UserRoleService:
        IEnumerable<UserRole> GetAll();

        void Add(UserRole role);

        void Update(UserRole role);

        void DeleteById(int id);

        void SaveChanges();
    }
}
