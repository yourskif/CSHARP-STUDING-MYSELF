using System;
using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Data.InitDataFactory;

namespace ConsoleApp.Helpers
{
    /// <summary>
    /// Creates configured <see cref="StoreDbContext"/> and ensures the SQLite DB exists
    /// in the solution root (next to the .sln). Also runs lightweight seeding.
    /// </summary>
    public static class StoreDbFactory
    {
        /// <summary>Create configured context.</summary>
        public static StoreDbContext Create(string? databaseFile = null)
        {
            return CreateDbContext(databaseFile);
        }

        /// <summary>Actual factory method used by ConsoleApp.</summary>
        public static StoreDbContext CreateDbContext(string? databaseFile = null)
        {
            // 1) Find solution root (nearest parent containing a *.sln)
            string baseDir = AppContext.BaseDirectory;
            string root = FindSolutionRoot(baseDir) ?? baseDir;

            // 2) DB path must be in the solution root
            string dbPath = databaseFile ?? Path.Combine(root, "store.db");

            // 3) Configure DbContext (SQLite)
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .EnableSensitiveDataLogging()
                .Options;

            // 4) Create, ensure DB, seed
            var db = new StoreDbContext(options);
            db.Database.EnsureCreated();

            // ✅ call your SeedAll
            TestDataFactory.SeedAll(db);

            return db;
        }

        /// <summary>
        /// Walks up from a starting folder until a directory with a *.sln file is found.
        /// Returns null if not found.
        /// </summary>
        private static string? FindSolutionRoot(string start)
        {
            var dir = new DirectoryInfo(start);
            while (dir != null)
            {
                if (dir.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
                    return dir.FullName;

                dir = dir.Parent;
            }
            return null;
        }
    }
}
