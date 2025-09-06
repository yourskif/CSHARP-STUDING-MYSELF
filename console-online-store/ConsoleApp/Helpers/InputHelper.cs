using System;
using System.Globalization;
using StoreBLL.Models;

namespace ConsoleApp.Helpers
{
    internal static class InputHelper
    {
        public static UserRoleModel ReadUserRoleModel()
        {
            Console.WriteLine("Input User Role Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            Console.WriteLine("Input User Role Name");
            var name = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(name);
            return new UserRoleModel(id, name);
        }

        public static OrderStateModel ReadOrderStateModel()
        {
            Console.WriteLine("Input State Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            Console.WriteLine("Input State Name");
            var name = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(name);
            return new OrderStateModel(id, name);
        }

        public static CategoryModel ReadCategoryModel()
        {
            Console.WriteLine("Input Category Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            Console.WriteLine("Input Category Name");
            var name = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(name);
            return new CategoryModel(id, name);
        }

        public static ManufacturerModel ReadManufacturerModel()
        {
            Console.WriteLine("Input Manufacturer Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            Console.WriteLine("Input Manufacturer Name");
            var name = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(name);
            return new ManufacturerModel(id, name);
        }

        public static ProductTitleModel ReadProductTitleModel()
        {
            Console.WriteLine("Input Product Title Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            Console.WriteLine("Input Product Title");
            var title = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(title);
            Console.WriteLine("Input Category Id");
            var categoryId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
            return new ProductTitleModel(id, title, categoryId);
        }

        public static ProductModel ReadProductModel()
        {
            Console.WriteLine("Input Product Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Product Title Id");
            var titleId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Manufacturer Id");
            var manufacturerId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Unit Price");
            var unitPrice = decimal.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Comment (optional)");
            var comment = Console.ReadLine() ?? string.Empty;

            return new ProductModel(id, titleId, manufacturerId, unitPrice, comment);
        }

        public static UserModel ReadUserModel()
        {
            Console.WriteLine("Input User Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input First Name");
            var firstName = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(firstName);

            Console.WriteLine("Input Last Name");
            var lastName = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(lastName);

            Console.WriteLine("Input Login");
            var login = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(login);

            Console.WriteLine("Input Password");
            var password = Console.ReadLine();
            ArgumentNullException.ThrowIfNull(password);

            Console.WriteLine("Input User Role Id");
            var userRoleId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            return new UserModel(id, firstName, lastName, login, password, userRoleId);
        }

        public static CustomerOrderModel ReadCustomerOrderModel()
        {
            Console.WriteLine("Input Order Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Customer Id");
            var customerId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Operation Time (yyyy-MM-dd HH:mm:ss or press Enter for now)");
            var dateInput = Console.ReadLine();
            DateTime operationTime = string.IsNullOrEmpty(dateInput)
                ? DateTime.Now
                : DateTime.Parse(dateInput, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Order State Id (1-New, 2-Cancelled by user, etc.)");
            var orderStateId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            return new CustomerOrderModel(id, customerId, operationTime, orderStateId);
        }

        public static OrderDetailModel ReadOrderDetailModel()
        {
            Console.WriteLine("Input Order Detail Id");
            var id = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Customer Order Id");
            var customerOrderId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Product Id");
            var productId = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Price");
            var price = decimal.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            Console.WriteLine("Input Product Amount");
            var productAmount = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

            return new OrderDetailModel(id, customerOrderId, productId, price, productAmount);
        }

        // Helper method to safely read integer input
        public static int ReadInt(string prompt, int defaultValue = 0)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            return int.TryParse(input, out var result) ? result : defaultValue;
        }

        // Helper method to safely read decimal input
        public static decimal ReadDecimal(string prompt, decimal defaultValue = 0)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                return defaultValue;

            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        // Helper method to safely read string input
        public static string ReadString(string prompt, bool required = false)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (required && string.IsNullOrEmpty(input))
            {
                Console.WriteLine("This field is required. Please enter a value:");
                return ReadString(prompt, required);
            }

            return input ?? string.Empty;
        }
    }
}