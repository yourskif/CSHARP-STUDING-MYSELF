namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;
using StoreBLL.Security;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

public class UserService : ICrud
{
    private readonly StoreDbContext context;
    private readonly IUserRepository users;

    public UserService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.users = new UserRepository(context);
    }

    /// <summary>
    /// Authenticate by login + password.
    /// 1) Primary: PBKDF2 ("PBKDF2$...").
    /// 2) Fallback (legacy/demo seeds): plain-text match if stored value is not in PBKDF2 format.
    /// </summary>
    public UserModel? Authenticate(string login, string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(plainPassword))
            return null;

        var normalizedLogin = login.Trim();
        var u = this.users.FindByLogin(normalizedLogin);
        if (u == null) return null;

        var authenticated = false;

        // Try PBKDF2 first
        try
        {
            if (!string.IsNullOrWhiteSpace(u.Password) &&
                u.Password.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                authenticated = PasswordHasher.VerifyPassword(plainPassword, u.Password);
            }
        }
        catch
        {
            authenticated = false; // ignore and try legacy
        }

        // Legacy fallback: for old seed data not using PBKDF2
        if (!authenticated &&
            !string.IsNullOrWhiteSpace(u.Password) &&
            !u.Password.StartsWith("PBKDF2$", StringComparison.Ordinal))
        {
            authenticated = string.Equals(plainPassword, u.Password, StringComparison.Ordinal);
        }

        if (!authenticated) return null;

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

    /// <summary>
    /// Register new user (default role: User = 2). Returns created model or null if login exists.
    /// </summary>
    public UserModel? Register(string firstName, string lastName, string login, string plainPassword)
    {
        // 1) Normalize
        firstName ??= string.Empty;
        lastName ??= string.Empty;
        var normalizedLogin = (login ?? string.Empty).Trim();
        var normalizedPassword = plainPassword ?? string.Empty;

        // 2) Validate
        if (normalizedLogin.Length < 3)
            throw new ArgumentException("Логін має містити щонайменше 3 символи.", nameof(login));
        if (normalizedPassword.Length < 6)
            throw new ArgumentException("Пароль має бути не коротшим за 6 символів.", nameof(plainPassword));

        // 3) Uniqueness
        var existing = this.users.FindByLogin(normalizedLogin);
        if (existing != null)
            return null; // login already taken

        // 4) Role: за замовчуванням 'User' = 2 (узгоджено з сидингом/ТЗ)
        const int userRoleId = 2;

        // 5) Hash password (PBKDF2)
        var hash = PasswordHasher.HashPassword(normalizedPassword);

        // 6) Create entity
        var entity = new User
        {
            Name = firstName.Trim(),
            LastName = lastName.Trim(),
            Login = normalizedLogin,
            Password = hash,
            RoleId = userRoleId,
        };

        // 7) Persist
        this.users.Add(entity);
        this.users.SaveChanges();

        // 8) Map to BLL model
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

    public AbstractModel? GetById(int id)
    {
        var u = this.users.GetById(id);
        if (u == null) return null;

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
            throw new ArgumentException("Expected UserModel.", nameof(model));

        // Реєстрація через Add також повинна зберігати хеш, а не plain-text.
        if (string.IsNullOrWhiteSpace(m.Password))
            throw new ArgumentException("Password cannot be empty for Add.", nameof(model));

        var entity = new User
        {
            Name = (m.FirstName ?? string.Empty).Trim(),
            LastName = (m.LastName ?? string.Empty).Trim(),
            Login = (m.Login ?? string.Empty).Trim(),
            Password = PasswordHasher.HashPassword(m.Password),
            RoleId = m.RoleId == 0 ? 2 : m.RoleId, // 2 = User за замовчуванням
        };

        this.users.Add(entity);
        this.users.SaveChanges();
        m.Id = entity.Id;
        m.Password = string.Empty; // не тримаємо пароль у моделі
    }

    public void Update(AbstractModel model)
    {
        if (model is not UserModel m)
            throw new ArgumentException("Expected UserModel.", nameof(model));

        var entity = this.users.GetById(m.Id);
        if (entity == null) return;

        if (!string.IsNullOrWhiteSpace(m.FirstName))
            entity.Name = m.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(m.LastName))
            entity.LastName = m.LastName.Trim();
        if (!string.IsNullOrWhiteSpace(m.Login))
            entity.Login = m.Login.Trim();

        if (!string.IsNullOrWhiteSpace(m.Password))
            entity.Password = PasswordHasher.HashPassword(m.Password);

        // За потреби: оновлення ролі лише якщо > 0
        if (m.RoleId > 0)
            entity.RoleId = m.RoleId;

        this.users.SaveChanges();
        m.Password = string.Empty; // не зберігаємо пароль у модель після оновлення
    }

    public void Delete(int modelId)
    {
        var entity = this.users.GetById(modelId);
        if (entity == null) return;

        this.context.Users.Remove(entity);
        this.context.SaveChanges();
    }
}
