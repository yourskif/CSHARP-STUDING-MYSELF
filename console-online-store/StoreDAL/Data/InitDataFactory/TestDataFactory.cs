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

    public override CustomerOrder[] GetCustomerOrderData() =>
        new[]
        {
            // Order #1: New Order (State 1) by user John - reserved 10 apples
            new CustomerOrder(
                id: 1,
                operationTime: "2025-01-15 10:30:00",
                userId: 2,
                orderStateId: 1),

            // Order #2: Confirmed (State 4) by user Serge - reserved 15 chips
            new CustomerOrder(
                id: 2,
                operationTime: "2025-01-14 14:20:00",
                userId: 10,
                orderStateId: 4),

            // Order #3: In Delivery (State 6) by user John - reserved 5 tomatoes
            new CustomerOrder(
                id: 3,
                operationTime: "2025-01-13 09:15:00",
                userId: 2,
                orderStateId: 6),

            // Order #4: Delivered (State 7) by user Serge - ready to confirm
            new CustomerOrder(
                id: 4,
                operationTime: "2025-01-12 16:45:00",
                userId: 10,
                orderStateId: 7),
        };

    public override OrderDetail[] GetOrderDetailData() =>
        new[]
        {
            // Order #1 details: 10 apples
            new OrderDetail(
                id: 1,
                orderId: 1,
                productId: 1,
                productAmount: 10,
                price: 2.50m),

            // Order #2 details: 15 chips
            new OrderDetail(
                id: 2,
                orderId: 2,
                productId: 3,
                productAmount: 15,
                price: 1.40m),

            // Order #3 details: 5 tomatoes
            new OrderDetail(
                id: 3,
                orderId: 3,
                productId: 4,
                productAmount: 5,
                price: 3.20m),

            // Order #4 details: 20 water bottles + 10 chips
            new OrderDetail(
                id: 4,
                orderId: 4,
                productId: 2,
                productAmount: 20,
                price: 0.80m),
            new OrderDetail(
                id: 5,
                orderId: 4,
                productId: 3,
                productAmount: 10,
                price: 1.40m),
        };
}
