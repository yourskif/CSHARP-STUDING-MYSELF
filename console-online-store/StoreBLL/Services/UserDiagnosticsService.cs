// Path: console-online-store/StoreBLL/Services/UserDiagnosticsService.cs
namespace StoreBLL.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Security;

using StoreDAL.Entities;
using StoreDAL.UnitOfWork;

/// <summary>
/// Service for user diagnostics and admin operations.
/// Provides functionality for password hash verification and admin account management.
/// </summary>
public sealed class UserDiagnosticsService
{
    private readonly IStoreUnitOfWork unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDiagnosticsService"/> class.
    /// </summary>
    /// <param name="unitOfWork">Unit of Work for transaction management.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public UserDiagnosticsService(IStoreUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public IEnumerable<UserHashInfo> GetUsersWithHashInfo()
    {
        return this.unitOfWork.Context.Users
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

    public UserHashInfo ResetDefaultAdmin()
    {
        const string adminLogin = "admin";
        const string adminPassword = "Admin@123";

        var admin = this.unitOfWork.Context.Users.FirstOrDefault(u => u.Login == adminLogin);

        if (admin == null)
        {
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

            this.unitOfWork.Context.Users.Add(newAdmin);
            this.unitOfWork.SaveChanges();

            return new UserHashInfo
            {
                Id = newAdmin.Id,
                Login = newAdmin.Login,
                RoleId = newAdmin.RoleId,
                IsHashed = true,
                PasswordPreview = GetPasswordPreview(newAdmin.Password),
            };
        }

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
            this.unitOfWork.SaveChanges();
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

    public int GetTotalUserCount()
    {
        return this.unitOfWork.Context.Users.Count();
    }

    private static string GetPasswordPreview(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return string.Empty;
        }

        return password.Length > 28 ? password[..28] : password;
    }

    public class UserHashInfo
    {
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public bool IsHashed { get; set; }

        public string PasswordPreview { get; set; } = string.Empty;
    }
}
