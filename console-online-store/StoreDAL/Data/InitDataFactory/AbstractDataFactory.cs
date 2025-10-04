namespace StoreDAL.Data.InitDataFactory;

using StoreDAL.Entities;

/// <summary>
/// Abstract base class for data factory implementations that provide initial database seeding data.
/// Implements the Factory Method pattern to allow different data sets (test, production, demo) to be injected into the database context.
/// </summary>
/// <remarks>
/// <para>
/// This abstract factory defines the contract for providing initial data for all database entities.
/// Concrete implementations (e.g., <c>TestDataFactory</c>, <c>ReleaseDataFactory</c>, <c>DefaultDataFactory</c>)
/// provide specific data sets appropriate for different environments or scenarios.
/// </para>
/// <para>
/// The factory is used by <see cref="StoreDbContext"/> during model creation to seed the database
/// with initial reference data and sample records. This approach allows easy switching between
/// different data sets without modifying the context configuration.
/// </para>
/// <para>
/// Typical usage pattern:
/// <code>
/// var factory = new TestDataFactory(); // or ReleaseDataFactory, DefaultDataFactory
/// var context = new StoreDbContext(options, factory);
/// </code>
/// </para>
/// </remarks>
public abstract class AbstractDataFactory
{
    /// <summary>
    /// Gets the initial data set for product categories.
    /// </summary>
    /// <returns>Array of <see cref="Category"/> entities to seed the Categories table.</returns>
    /// <remarks>
    /// Categories organize products into logical groups such as fruits, vegetables, drinks, etc.
    /// This data should be provided before product titles as they depend on category IDs.
    /// </remarks>
    public abstract Category[] GetCategoryData();

    /// <summary>
    /// Gets the initial data set for customer orders.
    /// </summary>
    /// <returns>Array of <see cref="CustomerOrder"/> entities to seed the CustomerOrders table.</returns>
    /// <remarks>
    /// Orders represent purchase transactions made by users. Each order has a state, operation time, and user reference.
    /// This data should be provided after users and order states are seeded.
    /// </remarks>
    public abstract CustomerOrder[] GetCustomerOrderData();

    /// <summary>
    /// Gets the initial data set for product manufacturers.
    /// </summary>
    /// <returns>Array of <see cref="Manufacturer"/> entities to seed the Manufacturers table.</returns>
    /// <remarks>
    /// Manufacturers represent the companies that produce the products sold in the store.
    /// This data should be provided before products as they depend on manufacturer IDs.
    /// </remarks>
    public abstract Manufacturer[] GetManufacturerData();

    /// <summary>
    /// Gets the initial data set for order line items.
    /// </summary>
    /// <returns>Array of <see cref="OrderDetail"/> entities to seed the OrderDetails table.</returns>
    /// <remarks>
    /// Order details represent individual products within an order, including quantity and price at the time of purchase.
    /// This data should be provided last as it depends on both customer orders and products.
    /// </remarks>
    public abstract OrderDetail[] GetOrderDetailData();

    /// <summary>
    /// Gets the initial data set for order workflow states.
    /// </summary>
    /// <returns>Array of <see cref="OrderState"/> entities to seed the OrderStates table.</returns>
    /// <remarks>
    /// Order states define the workflow stages of an order (New, Confirmed, Delivered, Cancelled, etc.).
    /// This reference data is typically consistent across all implementations and should be provided
    /// before customer orders are seeded.
    /// </remarks>
    public abstract OrderState[] GetOrderStateData();

    /// <summary>
    /// Gets the initial data set for products.
    /// </summary>
    /// <returns>Array of <see cref="Product"/> entities to seed the Products table.</returns>
    /// <remarks>
    /// Products represent actual sellable items with pricing, descriptions, stock quantities, and reservations.
    /// This data should be provided after product titles and manufacturers are seeded as products depend on both.
    /// </remarks>
    public abstract Product[] GetProductData();

    /// <summary>
    /// Gets the initial data set for product titles.
    /// </summary>
    /// <returns>Array of <see cref="ProductTitle"/> entities to seed the ProductTitles table.</returns>
    /// <remarks>
    /// Product titles represent shared product names that can be associated with multiple SKU variants.
    /// This data should be provided after categories are seeded as titles reference category IDs.
    /// </remarks>
    public abstract ProductTitle[] GetProductTitleData();

    /// <summary>
    /// Gets the initial data set for application users.
    /// </summary>
    /// <returns>Array of <see cref="User"/> entities to seed the Users table.</returns>
    /// <remarks>
    /// <para>
    /// Users represent individuals who can access the system with different permission levels.
    /// This data should be provided after user roles are seeded as users reference role IDs.
    /// </para>
    /// <para>
    /// Important: Passwords in the returned data should be properly hashed using the application's
    /// password hashing mechanism before seeding. Never store plain-text passwords in the database.
    /// </para>
    /// </remarks>
    public abstract User[] GetUserData();

    /// <summary>
    /// Gets the initial data set for user roles.
    /// </summary>
    /// <returns>Array of <see cref="UserRole"/> entities to seed the UserRoles table.</returns>
    /// <remarks>
    /// User roles define access levels and permissions (Admin, Registered, Guest).
    /// This reference data is typically consistent across implementations and should be provided
    /// before users are seeded.
    /// </remarks>
    public abstract UserRole[] GetUserRoleData();
}
