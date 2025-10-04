namespace StoreDAL.Data;

using System;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data.InitDataFactory;
using StoreDAL.Entities;

/// <summary>
/// Entity Framework Core database context for the online store application.
/// Provides access to all database entities and manages database connections, transactions, and change tracking.
/// </summary>
/// <remarks>
/// This context uses SQLite as the database provider and supports initial data seeding through
/// the <see cref="AbstractDataFactory"/> pattern. The context is configured during application
/// startup and manages the following entity sets: Categories, Manufacturers, Products, Users,
/// Orders, and their related entities.
/// </remarks>
public class StoreDbContext : DbContext
{
    private readonly AbstractDataFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreDbContext"/> class.
    /// Default parameterless constructor for design-time operations and migrations.
    /// </summary>
    public StoreDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StoreDbContext"/> class with specified options and data factory.
    /// </summary>
    /// <param name="options">Database context configuration options including connection string and provider settings.</param>
    /// <param name="factory">Data factory for initial database seeding. Provides test or production data sets.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <see langword="null"/>.</exception>
    public StoreDbContext(DbContextOptions options, AbstractDataFactory factory)
        : base(options)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// Gets or sets the database set for product categories.
    /// Categories organize products into logical groups (e.g., fruits, vegetables, drinks).
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// Gets or sets the database set for customer orders.
    /// Represents order headers with user information, order date, and current state.
    /// </summary>
    public DbSet<CustomerOrder> CustomerOrders { get; set; }

    /// <summary>
    /// Gets or sets the database set for product manufacturers.
    /// Manufacturers represent the companies that produce the products.
    /// </summary>
    public DbSet<Manufacturer> Manufacturers { get; set; }

    /// <summary>
    /// Gets or sets the database set for order line items.
    /// Each detail represents a product, quantity, and price within a specific order.
    /// </summary>
    public DbSet<OrderDetail> OrderDetails { get; set; }

    /// <summary>
    /// Gets or sets the database set for order workflow states.
    /// States include: New Order, Confirmed, In Delivery, Delivered, Cancelled, etc.
    /// </summary>
    public DbSet<OrderState> OrderStates { get; set; }

    /// <summary>
    /// Gets or sets the database set for products.
    /// Products represent actual sellable items with pricing, stock quantities, and descriptions.
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// Gets or sets the database set for product titles.
    /// Product titles represent shared product names that can have multiple SKU variants.
    /// </summary>
    public DbSet<ProductTitle> ProductTitles { get; set; }

    /// <summary>
    /// Gets or sets the database set for application users.
    /// Users can have different roles (Admin, Registered, Guest) with varying permissions.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the database set for user roles.
    /// Roles define access levels and permissions within the application.
    /// </summary>
    public DbSet<UserRole> UserRoles { get; set; }

    /// <summary>
    /// Configures the database model and seeds initial data during database creation.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the database model.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="modelBuilder"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method is called by Entity Framework during model creation. It uses the configured
    /// <see cref="AbstractDataFactory"/> to seed initial reference data such as categories,
    /// manufacturers, order states, and user roles. The seeding ensures that the database
    /// has necessary reference data for application operation.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        // Seed reference data in the correct order to respect foreign key constraints
        modelBuilder.Entity<Category>().HasData(this.factory.GetCategoryData());
        modelBuilder.Entity<Manufacturer>().HasData(this.factory.GetManufacturerData());
        modelBuilder.Entity<OrderState>().HasData(this.factory.GetOrderStateData());
        modelBuilder.Entity<UserRole>().HasData(this.factory.GetUserRoleData());
        modelBuilder.Entity<User>().HasData(this.factory.GetUserData());
        modelBuilder.Entity<ProductTitle>().HasData(this.factory.GetProductTitleData());
        modelBuilder.Entity<Product>().HasData(this.factory.GetProductData());
        modelBuilder.Entity<CustomerOrder>().HasData(this.factory.GetCustomerOrderData());
        modelBuilder.Entity<OrderDetail>().HasData(this.factory.GetOrderDetailData());
    }
}
