// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Data\StoreDbFactory.cs
namespace StoreDAL.Data;

using System.IO;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data.InitDataFactory;

public static class StoreDbFactory
{
    public static StoreDbContext Create()
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "store.db");

        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new StoreDbContext(options);
        ctx.Database.EnsureCreated();

        DefaultDataFactory.SeedIfEmpty(ctx);
        return ctx;
    }
}
