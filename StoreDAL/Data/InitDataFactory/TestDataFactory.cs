// Path: C:\Users\SK\source\repos\C#\1313\console-online-store\StoreDAL\Data\InitDataFactory\TestDataFactory.cs
namespace StoreDAL.Data.InitDataFactory;

using System;
using StoreDAL.Entities;

public sealed class TestDataFactory : AbstractDataFactory
{
    public override Category[] GetCategoryData() =>
        new[]
        {
            new Category { Id = 1, Name = "fruits" },
            new Category { Id = 2, Name = "water" },
            new Category { Id = 3, Name = "snacks" },
            new Category { Id = 4, Name = "vegetables" },
        };

    public override Manufacturer[] GetManufacturerData() =>
        new[]
        {
            new Manufacturer { Id = 1, Name = "GreenFarm" },
            new Manufacturer { Id = 2, Name = "FreshCo" },
        };

    public override ProductTitle[] GetProductTitleData() =>
        new[]
        {
            new ProductTitle { Id = 1, Title = "Apples Gala", CategoryId = 1 },
            new ProductTitle { Id = 2, Title = "Mineral Water 1L", CategoryId = 2 },
            new ProductTitle { Id = 3, Title = "Potato Chips", CategoryId = 3 },
            new ProductTitle { Id = 4, Title = "Tomatoes Cherry", CategoryId = 4 },
        };

    public override Product[] GetProductData() =>
        new[]
        {
            // Product(id, productTitleId, manufacturerId, description, unitPrice, stockQuantity)
            new Product(
                id: 1,
                productTitleId: 1,
                manufacturerId: 1,
                description: "Fresh apples",
                unitPrice: 2.50m,
                stockQuantity: 120)
            {
                ReservedQuantity = 10,
            },
            new Product(
                id: 2,
                productTitleId: 2,
                manufacturerId: 2,
                description: "Still water",
                unitPrice: 0.80m,
                stockQuantity: 300)
            {
                ReservedQuantity = 0,
            },
            new Product(
                id: 3,
                productTitleId: 3,
                manufacturerId: 2,
                description: "Salted chips",
                unitPrice: 1.40m,
                stockQuantity: 200)
            {
                ReservedQuantity = 15,
            },
            new Product(
                id: 4,
                productTitleId: 4,
                manufacturerId: 1,
                description: "Sweet cherry",
                unitPrice: 3.20m,
                stockQuantity: 150)
            {
                ReservedQuantity = 5,
            },
        };

    public override UserRole[] GetUserRoleData() =>
        new[]
        {
            new UserRole(1, "Admin"),
            new UserRole(2, "Registered"),
            new UserRole(3, "Guest"),
        };

    public override OrderState[] GetOrderStateData() =>
        new[]
        {
            new OrderState(1, "New Order"),
            new OrderState(2, "Canceled by user"),
            new OrderState(3, "Canceled by administrator"),
            new OrderState(4, "Confirmed"),
            new OrderState(5, "Moved to delivery company"),
            new OrderState(6, "In delivery"),
            new OrderState(7, "Delivered to client"),
            new OrderState(8, "Delivery confirmed by client"),
        };

    public override User[] GetUserData() =>
        new[]
        {
            // Passwords are PLAINTEXT here; StoreDbFactory hashes them on seed.
            new User(
                id: 1,
                name: "Admin",
                lastName: "Root",
                login: "admin",
                password: "admin123",
                roleId: 1)
            {
                IsBlocked = false,
            },
            new User(
                id: 2,
                name: "John",
                lastName: "Doe",
                login: "user",
                password: "user123",
                roleId: 2)
            {
                IsBlocked = false,
            },
            new User(
                id: 10,
                name: "Serge",
                lastName: "K",
                login: "sergek",
                password: "sk123",
                roleId: 2)
            {
                IsBlocked = false,
            },
        };

    public override CustomerOrder[] GetCustomerOrderData() => Array.Empty<CustomerOrder>();

    public override OrderDetail[] GetOrderDetailData() => Array.Empty<OrderDetail>();
}
