using System.Collections.Generic;
using System.Linq;
using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;

namespace StoreDAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly StoreDbContext _db;

        public UserRepository(StoreDbContext db) => _db = db;

        public User? FindByLogin(string login) =>
            _db.Users.FirstOrDefault(u => u.Login == login);

        public User? GetById(int id) =>
            _db.Users.FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() =>
            _db.Users.AsEnumerable();

        public void Add(User user) =>
            _db.Users.Add(user);

        public void SaveChanges() =>
            _db.SaveChanges();
    }
}
