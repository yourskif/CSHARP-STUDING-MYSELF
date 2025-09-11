// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Data\StoreDbContext.cs
namespace StoreDAL.Data;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Entities;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
    public DbSet<ProductTitle> ProductTitles { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<CustomerOrder> CustomerOrders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
    public DbSet<OrderState> OrderStates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ProductTitle -> Category (many-to-one)
        modelBuilder.Entity<ProductTitle>()
            .HasOne(pt => pt.Category)
            .WithMany(c => c.Titles)
            .HasForeignKey(pt => pt.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product -> ProductTitle (many-to-one)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Title)
            .WithMany(t => t.Products)
            .HasForeignKey(p => p.TitleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product -> Manufacturer (many-to-one)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Manufacturer)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        // CustomerOrder -> User (many-to-one)
        modelBuilder.Entity<CustomerOrder>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // CustomerOrder -> OrderState (many-to-one)
        modelBuilder.Entity<CustomerOrder>()
            .HasOne(o => o.State)
            .WithMany()
            .HasForeignKey(o => o.OrderStateId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderDetail -> CustomerOrder (many-to-one) with navigation names used in your code
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.CustomerOrder)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderDetail -> Product (many-to-one)
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prices precision (SQLite stores as NUMERIC, precision is still good practice)
        modelBuilder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetail>()
            .Property(od => od.UnitPrice)
            .HasPrecision(18, 2);
    }
}
