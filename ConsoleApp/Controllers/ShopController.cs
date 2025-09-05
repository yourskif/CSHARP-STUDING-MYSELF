using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Controllers;
using ConsoleApp.Handlers.ContextMenuHandlers;
using ConsoleApp.Helpers;
using ConsoleApp1;
using ConsoleMenu;
using StoreBLL.Services;
using StoreDAL.Data;

namespace ConsoleApp.Services
{
    public static class ShopController
    {
        private static StoreDbContext context = UserMenuController.Context;

        public static void AddOrder()
        {
            throw new NotImplementedException();
        }

        public static void UpdateOrder()
        {
            throw new NotImplementedException();
        }

        public static void DeleteOrder()
        {
            throw new NotImplementedException();
        }

        public static void ShowOrder()
        {
            throw new NotImplementedException();
        }

        public static void ShowAllOrders()
        {
            throw new NotImplementedException();
        }

        public static void AddOrderDetails()
        {
            throw new NotImplementedException();
        }

        public static void UpdateOrderDetails()
        {
            throw new NotImplementedException();
        }

        public static void DeleteOrderDetails()
        {
            throw new NotImplementedException();
        }

        public static void ShowAllOrderDetails()
        {
            throw new NotImplementedException();
        }

        public static void ProcessOrder()
        {
            throw new NotImplementedException();
        }

        public static void ShowAllOrderStates()
        {
            var service = new OrderStateService(context);
            var menu = new ContextMenu(new AdminContextMenuHandler(service, InputHelper.ReadOrderStateModel), service.GetAll);
            menu.Run();
        }
    }
}
