using System.Collections.Generic;
using System.Linq;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContext db;

        public UserRepository(StoreDbContext db) => this.db = db;

        public User? FindByLogin(string login) =>
            this.db.Users.FirstOrDefault(u => u.Login == login);

        public User? GetById(int id) =>
            this.db.Users.FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() =>
            this.db.Users.AsEnumerable();

        public void Add(User user) =>
            this.db.Users.Add(user);

        public void SaveChanges() =>
            this.db.SaveChanges();
    }
}
