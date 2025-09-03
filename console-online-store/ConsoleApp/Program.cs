using System;
using System.Linq;
using ConsoleMenu;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;
using StoreDAL.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ConsoleApp1
{
    public static class Program
    {
        // TODO: todo
        public static void Main(string[] args)
        {
            UserMenuController.Start();
        }
    }
}