using System;

using ConsoleApp.Controllers;
using ConsoleApp.Helpers;      // <-- needed for FileLogger
using ConsoleApp.Scenarios;

namespace ConsoleApp
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            FileLogger.Init();
            FileLogger.AttachConsoleTee();

            if (args.Length == 0)
            {
                // Default interactive flow
                UserMenuController.Start();
                return 0;
            }

            var flag = args[0];

            switch (flag)
            {
                case "--auto-admin-crud":
                    FileLogger.Section("=== Admin CRUD Smoke ===");
                    AdminCrudSmokeRunner.Run();
                    return 0;

                case "--auto-admin-crud-orderstates":
                    FileLogger.Section("=== Admin OrderState CRUD Smoke ===");
                    AdminOrderStateCrudSmokeRunner.Run();
                    return 0;

                case "--auto-admin-crud-manufacturers":
                    FileLogger.Section("=== Admin Manufacturer CRUD Smoke ===");
                    AdminManufacturerCrudSmokeRunner.Run();
                    return 0;

                case "--auto-seed":
                    FileLogger.Section("=== Full Seed Runner ===");
                    SeedAllRunner.Run();
                    return 0;

                case "--auto-seed-smoke":
                    FileLogger.Section("=== Seed Smoke Diagnostics ===");
                    SeedAllSmokeRunner.Run();
                    return 0;

                case "--auto-diagnostics":
                    FileLogger.Section("=== Diagnostics Smoke ===");
                    DiagnosticsSmokeRunner.Run();
                    return 0;

                default:
                    PrintHelp();
                    return 1;
            }
        }

        private static void PrintHelp()
        {
            Out("Usage:");
            Out("  dotnet run --project ConsoleApp -- --auto-admin-crud");
            Out("  dotnet run --project ConsoleApp -- --auto-admin-crud-orderstates");
            Out("  dotnet run --project ConsoleApp -- --auto-admin-crud-manufacturers");
            Out("  dotnet run --project ConsoleApp -- --auto-seed");
            Out("  dotnet run --project ConsoleApp -- --auto-seed-smoke");
            Out("  dotnet run --project ConsoleApp -- --auto-diagnostics");
            Out();
            Out("If no arguments are provided → interactive login menu starts.");
        }

        private static void Out(string? message = null) => Console.WriteLine(message ?? string.Empty);
    }
}
