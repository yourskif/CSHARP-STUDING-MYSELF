namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;
using StoreBLL.Security;          // PasswordHasher

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

public class UserService : ICrud
{
    private readonly StoreDbContext context;
    private readonly IUserRepository users;
    private readonly IUserRoleRepository roles;

    public UserService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.users = new UserRepository(context);
        this.roles = new UserRoleRepository(context);
    }

    /// <summary>
    /// Реєстрація нового користувача з роллю User (RoleId = 2).
    /// Повертає створену модель або null, якщо логін уже існує.
    /// </summary>
    public UserModel? Register(string firstName, string lastName, string login, string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Логін не може бути порожнім.", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("Пароль не може бути порожнім.", nameof(plainPassword));
        }

        // 1) Перевірка унікальності логіна
        var existing = this.users.FindByLogin(login);
        if (existing != null)
        {
            return null; // користувач із таким логіном уже є
        }

        // 2) Фіксована роль "User" (згідно сидингу RoleId=2)
        const int userRoleId = 2;

        // 3) Хешуємо пароль (PBKDF2)
        string hash = PasswordHasher.HashPassword(plainPassword);

        // 4) Створюємо сутність DAL
        var entity = new User
        {
            Name = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Login = login,
            Password = hash,
            RoleId = userRoleId,
        };

        this.users.Add(entity);
        this.users.SaveChanges();

        // 5) Віддаємо BLL-модель (пароль не повертаємо)
        return new UserModel
        {
            Id = entity.Id,
            FirstName = entity.Name,
            LastName = entity.LastName,
            Login = entity.Login,
            Password = string.Empty,
            RoleId = entity.RoleId,
        };
    }

    // ---------------- ICrud ----------------

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.users
            .GetAll()
            .Select(u => new UserModel
            {
                Id = u.Id,
                FirstName = u.Name,
                LastName = u.LastName,
                Login = u.Login,
                Password = string.Empty,
                RoleId = u.RoleId,
            });
    }

    public AbstractModel GetById(int id)
    {
        var u = this.users.GetById(id);
        if (u == null)
        {
            // Повертаємо null — споживачі вже очікують таку поведінку (runtime null, без зміни інтерфейсу)
            return null!;
        }

        return new UserModel
        {
            Id = u.Id,
            FirstName = u.Name,
            LastName = u.LastName,
            Login = u.Login,
            Password = string.Empty,
            RoleId = u.RoleId,
        };
    }

    public void Add(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Очікується UserModel.", nameof(model));
        }

        string hash = string.IsNullOrEmpty(m.Password)
            ? string.Empty
            : PasswordHasher.HashPassword(m.Password);

        var entity = new User
        {
            Name = m.FirstName ?? string.Empty,
            LastName = m.LastName ?? string.Empty,
            Login = m.Login ?? string.Empty,
            Password = hash,
            RoleId = m.RoleId == 0 ? 2 : m.RoleId, // за замовчуванням користувач
        };

        this.users.Add(entity);
        this.users.SaveChanges();
        m.Id = entity.Id;
    }

    public void Update(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Очікується UserModel.", nameof(model));
        }

        var entity = this.users.GetById(m.Id);
        if (entity == null)
        {
            return;
        }

        entity.Name = m.FirstName ?? entity.Name;
        entity.LastName = m.LastName ?? entity.LastName;
        entity.Login = m.Login ?? entity.Login;

        if (!string.IsNullOrWhiteSpace(m.Password))
        {
            entity.Password = PasswordHasher.HashPassword(m.Password);
        }

        this.users.SaveChanges();
    }

    public void Delete(int modelId)
    {
        var entity = this.users.GetById(modelId);
        if (entity == null)
        {
            return;
        }

        // IUserRepository не має Delete, тому видаляємо напряму через контекст
        this.context.Users.Remove(entity);
        this.context.SaveChanges();
    }
}
