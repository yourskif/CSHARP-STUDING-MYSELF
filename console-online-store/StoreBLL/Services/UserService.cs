// Path: C:\Users\SK\source\repos\C#\1414\console-online-store\StoreBLL\Services\UserService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StoreBLL.Models;
using StoreBLL.Security;
using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// User business logic with compatibility for different DAL.User schemas.
/// Required columns: Id, Login, Password, RoleId.
/// Optional columns: FirstName/LastName OR Name; IsBlocked/Blocked/IsActive/Status for blocking.
/// </summary>
public class UserService
{
    private readonly StoreDbContext context;

    /// <summary>Initializes a new instance of the <see cref="UserService"/> class.</summary>
    /// <param name="context">EF Core context.</param>
    public UserService(StoreDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ===================== AUTH =====================

    /// <summary>
    /// Authenticates by login and password (PBKDF2 verification).
    /// </summary>
    /// <param name="login">User login.</param>
    /// <param name="password">Plain password to check.</param>
    /// <returns>Authenticated <see cref="UserModel"/> or <c>null</c> if credentials invalid.</returns>
    public UserModel? Authenticate(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var entity = this.context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Login == login);

        if (entity is null)
        {
            return null;
        }

        if (!PasswordHasher.VerifyPassword(password, entity.Password))
        {
            return null;
        }

        return ToModel(entity);
    }

    // ===================== CRUD / PROFILE =====================

    /// <summary>
    /// Registers a new user from a model. Password is hashed.
    /// </summary>
    /// <param name="model">User data.</param>
    /// <returns>Created user model without password.</returns>
    public UserModel Register(UserModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new User
        {
            Login = model.Login ?? string.Empty,
            Password = PasswordHasher.HashPassword(model.Password ?? string.Empty),
            RoleId = model.RoleId == 0 ? 2 : model.RoleId, // default user
        };

        // Names: prefer FirstName/LastName, else single Name column
        TrySetString(entity, "FirstName", model.FirstName ?? string.Empty);
        TrySetString(entity, "LastName", model.LastName ?? string.Empty);

        if (!HasProperty(entity, "FirstName") && !HasProperty(entity, "LastName") && HasProperty(entity, "Name"))
        {
            TrySetString(entity, "Name", ComposeName(model.FirstName, model.LastName, model.Login));
        }

        this.context.Users.Add(entity);
        this.context.SaveChanges();

        return ToModel(entity);
    }

    /// <summary>
    /// Overload expected by Console layer: Register(login, password, firstName, lastName) → roleId = 2.
    /// </summary>
    /// <param name="login">Login.</param>
    /// <param name="password">Plain password.</param>
    /// <param name="firstName">First name (optional).</param>
    /// <param name="lastName">Last name (optional).</param>
    /// <returns>Created user model.</returns>
    public UserModel Register(string login, string password, string? firstName, string? lastName)
    {
        return this.Register(new UserModel
        {
            Login = login,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            RoleId = 2,
        });
    }

    /// <summary>
    /// Updates profile names (user self-service).
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="firstName">New first name (optional).</param>
    /// <param name="lastName">New last name (optional).</param>
    /// <returns><c>true</c> if saved; otherwise <c>false</c>.</returns>
    public bool UpdateProfile(int userId, string? firstName, string? lastName)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == userId);
        if (e is null)
        {
            return false;
        }

        bool changed = false;

        if (HasProperty(e, "FirstName") || HasProperty(e, "LastName"))
        {
            var curFirst = ReadString(e, "FirstName") ?? string.Empty;
            var curLast = ReadString(e, "LastName") ?? string.Empty;

            var newFirst = firstName ?? curFirst;
            var newLast = lastName ?? curLast;

            if (!string.Equals(newFirst, curFirst, StringComparison.Ordinal))
            {
                TrySetString(e, "FirstName", newFirst);
                changed = true;
            }

            if (!string.Equals(newLast, curLast, StringComparison.Ordinal))
            {
                TrySetString(e, "LastName", newLast);
                changed = true;
            }
        }
        else if (HasProperty(e, "Name"))
        {
            var cur = ReadString(e, "Name") ?? e.Login;
            var composed = ComposeName(firstName, lastName, e.Login);
            var newName = string.IsNullOrWhiteSpace(composed) ? cur : composed;

            if (!string.Equals(newName, cur, StringComparison.Ordinal))
            {
                TrySetString(e, "Name", newName);
                changed = true;
            }
        }

        if (changed)
        {
            this.context.SaveChanges();
        }

        return changed;
    }

    /// <summary>
    /// Changes password (verifies current).
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="currentPassword">Current plain password.</param>
    /// <param name="newPassword">New plain password.</param>
    /// <returns><c>true</c> if changed; otherwise <c>false</c>.</returns>
    public bool ChangePassword(int userId, string currentPassword, string newPassword)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == userId);
        if (e is null)
        {
            return false;
        }

        if (!PasswordHasher.VerifyPassword(currentPassword, e.Password))
        {
            return false;
        }

        e.Password = PasswordHasher.HashPassword(newPassword);
        this.context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Admin update: may change role and names from full model.
    /// </summary>
    /// <param name="model">User model containing changes.</param>
    public void UpdateByAdmin(UserModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var e = this.context.Users.FirstOrDefault(u => u.Id == model.Id)
            ?? throw new KeyNotFoundException($"User with id {model.Id} not found.");

        if (!string.IsNullOrWhiteSpace(model.Login))
        {
            e.Login = model.Login!;
        }

        if (model.RoleId != 0)
        {
            e.RoleId = model.RoleId;
        }

        if (HasProperty(e, "FirstName") || HasProperty(e, "LastName"))
        {
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                TrySetString(e, "FirstName", model.FirstName ?? string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                TrySetString(e, "LastName", model.LastName ?? string.Empty);
            }
        }
        else if (HasProperty(e, "Name"))
        {
            var composed = ComposeName(model.FirstName, model.LastName, e.Login);
            if (!string.IsNullOrWhiteSpace(composed))
            {
                TrySetString(e, "Name", composed);
            }
        }

        this.context.SaveChanges();
    }

    /// <summary>
    /// Overload expected by AdminUsersMenu: change only role (id, roleId).
    /// </summary>
    /// <param name="id">User id.</param>
    /// <param name="roleId">New role id.</param>
    /// <returns><c>true</c> if role updated or already the same; otherwise <c>false</c> if user not found.</returns>
    public bool UpdateByAdmin(int id, int roleId)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == id);
        if (e is null)
        {
            return false;
        }

        if (e.RoleId == roleId)
        {
            return true;
        }

        e.RoleId = roleId;
        this.context.SaveChanges();
        return true;
    }

    /// <summary>
    /// Overload expected by AdminUsersMenu: full model + out error.
    /// </summary>
    /// <param name="model">User model with changes.</param>
    /// <param name="error">Output error message.</param>
    /// <returns><c>true</c> on success; otherwise <c>false</c>.</returns>
    public bool UpdateByAdmin(UserModel model, out string error)
    {
        error = string.Empty;

        if (model is null)
        {
            error = "Model is null.";
            return false;
        }

        var e = this.context.Users.FirstOrDefault(u => u.Id == model.Id);
        if (e is null)
        {
            error = $"User with id {model.Id} not found.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(model.Login))
        {
            e.Login = model.Login!;
        }

        if (model.RoleId != 0 && e.RoleId != model.RoleId)
        {
            e.RoleId = model.RoleId;
        }

        bool hasFirst = HasProperty(e, "FirstName");
        bool hasLast = HasProperty(e, "LastName");
        bool hasName = HasProperty(e, "Name");

        if (hasFirst || hasLast)
        {
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                TrySetString(e, "FirstName", model.FirstName ?? string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                TrySetString(e, "LastName", model.LastName ?? string.Empty);
            }
        }
        else if (hasName)
        {
            var composed = ComposeName(model.FirstName, model.LastName, e.Login);
            TrySetString(e, "Name", string.IsNullOrWhiteSpace(composed) ? (model.Login ?? e.Login) : composed);
        }

        this.context.SaveChanges();
        return true;
    }

    /// <summary>Deletes a user by id.</summary>
    /// <param name="id">User id.</param>
    public void Delete(int id)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == id);
        if (e is null)
        {
            return;
        }

        this.context.Users.Remove(e);
        this.context.SaveChanges();
    }

    // ===================== ADMIN: BLOCK / UNBLOCK =====================

    /// <summary>
    /// Blocks a user (tries IsBlocked/Blocked/IsActive/Status).
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns><c>true</c> if a supported flag was changed; otherwise <c>false</c>.</returns>
    public bool BlockUser(int id)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == id);
        if (e is null)
        {
            return false;
        }

        bool changed =
            TrySetBool(e, "IsBlocked", true) ||
            TrySetBool(e, "Blocked", true) ||
            TrySetBool(e, "IsActive", false) ||
            TrySetInt(e, "Status", 0);

        if (changed)
        {
            this.context.SaveChanges();
        }

        return changed;
    }

    /// <summary>
    /// Unblocks a user (reverse of <see cref="BlockUser(int)"/>).
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns><c>true</c> if a supported flag was changed; otherwise <c>false</c>.</returns>
    public bool UnblockUser(int id)
    {
        var e = this.context.Users.FirstOrDefault(u => u.Id == id);
        if (e is null)
        {
            return false;
        }

        bool changed =
            TrySetBool(e, "IsBlocked", false) ||
            TrySetBool(e, "Blocked", false) ||
            TrySetBool(e, "IsActive", true) ||
            TrySetInt(e, "Status", 1);

        if (changed)
        {
            this.context.SaveChanges();
        }

        return changed;
    }

    // ===================== QUERIES =====================

    /// <summary>
    /// Gets user by id (no password exposed).
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>User model or <c>null</c>.</returns>
    public UserModel? GetById(int id)
    {
        var e = this.context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
        return e is null ? null : ToModel(e);
    }

    /// <summary>
    /// Returns all users ordered by Id (no password exposed).
    /// </summary>
    /// <returns>Sequence of <see cref="UserModel"/>.</returns>
    public IEnumerable<UserModel> GetAll()
    {
        return this.context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .AsEnumerable()
            .Select(ToModel)
            .ToList();
    }

    // ===================== helpers =====================

    /// <summary>Maps DAL user entity to BLL model (without password).</summary>
    /// <param name="e">DAL user entity.</param>
    /// <returns>Mapped <see cref="UserModel"/>.</returns>
    private static UserModel ToModel(User e)
    {
        // FirstName/LastName -> prefer; else fallback to Name; else Login.
        string first = ReadString(e, "FirstName") ?? string.Empty;
        string last = ReadString(e, "LastName") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(first))
        {
            var single = ReadString(e, "Name");
            if (!string.IsNullOrWhiteSpace(single))
            {
                first = single;
            }
        }

        return new UserModel
        {
            Id = e.Id,
            Login = e.Login,
            RoleId = e.RoleId,
            FirstName = string.IsNullOrWhiteSpace(first) ? null : first,
            LastName = string.IsNullOrWhiteSpace(last) ? null : last,
            Password = string.Empty,
        };
    }

    /// <summary>Builds a display name from parts with fallback.</summary>
    private static string ComposeName(string? first, string? last, string? fallback)
    {
        var f = (first ?? string.Empty).Trim();
        var l = (last ?? string.Empty).Trim();
        var full = $"{f} {l}".Trim();
        return string.IsNullOrWhiteSpace(full) ? (fallback ?? string.Empty) : full;
    }

    /// <summary>Checks that an object exposes a public (case-insensitive) property.</summary>
    private static bool HasProperty(object obj, string name)
    {
        var pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        return pi != null && (pi.CanRead || pi.CanWrite);
    }

    /// <summary>Reads a string property value if present and readable.</summary>
    private static string? ReadString(object obj, string name)
    {
        var pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (pi is null || !pi.CanRead)
        {
            return null;
        }

        return pi.GetValue(obj) as string;
    }

    /// <summary>Sets a string property value if property exists and is writable.</summary>
    private static void TrySetString(object obj, string name, string? value)
    {
        var pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (pi is null || !pi.CanWrite)
        {
            return;
        }

        pi.SetValue(obj, value ?? string.Empty);
    }

    /// <summary>Tries to set a boolean-like property (bool or bool?).</summary>
    private static bool TrySetBool(object obj, string name, bool value)
    {
        var pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (pi is null || !pi.CanWrite)
        {
            return false;
        }

        if (pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?))
        {
            pi.SetValue(obj, value);
            return true;
        }

        return false;
    }

    /// <summary>Tries to set an integer-like property (int or int?).</summary>
    private static bool TrySetInt(object obj, string name, int value)
    {
        var pi = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (pi is null || !pi.CanWrite)
        {
            return false;
        }

        if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
        {
            pi.SetValue(obj, value);
            return true;
        }

        return false;
    }
}
