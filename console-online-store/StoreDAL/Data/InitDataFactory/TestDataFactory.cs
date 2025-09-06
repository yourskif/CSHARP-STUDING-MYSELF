namespace StoreDAL.Data.InitDataFactory;

using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Ідемпотентний сидер. Додає відсутні довідники/користувачів/товари.
/// Безпечний до багаторазового запуску (upsert за унікальними іменами).
/// </summary>
public static class TestDataFactory
{
    public static void SeedAll(StoreDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        // Якщо якісь таблиці ще не створені (на випадок зміни моделей) – EnsureCreated вже зробив своє.

        SeedRoles(db);
        SeedUsers(db);
        SeedOrderStates(db);
        SeedCatalog(db);
    }

    // ---- Roles ----------------------------------------------------------------

    private static void SeedRoles(StoreDbContext db)
    {
        EnsureRole(db, 1, "Admin");
        EnsureRole(db, 2, "User");
        db.SaveChanges();
    }

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

    private static void SeedUsers(StoreDbContext db)
    {
        EnsureUser(db, login: "admin", first: "System", last: "Administrator", roleId: 1, passwordPlainOrHash: "admin");
        EnsureUser(db, login: "user", first: "Regular", last: "User", roleId: 2, passwordPlainOrHash: "user");
        db.SaveChanges();
    }

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
                // у BLL передбачений fallback для legacy plain-text паролів
                Password = passwordPlainOrHash,
                RoleId = roleId
            });
        }
        else
        {
            // оновлюємо ПІБ/роль за потреби; пароль не чіпаємо
            u.Name = first;
            u.LastName = last;
            u.RoleId = roleId;
        }
    }

    // ---- Order states ----------------------------------------------------------

    private static void SeedOrderStates(StoreDbContext db)
    {
        EnsureOrderState(db, "New");
        EnsureOrderState(db, "Processing");
        EnsureOrderState(db, "Shipped");
        EnsureOrderState(db, "Cancelled");
        db.SaveChanges();
    }

    private static void EnsureOrderState(StoreDbContext db, string name)
    {
        if (!db.OrderStates.Any(s => s.Name == name))
            db.OrderStates.Add(new OrderState { Name = name });
    }

    // ---- Catalog (Categories/Manufacturers/Titles/Products) -------------------

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

    private static void EnsureProduct(StoreDbContext db, int titleId, int manufacturerId, string desc, decimal price, int stock)
    {
        // в демо – одна штука на Title+Manufacturer
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
