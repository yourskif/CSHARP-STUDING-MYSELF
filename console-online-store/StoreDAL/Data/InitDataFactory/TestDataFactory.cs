namespace StoreDAL.Data.InitDataFactory;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Idempotent seeder. Adds missing reference data/users/products.
/// Safe for multiple runs (upsert by unique names).
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Seeds all required data for step1: roles, users, order states, and catalog.
    /// </summary>
    /// <param name="db">Database context.</param>
    public static void SeedAll(StoreDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        // If some tables are not created yet (in case of model changes) - EnsureCreated already did its job.

        SeedRoles(db);
        SeedUsers(db);
        SeedOrderStates(db);
        SeedCatalog(db);
    }

    // ---- Roles ----------------------------------------------------------------

    /// <summary>
    /// Seeds user roles: Admin, User, Guest.
    /// </summary>
    private static void SeedRoles(StoreDbContext db)
    {
        EnsureRole(db, 1, "Admin");
        EnsureRole(db, 2, "User");
        EnsureRole(db, 3, "Guest");
        db.SaveChanges();
    }

    /// <summary>
    /// Ensures a role exists with the specified ID and name.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="id">Role ID.</param>
    /// <param name="name">Role name.</param>
    private static void EnsureRole(StoreDbContext db, int id, string name)
    {
        var role = db.UserRoles.FirstOrDefault(r => r.Name == name) ?? db.UserRoles.Find(id);
        if (role is null)
        {
            db.UserRoles.Add(new UserRole { Id = id, Name = name });
        }
        else
        {
            role.Name = name;
        }
    }

    // ---- Users ----------------------------------------------------------------

    /// <summary>
    /// Seeds demo users for testing different roles.
    /// </summary>
    private static void SeedUsers(StoreDbContext db)
    {
        EnsureUser(db, login: "admin", first: "System", last: "Administrator", roleId: 1, passwordPlainOrHash: "admin");
        EnsureUser(db, login: "user", first: "Regular", last: "User", roleId: 2, passwordPlainOrHash: "user");
        db.SaveChanges();
    }

    /// <summary>
    /// Ensures a user exists with the specified credentials.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="login">User login.</param>
    /// <param name="first">First name.</param>
    /// <param name="last">Last name.</param>
    /// <param name="roleId">Role ID.</param>
    /// <param name="passwordPlainOrHash">Password (plain text for legacy compatibility).</param>
    private static void EnsureUser(StoreDbContext db, string login, string first, string last, int roleId, string passwordPlainOrHash)
    {
        var u = db.Users.FirstOrDefault(x => x.Login == login);
        if (u is null)
        {
            db.Users.Add(new User
            {
                Name = first,
                LastName = last,
                Login = login,
                // BLL has fallback for legacy plain-text passwords
                Password = passwordPlainOrHash,
                RoleId = roleId
            });
        }
        else
        {
            // Update name/role if needed; don't touch password
            u.Name = first;
            u.LastName = last;
            u.RoleId = roleId;
        }
    }

    // ---- Order states ----------------------------------------------------------

    /// <summary>
    /// Seeds all 8 order states according to technical requirements.
    /// States match the order state diagram from requirements.
    /// </summary>
    private static void SeedOrderStates(StoreDbContext db)
    {
        // Technical requirements specify exact 8 states with specific names
        EnsureOrderState(db, "New Order");
        EnsureOrderState(db, "Cancelled by user");
        EnsureOrderState(db, "Cancelled by administrator");
        EnsureOrderState(db, "Confirmed");
        EnsureOrderState(db, "Moved to delivery company");
        EnsureOrderState(db, "In delivery");
        EnsureOrderState(db, "Delivered to client");
        EnsureOrderState(db, "Delivery confirmed by client");
        db.SaveChanges();
    }

    /// <summary>
    /// Ensures an order state exists with the specified name.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="name">Order state name.</param>
    private static void EnsureOrderState(StoreDbContext db, string name)
    {
        if (!db.OrderStates.Any(s => s.Name == name))
            db.OrderStates.Add(new OrderState { Name = name });
    }

    // ---- Catalog (Categories/Manufacturers/Titles/Products) -------------------

    /// <summary>
    /// Seeds demo catalog data for testing and demonstration.
    /// </summary>
    private static void SeedCatalog(StoreDbContext db)
    {
        var catPhones = EnsureCategory(db, "Phones");
        var catLaptops = EnsureCategory(db, "Laptops");
        var catAccs = EnsureCategory(db, "Accessories");

        var mContoso = EnsureManufacturer(db, "Contoso");
        var mFabrikam = EnsureManufacturer(db, "Fabrikam");

        var tPhone = EnsureProductTitle(db, "Contoso Phone X", catPhones.Id);
        var tLaptop = EnsureProductTitle(db, "Fabrikam Book 13", catLaptops.Id);

        EnsureProduct(db, titleId: tPhone.Id, manufacturerId: mContoso.Id, desc: "A demo smartphone", price: 499.00m, stock: 25);
        EnsureProduct(db, titleId: tLaptop.Id, manufacturerId: mFabrikam.Id, desc: "A demo ultrabook", price: 999.00m, stock: 10);

        db.SaveChanges();
    }

    /// <summary>
    /// Ensures a category exists with the specified name.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="name">Category name.</param>
    /// <returns>The category entity.</returns>
    private static Category EnsureCategory(StoreDbContext db, string name)
    {
        var c = db.Categories.FirstOrDefault(x => x.Name == name);
        if (c is null)
        {
            c = new Category { Name = name };
            db.Categories.Add(c);
            db.SaveChanges();
        }
        return c;
    }

    /// <summary>
    /// Ensures a manufacturer exists with the specified name.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="name">Manufacturer name.</param>
    /// <returns>The manufacturer entity.</returns>
    private static Manufacturer EnsureManufacturer(StoreDbContext db, string name)
    {
        var m = db.Manufacturers.FirstOrDefault(x => x.Name == name);
        if (m is null)
        {
            m = new Manufacturer { Name = name };
            db.Manufacturers.Add(m);
            db.SaveChanges();
        }
        return m;
    }

    /// <summary>
    /// Ensures a product title exists with the specified title and category.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="title">Product title.</param>
    /// <param name="categoryId">Category ID.</param>
    /// <returns>The product title entity.</returns>
    private static ProductTitle EnsureProductTitle(StoreDbContext db, string title, int categoryId)
    {
        var t = db.ProductTitles.FirstOrDefault(x => x.Title == title);
        if (t is null)
        {
            t = new ProductTitle { Title = title, CategoryId = categoryId };
            db.ProductTitles.Add(t);
            db.SaveChanges();
        }
        else if (t.CategoryId != categoryId)
        {
            t.CategoryId = categoryId;
            db.SaveChanges();
        }
        return t;
    }

    /// <summary>
    /// Ensures a product exists with the specified parameters.
    /// </summary>
    /// <param name="db">Database context.</param>
    /// <param name="titleId">Product title ID.</param>
    /// <param name="manufacturerId">Manufacturer ID.</param>
    /// <param name="desc">Product description.</param>
    /// <param name="price">Unit price.</param>
    /// <param name="stock">Stock quantity.</param>
    private static void EnsureProduct(StoreDbContext db, int titleId, int manufacturerId, string desc, decimal price, int stock)
    {
        // In demo - one item per Title+Manufacturer combination
        var p = db.Products.FirstOrDefault(x => x.TitleId == titleId && x.ManufacturerId == manufacturerId);
        if (p is null)
        {
            db.Products.Add(new Product
            {
                TitleId = titleId,
                ManufacturerId = manufacturerId,
                Description = desc,
                UnitPrice = price,
                Stock = stock
            });
        }
        else
        {
            p.Description = desc;
            p.UnitPrice = price;
            p.Stock = stock;
        }
    }
}
