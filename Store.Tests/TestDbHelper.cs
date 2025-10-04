using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

namespace Store.Tests;

internal static class TestDbHelper
{
    public static (StoreDbContext ctx, Action cleanup) CreateContext()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"store_test_{Guid.NewGuid():N}.db");

        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new StoreDbContext(options, new TestDataFactory());
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();

        void cleanup()
        {
            ctx.Dispose();
            try { File.Delete(dbPath); } catch { }
        }

        return (ctx, cleanup);
    }
}
