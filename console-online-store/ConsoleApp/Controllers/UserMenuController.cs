// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\ConsoleApp\Controllers\UserMenuController.cs
namespace ConsoleApp.Controllers;

using System;
using System.Data.Common;
using ConsoleApp.MenuBuilder.Admin;
using ConsoleApp.MenuBuilder.Guest;
using ConsoleApp.MenuBuilder.User;
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;

// Factory with hashing + EnsureDefaultAdmin
using AppStoreDbFactory = ConsoleApp.Helpers.StoreDbFactory;

/// <summary>
/// Main menu controller for handling user authentication and navigation.
/// </summary>
public static class UserMenuController
{
    /// <summary>Gets the global DB context for controllers.</summary>
    public static StoreDbContext Context { get; private set; } = null!;

    /// <summary>Gets the currently logged-in user.</summary>
    public static UserModel? CurrentUser { get; private set; }

    /// <summary>Sets current user (or null to logout).</summary>
    public static void SetCurrentUser(UserModel? user) => CurrentUser = user;

    /// <summary>App entry: main menu loop.</summary>
    public static void Start()
    {
        Context = AppStoreDbFactory.Create();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== ONLINE STORE =====");
            Console.WriteLine("1. Admin Login");
            Console.WriteLine("2. User Login");
            Console.WriteLine("3. Guest");
            Console.WriteLine("-------------------------");
            Console.WriteLine("Esc: Exit");

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    {
                        if (LoginAsAdmin())
                        {
                            AdminMainMenu.Show(Context);
                        }

                        break;
                    }

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    {
                        if (LoginAsUser())
                        {
                            UserMainMenu.Show(Context);
                        }

                        break;
                    }

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    {
                        GuestMainMenu.Show(Context);
                        break;
                    }

                case ConsoleKey.Escape:
                    {
                        Context.Dispose();
                        return;
                    }
            }
        }
    }

    private static bool LoginAsAdmin()
    {
        Console.Clear();
        Console.WriteLine("=== Admin Login ===");
        Console.Write("Login: ");
        string login = Console.ReadLine() ?? string.Empty;

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? string.Empty;

        try
        {
            var userService = new UserService(Context);
            var user = userService.Authenticate(login, password);

            if (user != null && user.RoleId == 1)
            {
                SetCurrentUser(user);
                Console.WriteLine($"Welcome, Admin {user.FirstName ?? user.Login}!");
                Pause();
                return true;
            }

            Console.WriteLine("Invalid admin credentials or insufficient privileges.");
            Pause();
            return false;
        }
        catch (DbException ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            Pause();
            return false;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Operation error: {ex.Message}");
            Pause();
            return false;
        }
    }

    private static bool LoginAsUser()
    {
        Console.Clear();
        Console.WriteLine("=== User Login ===");
        Console.Write("Login: ");
        string login = Console.ReadLine() ?? string.Empty;

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? string.Empty;

        try
        {
            var userService = new UserService(Context);
            var user = userService.Authenticate(login, password);

            if (user != null && user.RoleId == 2)
            {
                SetCurrentUser(user);
                Console.WriteLine($"Welcome, {user.FirstName ?? user.Login}!");
                Pause();
                return true;
            }

            Console.WriteLine("Invalid user credentials.");
            Pause();
            return false;
        }
        catch (DbException ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            Pause();
            return false;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Operation error: {ex.Message}");
            Pause();
            return false;
        }
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
