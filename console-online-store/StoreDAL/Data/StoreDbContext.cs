// StoreDAL/Data/StoreDbContext.cs
using System;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Entities;

namespace StoreDAL.Data
{
    /// <summary>
    /// EF Core DbContext for the Online Store domain.
    /// </summary>
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities used in the project.
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductTitle> ProductTitles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<OrderState> OrderStates { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }

        /// <summary>
        /// Model configuration.
        /// CA1062: validate external input parameter.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            // Keep existing configuration if you already have it in partials or elsewhere.
            base.OnModelCreating(modelBuilder);

            // Place fluent configuration here if needed (keys, relationships, constraints, seeding, etc.).
            // Example (commented):
            // modelBuilder.Entity<Category>()
            //     .HasMany(c => c.Products)
            //     .WithOne(p => p.Category)
            //     .HasForeignKey(p => p.CategoryId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
