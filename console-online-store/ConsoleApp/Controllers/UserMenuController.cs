// Path: console-online-store/ConsoleApp/Controllers/UserMenuController.cs
namespace ConsoleApp.Controllers;

using System;

using StoreBLL.Models;
using StoreBLL.Services;

using StoreDAL.UnitOfWork;

/// <summary>
/// Main menu controller for user operations.
/// Manages authentication and navigation.
/// </summary>
public static class UserMenuController
{
    public static IStoreUnitOfWork? UnitOfWork { get; set; }

    public static UserModel? CurrentUser { get; private set; }

    /// <summary>
    /// Sets the current user (for login/logout operations).
    /// </summary>
    /// <param name="user">User to set as current, or null to logout.</param>
    public static void SetCurrentUser(UserModel? user)
    {
        CurrentUser = user;
    }

    /// <summary>App entry: main menu loop.</summary>
    public static void Start()
    {
#pragma warning disable CA2000 // Context is intentionally kept alive for entire application lifetime
        // Create context and wrap in UnitOfWork - kept alive for entire app lifetime
        var context = StoreDAL.Data.StoreDbFactory.Create();
        UnitOfWork = new StoreDAL.UnitOfWork.StoreUnitOfWork(context);
#pragma warning restore CA2000

        while (true)
        {
            Console.Clear();
            if (CurrentUser == null)
            {
                ShowGuestMenu();
            }
            else
            {
                ShowUserMenu();
            }
        }
    }

    /// <summary>Guest menu: login or register.</summary>
    private static void ShowGuestMenu()
    {
        Console.WriteLine("=== ONLINE STORE ===");
        Console.WriteLine();
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Browse Products (Guest)");
        Console.WriteLine();
        Console.WriteLine("Esc: Exit");

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                Login();
                break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                Register();
                break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                BrowseProductsAsGuest();
                break;
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
        }
    }

    /// <summary>Logged-in user menu: role-based navigation.</summary>
    private static void ShowUserMenu()
    {
        if (UnitOfWork == null)
        {
            Console.WriteLine("Error: Database context not initialized.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
            return;
        }

        Console.WriteLine($"=== ONLINE STORE ===");
        Console.WriteLine($"Logged in as: {CurrentUser?.Login} ({GetRoleName(CurrentUser?.RoleId ?? 0)})");
        Console.WriteLine();

        if (CurrentUser?.RoleId == 1)
        {
            ShowAdminMenu();
        }
        else
        {
            ShowRegisteredUserMenu();
        }
    }

    private static void ShowAdminMenu()
    {
        Console.WriteLine("1. Manage Users");
        Console.WriteLine("2. Manage Products");
        Console.WriteLine("3. Manage Categories");
        Console.WriteLine("4. Manage Orders");
        Console.WriteLine("5. System Diagnostics");
        Console.WriteLine();
        Console.WriteLine("L: Logout");
        Console.WriteLine("Esc: Exit");

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                new AdminUserController(UnitOfWork!).ShowUserManagement();
                break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                MenuBuilder.Admin.AdminMainMenu.Show(UnitOfWork!);
                break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                new AdminCategoryController(UnitOfWork!).ShowCategories();
                break;
            case ConsoleKey.D4:
            case ConsoleKey.NumPad4:
                new AdminOrderController(UnitOfWork!).Run();
                break;
            case ConsoleKey.D5:
            case ConsoleKey.NumPad5:
                new AdminDiagnosticsController(UnitOfWork!).ShowDiagnostics();
                break;
            case ConsoleKey.L:
                Logout();
                break;
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
        }
    }

    private static void ShowRegisteredUserMenu()
    {
        Console.WriteLine("1. Browse Products");
        Console.WriteLine("2. My Orders");
        Console.WriteLine("3. My Profile");
        Console.WriteLine();
        Console.WriteLine("L: Logout");
        Console.WriteLine("Esc: Exit");

        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                MenuBuilder.User.UserMainMenu.Show(UnitOfWork!);
                break;
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                new UserOrderController(UnitOfWork!).ShowOrderMenu();
                break;
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                new UserController(UnitOfWork!).ShowProfileUpdateMenu();
                break;
            case ConsoleKey.L:
                Logout();
                break;
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
        }
    }

    private static void Login()
    {
        Console.Clear();
        Console.WriteLine("=== LOGIN ===");

        Console.Write("Login: ");
        var login = Console.ReadLine();

        Console.Write("Password: ");
        var password = ReadPassword();
        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Login and password cannot be empty!");
            Pause();
            return;
        }

        if (UnitOfWork == null)
        {
            Console.WriteLine("Error: Database context not initialized.");
            Pause();
            return;
        }

        var userService = new UserService(UnitOfWork);

        var user = userService.Authenticate(login, password);
        if (user != null)
        {
            if (user.IsBlocked)
            {
                Console.WriteLine("Your account is blocked. Contact administrator.");
                Pause();
                return;
            }

            CurrentUser = user;
            Console.WriteLine($"Welcome, {user.Login}!");
            Pause();
        }
        else
        {
            Console.WriteLine("Invalid login or password!");
            Pause();
        }
    }

    private static void Register()
    {
        Console.Clear();
        Console.WriteLine("=== REGISTER ===");

        Console.Write("Login: ");
        var login = Console.ReadLine();

        Console.Write("Password: ");
        var password = ReadPassword();
        Console.WriteLine();

        Console.Write("Confirm Password: ");
        var confirmPassword = ReadPassword();
        Console.WriteLine();

        if (password != confirmPassword)
        {
            Console.WriteLine("Passwords do not match!");
            Pause();
            return;
        }

        Console.Write("First Name: ");
        var firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(login) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("All fields are required!");
            Pause();
            return;
        }

        if (UnitOfWork == null)
        {
            Console.WriteLine("Error: Database context not initialized.");
            Pause();
            return;
        }

        var userService = new UserService(UnitOfWork);

        try
        {
            var newUser = userService.Register(firstName, lastName, login, password);

            if (newUser == null)
            {
                Console.WriteLine("Registration failed. Login might already exist.");
            }
            else
            {
                Console.WriteLine("Registration successful! You can now login.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
        }

        Pause();
    }

    private static void Logout()
    {
        CurrentUser = null;
        Console.Clear();
        Console.WriteLine("You have been logged out.");
        Pause();
    }

    private static void BrowseProductsAsGuest()
    {
        if (UnitOfWork == null)
        {
            Console.WriteLine("Error: Database context not initialized.");
            Pause();
            return;
        }

        MenuBuilder.User.UserMainMenu.Show(UnitOfWork);
    }

    private static string ReadPassword()
    {
        var password = string.Empty;
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        return password;
    }

    private static string GetRoleName(int roleId) => roleId switch
    {
        1 => "Administrator",
        2 => "Registered User",
        _ => "Guest",
    };

    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}
