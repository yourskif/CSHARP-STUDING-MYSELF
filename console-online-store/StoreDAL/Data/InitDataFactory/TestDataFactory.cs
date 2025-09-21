// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Data\InitDataFactory\TestDataFactory.cs

namespace StoreDAL.Data.InitDataFactory;
using System;
using StoreDAL.Entities;

/// <summary>
/// Test data factory for seeding development and testing database.
/// Contains realistic product data with proper pricing.
/// </summary>
public class TestDataFactory : AbstractDataFactory
{
    public override Category[] GetCategoryData() => new[]
    {
        new Category(1, "fruits"),
        new Category(2, "water"),
        new Category(3, "snacks"),
        new Category(4, "vegetables"),
    };

    public override Manufacturer[] GetManufacturerData() => new[]
    {
        new Manufacturer(1, "GreenFarm"),
        new Manufacturer(2, "FreshCo"),
    };

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

    public override Product[] GetProductData() => new[]
    {
        new Product(1, 1, 1, "Fresh Gala apples from GreenFarm", 49.90m),
        new Product(2, 2, 1, "Bananas ripe and sweet", 39.50m),
        new Product(3, 3, 2, "Juicy oranges from FreshCo", 59.00m),
        new Product(4, 4, 2, "Still water 1.5L", 19.90m),
        new Product(5, 5, 2, "Sparkling water 1.5L", 21.50m),
        new Product(6, 6, 1, "Potatoes 2kg bag", 34.00m),
        new Product(7, 7, 1, "Tomatoes 1kg", 55.00m),
        new Product(8, 8, 2, "Potato chips classic 150g", 29.90m),
        new Product(9, 9, 2, "Potato chips paprika 150g", 31.90m),
        new Product(10, 10, 1, "Cucumbers 1kg", 44.00m),
        new Product(11, 11, 1, "Carrots 1kg", 24.00m),
        new Product(12, 12, 2, "Lemons 1kg", 69.00m),
    };

    public override UserRole[] GetUserRoleData() => new[]
    {
        new UserRole(1, "Admin"),
        new UserRole(2, "User"),
        new UserRole(3, "Guest"),
    };

    public override User[] GetUserData() => new[]
    {
        // admin / admin123
        new User(
            1,
            "Admin",
            "Root",
            "admin",
            "PBKDF2$100000$X7sU4YPr7zS2zKwF5yR+bg==$jm4q1pJPl7+C6cBkp8hqnKgGvjRHPYgvQcXYzW1rsRY=",
            1),
        // user / user123
        new User(
            2,
            "John",
            "Doe",
            "user",
            "PBKDF2$100000$KQ9aB7qCzR3tF4yL2wM+xQ==$vN8P5YRm9+D2dDhL4gjnmLhFwkRGOXhwSdYZsT0prTU=",
            2),
    };

    public override CustomerOrder[] GetCustomerOrderData() => new[]
    {
        new CustomerOrder(1, DateTime.UtcNow.AddDays(-2).ToString("u"), 2, 1),
        new CustomerOrder(2, DateTime.UtcNow.AddDays(-1).ToString("u"), 2, 4),
    };

    public override OrderDetail[] GetOrderDetailData() => new[]
    {
        new OrderDetail(1, 1, 1, 49.90m, 1),
        new OrderDetail(2, 1, 4, 19.90m, 2),
        new OrderDetail(3, 2, 7, 55.00m, 1),
        new OrderDetail(4, 2, 5, 21.50m, 3),
    };
}