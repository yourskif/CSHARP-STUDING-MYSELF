using System;
using System.Globalization;
using StoreBLL.Models;

namespace ConsoleApp.Helpers
{
    internal static class InputHelper
    {
        // ======== БАЗОВІ ЧИТАЧІ ========

        public static int ReadInt(string prompt)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                Console.WriteLine("Invalid integer. Try again.");
                return ReadInt(prompt);
            }

            return value;
        }

        public static string ReadString(string prompt, bool required = false)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (required && string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("This field is required. Please enter a value.");
                return ReadString(prompt, required);
            }

            return input ?? string.Empty;
        }

        // ======== КОНКРЕТНІ МОДЕЛІ ДЛЯ МЕНЮ/КОНТРОЛЕРІВ ========

        // РОЛІ КОРИСТУВАЧА
        public static UserRoleModel ReadUserRoleModel()
        {
            Console.WriteLine("Input User Role Id");
            int id = ReadInt("Id: ");

            Console.WriteLine("Input User Role Name");
            string name = ReadString("Name: ", required: true);

            return new UserRoleModel(id, name);
        }

        // СТАН ЗАМОВЛЕННЯ (для ShopController)
        public static OrderStateModel ReadOrderStateModel()
        {
            Console.WriteLine("Input Order State Id");
            int id = ReadInt("Id: ");

            Console.WriteLine("Input Order State Name");
            string stateName = ReadString("State name: ", required: true);

            return new OrderStateModel(id, stateName);
        }

        // КАТЕГОРІЯ (для ProductController)
        public static CategoryModel ReadCategoryModel()
        {
            Console.WriteLine("Input Category Id");
            int id = ReadInt("Id: ");

            Console.WriteLine("Input Category Name");
            string name = ReadString("Name: ", required: true);

            return new CategoryModel(id, name);
        }

        // ВИРОБНИК (для ProductController)
        public static ManufacturerModel ReadManufacturerModel()
        {
            Console.WriteLine("Input Manufacturer Id");
            int id = ReadInt("Id: ");

            Console.WriteLine("Input Manufacturer Name");
            string name = ReadString("Name: ", required: true);

            return new ManufacturerModel(id, name);
        }

        // НАЗВА ТОВАРУ (для ProductController) — ДОДАНО CategoryId
        public static ProductTitleModel ReadProductTitleModel()
        {
            Console.WriteLine("Input Product Title Id");
            int id = ReadInt("Id: ");

            Console.WriteLine("Input Product Title");
            string title = ReadString("Title: ", required: true);

            Console.WriteLine("Input Category Id for this Product Title");
            int categoryId = ReadInt("CategoryId: ");

            return new ProductTitleModel(id, title, categoryId);
        }
    }
}
