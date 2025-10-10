// Path: console-online-store/ConsoleApp/Helpers/Db.cs
namespace ConsoleApp.Helpers;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

public static class Db
{
    public static StoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlite("Data Source=store.db")
            .Options;

        var factory = new TestDataFactory();
        return new StoreDbContext(options, factory);
    }
}
