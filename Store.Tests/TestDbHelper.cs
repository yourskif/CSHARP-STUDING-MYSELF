using System;
using System.IO;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;
using StoreDAL.UnitOfWork;

namespace Store.Tests;

/// <summary>
/// Helper class for creating test database contexts and unit of work instances.
/// Provides isolated test databases using SQLite in-memory or temporary files.
/// </summary>
internal static class TestDbHelper
{
    /// <summary>
    /// Creates a test database context with isolated temporary SQLite database.
    /// Legacy method - prefer CreateUnitOfWork for new tests.
    /// </summary>
    /// <returns>Database context and cleanup action.</returns>
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
            try { File.Delete(dbPath); }
            catch { }
        }

        return (ctx, cleanup);
    }

    /// <summary>
    /// Creates a test Unit of Work with isolated temporary SQLite database.
    /// Recommended method for all new tests using UnitOfWork pattern.
    /// </summary>
    /// <returns>Unit of Work instance and cleanup action.</returns>
    public static (IStoreUnitOfWork unitOfWork, Action cleanup) CreateUnitOfWork()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"store_test_{Guid.NewGuid():N}.db");
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .EnableSensitiveDataLogging()
            .Options;
        var ctx = new StoreDbContext(options, new TestDataFactory());
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();

        var unitOfWork = new StoreUnitOfWork(ctx);

        void cleanup()
        {
            unitOfWork.Dispose();
            try { File.Delete(dbPath); }
            catch { }
        }

        return (unitOfWork, cleanup);
    }
}
