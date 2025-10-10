// Path: console-online-store/StoreBLL/Services/UserDiagnosticsService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Security;

using StoreDAL.Data;
using StoreDAL.Entities;

/// <summary>
/// Service for user diagnostics and admin operations.
/// Provides functionality for password hash verification and admin account management.
/// </summary>
public sealed class UserDiagnosticsService
{
    private readonly StoreDbContext db;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDiagnosticsService"/> class.
    /// </summary>
    /// <param name="db">Database context for user operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    public UserDiagnosticsService(StoreDbContext db)
    {
        this.db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// Gets all users with password hash information.
    /// </summary>
    /// <returns>Enumerable of users with hash verification status.</returns>
    public IEnumerable<UserHashInfo> GetUsersWithHashInfo()
    {
        return this.db.Users
            .OrderBy(u => u.Id)
            .Take(100)
            .ToList()
            .Select(u => new UserHashInfo
            {
                Id = u.Id,
                Login = u.Login,
                RoleId = u.RoleId,
                IsHashed = u.Password?.StartsWith("PBKDF2$", StringComparison.Ordinal) == true,
                PasswordPreview = GetPasswordPreview(u.Password),
            });
    }

    /// <summary>
    /// Resets default admin account to standard credentials.
    /// Sets login to "admin" and password to "Admin@123" with proper hashing.
    /// </summary>
    /// <returns>Information about reset admin account.</returns>
    public UserHashInfo ResetDefaultAdmin()
    {
        const string adminLogin = "admin";
        const string adminPassword = "Admin@123";

        var admin = this.db.Users.FirstOrDefault(u => u.Login == adminLogin);

        if (admin == null)
        {
            // Create new admin user
            var adminHash = PasswordHasher.HashPassword(adminPassword);
            var newAdmin = new User
            {
                Login = adminLogin,
                Password = adminHash,
                RoleId = 1,
                Name = "Admin",
                LastName = "Root",
                IsBlocked = false,
            };

            this.db.Users.Add(newAdmin);
            this.db.SaveChanges();

            return new UserHashInfo
            {
                Id = newAdmin.Id,
                Login = newAdmin.Login,
                RoleId = newAdmin.RoleId,
                IsHashed = true,
                PasswordPreview = GetPasswordPreview(newAdmin.Password),
            };
        }

        // Fix existing admin
        bool changed = false;

        if (admin.RoleId != 1)
        {
            admin.RoleId = 1;
            changed = true;
        }

        if (!PasswordHasher.VerifyPassword(adminPassword, admin.Password))
        {
            admin.Password = PasswordHasher.HashPassword(adminPassword);
            changed = true;
        }

        if (admin.IsBlocked)
        {
            admin.IsBlocked = false;
            changed = true;
        }

        if (changed)
        {
            this.db.SaveChanges();
        }

        bool hashed = admin.Password?.StartsWith("PBKDF2$", StringComparison.Ordinal) == true;

        return new UserHashInfo
        {
            Id = admin.Id,
            Login = admin.Login,
            RoleId = admin.RoleId,
            IsHashed = hashed,
            PasswordPreview = GetPasswordPreview(admin.Password),
        };
    }

    /// <summary>
    /// Gets total count of users.
    /// </summary>
    /// <returns>Total user count.</returns>
    public int GetTotalUserCount()
    {
        return this.db.Users.Count();
    }

    private static string GetPasswordPreview(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return string.Empty;
        }

        return password.Length > 28 ? password[..28] : password;
    }

    /// <summary>
    /// Represents user with password hash information.
    /// </summary>
    public class UserHashInfo
    {
        /// <summary>
        /// Gets or sets user ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user login.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user role ID.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether password is hashed with PBKDF2.
        /// </summary>
        public bool IsHashed { get; set; }

        /// <summary>
        /// Gets or sets password preview (first 28 characters).
        /// </summary>
        public string PasswordPreview { get; set; } = string.Empty;
    }
}
