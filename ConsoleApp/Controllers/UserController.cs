using System;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;

namespace ConsoleApp.Controllers
{
    public class UserController
    {
        private readonly UserService service;

        public UserController(StoreDbContext context)
        {
            this.service = new UserService(context);
        }

        public void ShowUser(int id)
        {
            var user = (UserModel)this.service.GetById(id);

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return;
            }

            Console.WriteLine($"{user.FirstName} {user.LastName} ({user.Login})");
        }
    }
}
