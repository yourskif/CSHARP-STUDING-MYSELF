using System.Collections.Generic;

using StoreDAL.Entities;

namespace StoreDAL.Interfaces
{
    public interface IUserRepository
    {
        User? FindByLogin(string login);

        User? GetById(int id);

        IEnumerable<User> GetAll();

        void Add(User user);

        void SaveChanges();
    }
}
