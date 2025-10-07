using System.IO;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;

namespace ConsoleApp.Helpers
{
    public static class Db
    {
        public static StoreDbContext Create()
        {
            // кладемо SQLite-файл поруч із виконуваним
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "store.db");

            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new StoreDbContext(options);
        }
    }
}
