using System;
using System.Linq;
using ConsoleApp.Controllers; // for UserMenuController.CurrentUser
using StoreBLL.Models;
using StoreBLL.Services;
using StoreDAL.Data;

namespace ConsoleApp.MenuBuilder.Admin
{
    /// <summary>
    /// Console menu for user management by administrator (list/block/unblock/delete/edit).
    /// Adds confirmations and self-protection (no self-block/self-delete).
    /// </summary>
    public static class AdminUsersMenu
    {
        public static void Show(StoreDbContext db)
        {
            var service = new UserService(db);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Admin: Users Management ===");
                Console.WriteLine("1) List users");
                Console.WriteLine("2) Block user by Id");
                Console.WriteLine("3) Unblock user by Id");
                Console.WriteLine("4) Delete user by Id");
                Console.WriteLine("5) Edit user profile (by Id)");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Esc) Back");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        {
                            Console.Clear();

                            var list = service
                                .GetAll()
                                .OfType<UserModel>()
                                .OrderBy(u => u.Id)
                                .ToList();

                            if (list.Count == 0)
                            {
                                Console.WriteLine("No users found.");
                            }
                            else
                            {
                                Console.WriteLine("Id  | Login         | Name                 | Role | Blocked");
                                Console.WriteLine("----+---------------+----------------------+------|--------");
                                foreach (var u in list)
                                {
                                    var fullName = $"{u.FirstName} {u.LastName}".Trim();
                                    Console.WriteLine($"{u.Id,-3} | {u.Login,-13} | {fullName,-20} | {u.RoleId,4} | {(u.IsBlocked ? "YES" : "NO")}");
                                }
                            }

                            Pause();
                            break;
                        }

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        {
                            Console.Write("Enter user Id to BLOCK: ");
                            if (int.TryParse(Console.ReadLine(), out var idToBlock))
                            {
                                // self-protection
                                if (UserMenuController.CurrentUser?.Id == idToBlock)
                                {
                                    Console.WriteLine("You cannot block yourself.");
                                    Pause();
                                    break;
                                }

                                var ok = service.BlockUser(idToBlock);
                                Console.WriteLine(ok ? "User blocked." : "User not found.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid Id.");
                            }

                            Pause();
                            break;
                        }

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        {
                            Console.Write("Enter user Id to UNBLOCK: ");
                            if (int.TryParse(Console.ReadLine(), out var idToUnblock))
                            {
                                var ok = service.UnblockUser(idToUnblock);
                                Console.WriteLine(ok ? "User unblocked." : "User not found.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid Id.");
                            }

                            Pause();
                            break;
                        }

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        {
                            Console.Write("Enter user Id to DELETE: ");
                            if (int.TryParse(Console.ReadLine(), out var idToDelete))
                            {
                                // self-protection
                                if (UserMenuController.CurrentUser?.Id == idToDelete)
                                {
                                    Console.WriteLine("You cannot delete your own account.");
                                    Pause();
                                    break;
                                }

                                var user = service.GetById(idToDelete) as UserModel;
                                if (user == null)
                                {
                                    Console.WriteLine("User not found.");
                                    Pause();
                                    break;
                                }

                                if (!ConfirmYN($"Are you sure you want to delete '{user.Login}' (ID={user.Id})"))
                                {
                                    Console.WriteLine("Deletion cancelled.");
                                    Pause();
                                    break;
                                }

                                try
                                {
                                    service.Delete(idToDelete);
                                    Console.WriteLine("User deleted.");
                                }
                                catch (InvalidOperationException ex)
                                {
                                    Console.WriteLine($"Cannot delete user: {ex.Message}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid Id.");
                            }

                            Pause();
                            break;
                        }

                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        {
                            Console.Write("Enter user Id to EDIT: ");
                            if (!int.TryParse(Console.ReadLine(), out var idToEdit))
                            {
                                Console.WriteLine("Invalid Id.");
                                Pause();
                                break;
                            }

                            var current = service.GetById(idToEdit) as UserModel;
                            if (current == null)
                            {
                                Console.WriteLine("User not found.");
                                Pause();
                                break;
                            }

                            Console.WriteLine($"\nCurrent First Name: {current.FirstName}");
                            Console.Write("New First Name (Enter to keep): ");
                            var first = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(first))
                            {
                                first = current.FirstName;
                            }

                            Console.WriteLine($"Current Last Name:  {current.LastName}");
                            Console.Write("New Last Name (Enter to keep): ");
                            var last = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(last))
                            {
                                last = current.LastName;
                            }

                            Console.WriteLine($"Current Login:      {current.Login}");
                            Console.Write("New Login (Enter to keep): ");
                            var login = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(login))
                            {
                                login = current.Login;
                            }

                            Console.WriteLine($"Current RoleId:     {current.RoleId} (1=admin, 2=user)");
                            Console.Write("New RoleId (Enter to keep): ");
                            var roleStr = Console.ReadLine();
                            int roleId = current.RoleId;
                            if (!string.IsNullOrWhiteSpace(roleStr) && int.TryParse(roleStr, out var parsedRole))
                            {
                                roleId = parsedRole;
                            }

                            // self-protection: do not allow demote last admin (Р В Р’В»Р В РЎвЂўР В РЎвЂ“Р РЋРІР‚вЂњР В РЎвЂќР В Р’В° Р В Р вЂ  Р РЋР С“Р В Р’ВµР РЋР вЂљР В Р вЂ Р РЋРІР‚вЂњР РЋР С“Р РЋРІР‚вЂњ),
                            // Р В Р’В°Р В Р’В»Р В Р’Вµ Р В РўвЂР В РЎвЂўР В РўвЂР В Р’В°Р РЋРІР‚С™Р В РЎвЂќР В РЎвЂўР В Р вЂ Р В РЎвЂў Р В Р вЂ¦Р В Р’Вµ Р В РўвЂР В РЎвЂўР В Р’В·Р В Р вЂ Р В РЎвЂўР В Р’В»Р РЋР РЏР РЋРІР‚СњР В РЎВР В РЎвЂў Р РЋР С“Р В РЎвЂўР В Р’В±Р РЋРІР‚вЂњ Р В Р’В·Р В Р вЂ¦Р РЋР РЏР РЋРІР‚С™Р В РЎвЂ Р В Р’В°Р В РўвЂР В РЎВР РЋРІР‚вЂњР В Р вЂ¦Р В РЎвЂќР РЋРЎвЂњ, Р РЋР РЏР В РЎвЂќР РЋРІР‚В°Р В РЎвЂў Р РЋРІР‚В Р В Р’Вµ Р В РЎвЂўР РЋР С“Р РЋРІР‚С™Р В Р’В°Р В Р вЂ¦Р В Р вЂ¦Р РЋРІР‚вЂњР В РІвЂћвЂ“ Р В Р’В°Р В РўвЂР В РЎВР РЋРІР‚вЂњР В Р вЂ¦
                            var input = new UserModel
                            {
                                Id = current.Id,
                                FirstName = first,
                                LastName = last,
                                Login = login,
                                RoleId = roleId,
                                IsBlocked = current.IsBlocked,
                            };

                            if (service.UpdateByAdmin(input, out var error))
                            {
                                Console.WriteLine("User profile updated.");
                            }
                            else
                            {
                                Console.WriteLine($"Update failed: {error}");
                            }

                            Pause();
                            break;
                        }

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static bool ConfirmYN(string prompt)
        {
            Console.Write($"{prompt} (Y/N): ");
            var s = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();
            return s is "y" or "yes";
        }
    }
}
