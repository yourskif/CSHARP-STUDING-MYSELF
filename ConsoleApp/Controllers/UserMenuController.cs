using ConsoleMenu;
using ConsoleMenu.Builder;
using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

namespace ConsoleApp.Controllers
{
    public enum UserRoles
    {
        Guest,
        Administrator,
        RegisteredCustomer,
    }

    public static class UserMenuController
    {
        private static readonly Dictionary<UserRoles, Menu> RolesToMenu;
        private static int userId;
        private static UserRoles userRole;
        private static StoreDbContext context;

        static UserMenuController()
        {
            userId = 0;
            userRole = UserRoles.Guest;
            RolesToMenu = new Dictionary<UserRoles, Menu>();
            var factory = new StoreDbFactory(new TestDataFactory());
            context = factory.CreateContext();
            RolesToMenu.Add(UserRoles.Guest, new GuestMainMenu().Create(context));
            RolesToMenu.Add(UserRoles.RegisteredCustomer, new UserMainMenu().Create(context));
            RolesToMenu.Add(UserRoles.Administrator, new AdminMainMenu().Create(context));
        }

        public static StoreDbContext Context
        {
            get { return context; }
        }

        public static void Login()
        {
            Console.WriteLine("Login: ");
            var login = Console.ReadLine();
            Console.WriteLine("Password: ");
            var password = Console.ReadLine();

            // TODO: Implement proper authentication with password encryption
            if (login == "admin")
            {
                userId = 1;
                userRole = UserRoles.Administrator;
            }
            else if (login == "user")
            {
                userId = 2;
                userRole = UserRoles.RegisteredCustomer;
            }
        }

        public static void Register()
        {
            Console.WriteLine("Registration");
            Console.WriteLine("Enter username: ");
            var username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            var password = Console.ReadLine();
            Console.WriteLine("Enter first name: ");
            var firstName = Console.ReadLine();
            Console.WriteLine("Enter last name: ");
            var lastName = Console.ReadLine();

            // TODO: Implement user registration with UserService
            Console.WriteLine("Registration successful! Please login.");
        }

        public static void Logout()
        {
            userId = 0;
            userRole = UserRoles.Guest;
            Console.WriteLine("You have been logged out.");
        }

        public static void Start()
        {
            ConsoleKey resKey;
            bool updateItems = true;
            do
            {
                resKey = RolesToMenu[userRole].RunOnce(ref updateItems);
            }
            while (resKey != ConsoleKey.Escape);
        }
    }
}