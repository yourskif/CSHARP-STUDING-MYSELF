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

/// <summary>
/// Business logic service for user management operations.
/// Handles user registration, authentication, and CRUD operations.
/// </summary>
public class UserService : ICrud
{
    private readonly StoreDbContext context;
    private readonly IUserRepository users;
    private readonly IUserRoleRepository roles;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public UserService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.users = new UserRepository(context);
        this.roles = new UserRoleRepository(context);
    }

    /// <summary>
    /// Registers a new user with User role (RoleId = 2).
    /// Returns created model or null if login already exists.
    /// </summary>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="login">Unique login.</param>
    /// <param name="plainPassword">Plain text password.</param>
    /// <returns>Created UserModel or null if login exists.</returns>
    public UserModel? Register(string firstName, string lastName, string login, string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cannot be empty.", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));
        }

        // 1) Check login uniqueness
        var existing = this.users.FindByLogin(login);
        if (existing != null)
        {
            return null; // user with this login already exists
        }

        // 2) Fixed role "User" (according to seeding RoleId=2)
        const int userRoleId = 2;

        // 3) Hash password (PBKDF2)
        string hash = PasswordHasher.HashPassword(plainPassword);

        // 4) Create DAL entity
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

        // 5) Return BLL model (password not returned)
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

    /// <summary>
    /// Authenticates user by login and password.
    /// Returns UserModel if credentials are valid, or null if invalid.
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="password">Plain text password.</param>
    /// <returns>UserModel if authenticated successfully, null otherwise.</returns>
    public UserModel? Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = this.users.FindByLogin(login);
        if (user == null)
        {
            return null; // user not found
        }

        // Verify password through PasswordHasher
        bool isValid = PasswordHasher.VerifyPassword(password, user.Password);
        if (!isValid)
        {
            return null; // incorrect password
        }

        // Return model without password
        return new UserModel
        {
            Id = user.Id,
            FirstName = user.Name,
            LastName = user.LastName,
            Login = user.Login,
            Password = string.Empty,
            RoleId = user.RoleId,
        };
    }

    // ---------------- ICrud Implementation ----------------

    /// <summary>
    /// Gets all users as AbstractModel collection.
    /// </summary>
    /// <returns>Collection of UserModel instances.</returns>
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

    /// <summary>
    /// Gets user by identifier.
    /// </summary>
    /// <param name="id">User identifier.</param>
    /// <returns>UserModel or null if not found.</returns>
    public AbstractModel GetById(int id)
    {
        var u = this.users.GetById(id);
        if (u == null)
        {
            // Return null — consumers already expect this behavior (runtime null, without interface change)
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

    /// <summary>
    /// Adds new user from model.
    /// </summary>
    /// <param name="model">UserModel to add.</param>
    public void Add(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Expected UserModel.", nameof(model));
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
            RoleId = m.RoleId == 0 ? 2 : m.RoleId, // default user role
        };

        this.users.Add(entity);
        this.users.SaveChanges();
        m.Id = entity.Id;
    }

    /// <summary>
    /// Updates existing user from model.
    /// </summary>
    /// <param name="model">UserModel with updated data.</param>
    public void Update(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Expected UserModel.", nameof(model));
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

    /// <summary>
    /// Deletes user by identifier.
    /// </summary>
    /// <param name="modelId">User identifier to delete.</param>
    public void Delete(int modelId)
    {
        var entity = this.users.GetById(modelId);
        if (entity == null)
        {
            return;
        }

        // IUserRepository doesn't have Delete, so delete directly through context
        this.context.Users.Remove(entity);
        this.context.SaveChanges();
    }
}
