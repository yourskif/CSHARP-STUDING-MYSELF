// Path: console-online-store/StoreBLL/Services/UserService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;
using StoreBLL.Security;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

/// <summary>
/// Business logic service for user management operations:
/// registration, authentication, profile management, and admin actions (block/unblock/delete).
/// </summary>
public class UserService : ICrud
{
    private readonly IStoreUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public UserService(IStoreUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
            roleId: m.RoleId)
        {
            IsBlocked = m.IsBlocked,
        };

        this.unitOfWork.Users.Add(entity);
        this.unitOfWork.SaveChanges();
        m.Id = entity.Id;
    }

    public void Delete(int modelId)
    {
        var entity = this.unitOfWork.Users.GetById(modelId);
        if (entity == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (this.unitOfWork.Users.HasOrders(modelId))
        {
            throw new InvalidOperationException("Cannot delete a user who has existing orders. Consider blocking instead.");
        }

        if (entity.RoleId == 1)
        {
            throw new InvalidOperationException("Cannot delete an administrator account.");
        }

        this.unitOfWork.Users.DeleteById(modelId);
        this.unitOfWork.SaveChanges();
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.unitOfWork.Users.GetAll().Select(u =>
            new UserModel
            {
                Id = u.Id,
                FirstName = u.Name,
                LastName = u.LastName,
                Login = u.Login,
                Password = u.Password,
                RoleId = u.RoleId,
                IsBlocked = u.IsBlocked,
            });
    }

    public AbstractModel GetById(int id)
    {
        var u = this.unitOfWork.Users.GetById(id)
            ?? throw new KeyNotFoundException($"User with id {id} not found.");

        return new UserModel
        {
            Id = u.Id,
            FirstName = u.Name,
            LastName = u.LastName,
            Login = u.Login,
            Password = u.Password,
            RoleId = u.RoleId,
            IsBlocked = u.IsBlocked,
        };
    }

    public void Update(AbstractModel model)
    {
        if (model is not UserModel m)
        {
            throw new ArgumentException("Expected UserModel", nameof(model));
        }

        var entity = this.unitOfWork.Users.GetById(m.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"User with id {m.Id} not found.");
        }

        entity.Name = m.FirstName;
        entity.LastName = m.LastName;
        entity.Login = m.Login;
        entity.Password = m.Password;
        entity.RoleId = m.RoleId;

        this.unitOfWork.Users.Update(entity);
        this.unitOfWork.SaveChanges();
    }

    public UserModel? Register(string firstName, string lastName, string login, string password)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cannot be empty.", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        var existingUser = this.unitOfWork.Users.FindByLogin(login);
        if (existingUser != null)
        {
            return null;
        }

        string passwordHash = PasswordHasher.HashPassword(password);

        var userEntity = new User(
            id: 0,
            name: firstName.Trim(),
            lastName: lastName.Trim(),
            login: login.Trim(),
            password: passwordHash,
            roleId: 2);

        this.unitOfWork.Users.Add(userEntity);
        this.unitOfWork.SaveChanges();

        return new UserModel
        {
            Id = userEntity.Id,
            FirstName = userEntity.Name,
            LastName = userEntity.LastName,
            Login = userEntity.Login,
            Password = userEntity.Password,
            RoleId = userEntity.RoleId,
            IsBlocked = userEntity.IsBlocked,
        };
    }

    public UserModel? Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var userEntity = this.unitOfWork.Users.FindByLogin(login);
        if (userEntity == null)
        {
            return null;
        }

        if (userEntity.IsBlocked)
        {
            return null;
        }

        if (!PasswordHasher.VerifyPassword(password, userEntity.Password))
        {
            return null;
        }

        return new UserModel
        {
            Id = userEntity.Id,
            FirstName = userEntity.Name,
            LastName = userEntity.LastName,
            Login = userEntity.Login,
            Password = userEntity.Password,
            RoleId = userEntity.RoleId,
            IsBlocked = userEntity.IsBlocked,
        };
    }

    public bool UpdateProfile(int userId, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return false;
        }

        var userEntity = this.unitOfWork.Users.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        userEntity.Name = firstName.Trim();
        userEntity.LastName = lastName.Trim();
        this.unitOfWork.Users.Update(userEntity);
        this.unitOfWork.SaveChanges();

        return true;
    }

    public bool ChangePassword(int userId, string currentPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        var userEntity = this.unitOfWork.Users.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        if (!PasswordHasher.VerifyPassword(currentPassword, userEntity.Password))
        {
            return false;
        }

        userEntity.Password = PasswordHasher.HashPassword(newPassword);
        this.unitOfWork.Users.Update(userEntity);
        this.unitOfWork.SaveChanges();

        return true;
    }

    public bool BlockUser(int userId)
    {
        var userEntity = this.unitOfWork.Users.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        if (!userEntity.IsBlocked)
        {
            userEntity.IsBlocked = true;
            this.unitOfWork.Users.Update(userEntity);
            this.unitOfWork.SaveChanges();
        }

        return true;
    }

    public bool UnblockUser(int userId)
    {
        var userEntity = this.unitOfWork.Users.GetById(userId);
        if (userEntity == null)
        {
            return false;
        }

        if (userEntity.IsBlocked)
        {
            userEntity.IsBlocked = false;
            this.unitOfWork.Users.Update(userEntity);
            this.unitOfWork.SaveChanges();
        }

        return true;
    }

    public bool UpdateByAdmin(UserModel input, out string error)
    {
        error = string.Empty;

        var entity = this.unitOfWork.Users.GetById(input.Id);
        if (entity == null)
        {
            error = "User not found.";
            return false;
        }

        var first = (input.FirstName ?? string.Empty).Trim();
        var last = (input.LastName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            error = "First and Last name cannot be empty.";
            return false;
        }

        var login = (input.Login ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(login) && !string.Equals(login, entity.Login, StringComparison.Ordinal))
        {
            var exists = this.unitOfWork.Users.FindByLogin(login);
            if (exists != null && exists.Id != entity.Id)
            {
                error = "Login is already taken.";
                return false;
            }

            entity.Login = login;
        }

        int newRoleId = input.RoleId == 0 ? entity.RoleId : input.RoleId;
        if (newRoleId != entity.RoleId)
        {
            if (newRoleId != 1 && newRoleId != 2)
            {
                error = "Invalid role id. Allowed: 1 (admin), 2 (user).";
                return false;
            }

            if (entity.RoleId == 1 && newRoleId != 1)
            {
                var admins = this.unitOfWork.Users.GetAll().Count(u => u.RoleId == 1);
                if (admins <= 1)
                {
                    error = "Cannot remove the last administrator.";
                    return false;
                }
            }

            entity.RoleId = newRoleId;
        }

        entity.Name = first;
        entity.LastName = last;

        this.unitOfWork.Users.Update(entity);
        this.unitOfWork.SaveChanges();
        return true;
    }
}
