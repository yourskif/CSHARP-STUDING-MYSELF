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

/// <summary>
/// Business logic service for user management operations.
/// Handles user registration, authentication, and profile management.
/// </summary>
public class UserService : ICrud
{
    private readonly IUserRepository repository;

    public UserService(StoreDbContext context)
    {
        this.repository = new UserRepository(context);
    }

    public void Add(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Expected UserModel", nameof(model));
        }

        var entity = new User(
            id: 0,
            name: m.FirstName,
            lastName: m.LastName,
            login: m.Login,
            password: m.Password,
            roleId: m.RoleId);

        this.repository.Add(entity);
        this.repository.SaveChanges();
        m.Id = entity.Id;
    }

    public void Delete(int modelId)
    {
        var entity = this.repository.GetById(modelId);
        if (entity != null)
        {
            // Note: IUserRepository doesn't have Delete method - would need to be added
            throw new NotImplementedException("Delete method not available in current IUserRepository interface");
        }
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(u =>
            new UserModel
            {
                Id = u.Id,
                FirstName = u.Name,
                LastName = u.LastName,
                Login = u.Login,
                Password = u.Password,
                RoleId = u.RoleId
            });
    }

    public AbstractModel? GetById(int id)
    {
        var u = this.repository.GetById(id);
        if (u == null)
        {
            return null;
        }

        return new UserModel
        {
            Id = u.Id,
            FirstName = u.Name,
            LastName = u.LastName,
            Login = u.Login,
            Password = u.Password,
            RoleId = u.RoleId
        };
    }

    public void Update(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Expected UserModel", nameof(model));
        }

        var entity = this.repository.GetById(m.Id);
        if (entity == null)
        {
            return;
        }

        entity.Name = m.FirstName;
        entity.LastName = m.LastName;
        entity.Login = m.Login;
        entity.Password = m.Password;
        entity.RoleId = m.RoleId;

        this.repository.SaveChanges();
    }

    /// <summary>
    /// Registers a new user with default User role (roleId = 2).
    /// </summary>
    /// <param name="firstName">User's first name.</param>
    /// <param name="lastName">User's last name.</param>
    /// <param name="login">Unique login name.</param>
    /// <param name="password">Plain text password (will be hashed).</param>
    /// <returns>Created user model if successful, null if login already exists.</returns>
    /// <exception cref="ArgumentException">Thrown if any parameter is invalid.</exception>
    public UserModel? Register(string firstName, string lastName, string login, string password)
    {
        // Validate input parameters
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty.", nameof(login));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        // Check if login already exists
        var existingUser = this.repository.FindByLogin(login);
        if (existingUser != null)
        {
            return null; // Login already exists
        }

        // Hash the password
        string passwordHash = PasswordHasher.HashPassword(password);

        // Create new user entity with User role (roleId = 2)
        var userEntity = new User(
            id: 0,
            name: firstName.Trim(),
            lastName: lastName.Trim(),
            login: login.Trim(),
            password: passwordHash,
            roleId: 2); // Default User role

        this.repository.Add(userEntity);
        this.repository.SaveChanges();

        return new UserModel
        {
            Id = userEntity.Id,
            FirstName = userEntity.Name,
            LastName = userEntity.LastName,
            Login = userEntity.Login,
            Password = userEntity.Password,
            RoleId = userEntity.RoleId
        };
    }

    /// <summary>
    /// Authenticates a user with login and password.
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="password">Plain text password.</param>
    /// <returns>UserModel if authentication successful, null otherwise.</returns>
    public UserModel? Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var userEntity = this.repository.FindByLogin(login);
        if (userEntity == null)
        {
            return null; // User not found
        }

        // Verify password
        if (!PasswordHasher.VerifyPassword(password, userEntity.Password))
        {
            return null; // Invalid password
        }

        return new UserModel
        {
            Id = userEntity.Id,
            FirstName = userEntity.Name,
            LastName = userEntity.LastName,
            Login = userEntity.Login,
            Password = userEntity.Password,
            RoleId = userEntity.RoleId
        };
    }

    /// <summary>
    /// Updates user profile information.
    /// </summary>
    /// <param name="userId">User ID to update.</param>
    /// <param name="firstName">New first name.</param>
    /// <param name="lastName">New last name.</param>
    /// <returns>True if update successful, false otherwise.</returns>
    public bool UpdateProfile(int userId, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return false;
        }

        var userEntity = this.repository.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        userEntity.Name = firstName.Trim();
        userEntity.LastName = lastName.Trim();
        this.repository.SaveChanges();

        return true;
    }

    /// <summary>
    /// Changes user password.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="currentPassword">Current password for verification.</param>
    /// <param name="newPassword">New password.</param>
    /// <returns>True if password changed successfully, false otherwise.</returns>
    public bool ChangePassword(int userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        var userEntity = this.repository.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        // Verify current password
        if (!PasswordHasher.VerifyPassword(currentPassword, userEntity.Password))
        {
            return false;
        }

        // Hash and set new password
        userEntity.Password = PasswordHasher.HashPassword(newPassword);
        this.repository.SaveChanges();

        return true;
    }
}
