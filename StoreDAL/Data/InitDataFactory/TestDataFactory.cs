namespace StoreDAL.Data.InitDataFactory;

using System;

using StoreDAL.Entities;

/// <summary>
/// Test data factory for seeding development and testing database.
/// Contains realistic product data with proper pricing and stock levels.
/// </summary>
public class TestDataFactory : AbstractDataFactory
{
    /// <summary>
    /// Gets category seed data with main product categories.
    /// </summary>
    /// <returns>Array of category entities.</returns>
    public override Category[] GetCategoryData() => new[]
    {
        new Category(1, "fruits"),
        new Category(2, "water"),
        new Category(3, "snacks"),
        new Category(4, "vegetables"),
    };

    /// <summary>
    /// Gets manufacturer seed data for product suppliers.
    /// </summary>
    /// <returns>Array of manufacturer entities.</returns>
    public override Manufacturer[] GetManufacturerData() => new[]
    {
        new Manufacturer(1, "GreenFarm"),
        new Manufacturer(2, "FreshCo"),
    };

    /// <summary>
    /// Gets order state data according to technical requirements (8 states).
    /// </summary>
    /// <returns>Array of order state entities.</returns>
    public override OrderState[] GetOrderStateData() => new[]
    {
        new OrderState(1, "New Order"),
        new OrderState(2, "Cancelled by user"),
        new OrderState(3, "Cancelled by administrator"),
        new OrderState(4, "Confirmed"),
        new OrderState(5, "Moved to delivery company"),
        new OrderState(6, "In delivery"),
        new OrderState(7, "Delivered to client"),
        new OrderState(8, "Delivery confirmed by client"),
    };

    /// <summary>
    /// Gets product title data with proper category assignments.
    /// </summary>
    /// <returns>Array of product title entities.</returns>
    public override ProductTitle[] GetProductTitleData() => new[]
    {
        new ProductTitle(1, "Apple Gala 1kg", 1),
        new ProductTitle(2, "Banana 1kg", 1),
        new ProductTitle(3, "Orange 1kg", 1),
        new ProductTitle(4, "Still Water 1.5L", 2),
        new ProductTitle(5, "Sparkling Water 1.5L", 2),
        new ProductTitle(6, "Potato 2kg", 4),
        new ProductTitle(7, "Tomato 1kg", 4),
        new ProductTitle(8, "Chips Classic 150g", 3),
        new ProductTitle(9, "Chips Paprika 150g", 3),
        new ProductTitle(10, "Cucumber 1kg", 4),
        new ProductTitle(11, "Carrot 1kg", 4),
        new ProductTitle(12, "Lemon 1kg", 1),
    };

    /// <summary>
    /// Gets product data with realistic pricing and stock levels.
    /// Now includes stock parameter for proper inventory management.
    /// </summary>
    /// <returns>Array of product entities with prices and stock.</returns>
    public override Product[] GetProductData() => new[]
    {
        new Product(1, 1, 1, "Fresh Gala apples from GreenFarm", 49.90m, 25),
        new Product(2, 2, 1, "Bananas ripe and sweet", 39.50m, 30),
        new Product(3, 3, 2, "Juicy oranges from FreshCo", 59.00m, 20),
        new Product(4, 4, 2, "Still water 1.5L", 19.90m, 50),
        new Product(5, 5, 2, "Sparkling water 1.5L", 21.50m, 45),
        new Product(6, 6, 1, "Potatoes 2kg bag", 34.00m, 15),
        new Product(7, 7, 1, "Tomatoes 1kg", 55.00m, 12),
        new Product(8, 8, 2, "Potato chips classic 150g", 29.90m, 40),
        new Product(9, 9, 2, "Potato chips paprika 150g", 31.90m, 35),
        new Product(10, 10, 1, "Cucumbers 1kg", 44.00m, 18),
        new Product(11, 11, 1, "Carrots 1kg", 24.00m, 22),
        new Product(12, 12, 2, "Lemons 1kg", 69.00m, 8),
    };

    /// <summary>
    /// Gets user role data for access control.
    /// </summary>
    /// <returns>Array of user role entities.</returns>
    public override UserRole[] GetUserRoleData() => new[]
    {
        new UserRole(1, "Admin"),
        new UserRole(2, "User"),
        new UserRole(3, "Guest"),
    };

    /// <summary>
    /// Gets user data with hashed passwords for testing different roles.
    /// </summary>
    /// <returns>Array of user entities.</returns>
    public override User[] GetUserData() => new[]
    {
        // PBKDF2-SHA256 hashed passwords for security
        // admin / Admin@123
        new User(1, "Admin", "Root", "admin",
            "150000.QWRtMW5TYWx0SXNIZXJlIQ==.fQStk19KCmM6k66KKMqvVUPfPf+vtI+UsuzgI6ZIYmg=", 1),
        // user / User@123
        new User(2, "John", "Doe", "user",
            "150000.VXNlclNhbHRJc0hlcmUhIQ==.YLxGcDEypcmzdHtmv/LGqRbjBKPtVAiDBm17tBssKJ4=", 2),
    };

    /// <summary>
    /// Gets customer order data for testing order management functionality.
    /// Constructor: (id, operationTime, userId, orderStateId)
    /// </summary>
    /// <returns>Array of customer order entities.</returns>
    public override CustomerOrder[] GetCustomerOrderData() => new[]
    {
        new CustomerOrder(1, DateTime.UtcNow.AddDays(-2).ToString("u"), 2, 1), // New Order by user John
        new CustomerOrder(2, DateTime.UtcNow.AddDays(-1).ToString("u"), 2, 4), // Confirmed order
    };

    /// <summary>
    /// Gets order detail data for testing order line items.
    /// Constructor: (id, orderId, productId, price, amount)
    /// </summary>
    /// <returns>Array of order detail entities.</returns>
    public override OrderDetail[] GetOrderDetailData() => new[]
    {
        new OrderDetail(1, 1, 1, 49.90m, 1), // Order 1: 1x Gala apples
        new OrderDetail(2, 1, 4, 19.90m, 2), // Order 1: 2x Still water
        new OrderDetail(3, 2, 7, 55.00m, 1), // Order 2: 1x Tomatoes
        new OrderDetail(4, 2, 5, 21.50m, 3), // Order 2: 3x Sparkling water
    };
}
