// Path: Store.Tests/UserServiceTests.cs
using System;
using System.Linq;
using StoreBLL.Security;
using StoreBLL.Services;
using Xunit;

namespace Store.Tests;

public class UserServiceTests
{
    [Fact]
    public void Register_CreatesUser_WithHashedPassword()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);

            // Act
            var user = service.Register("testuser", "Test@123", "Test", "User");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user.Login);
            Assert.Equal(2, user.RoleId); // Default registered role
            Assert.False(user.IsBlocked);

            // Verify password is hashed
            var dbUser = ctx.Users.First(u => u.Login == "testuser");
            Assert.StartsWith("PBKDF2$", dbUser.Password);
            Assert.True(PasswordHasher.VerifyPassword("Test@123", dbUser.Password));
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Authenticate_ReturnsNull_ForInvalidCredentials()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);
            service.Register("testuser", "Test@123", "Test", "User");

            // Act
            var result = service.Authenticate("testuser", "WrongPassword");

            // Assert
            Assert.Null(result);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void Authenticate_ReturnsUser_ForValidCredentials()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);
            service.Register("testuser", "Test@123", "Test", "User");

            // Act
            var result = service.Authenticate("testuser", "Test@123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Login);
            Assert.Equal("Test", result.FirstName);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void BlockUser_SetsIsBlocked_ToTrue()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);
            var user = service.Register("blockme", "Test@123", "Block", "Me");

            // Act
            var result = service.BlockUser(user.Id);

            // Assert
            Assert.True(result);
            var dbUser = ctx.Users.First(u => u.Id == user.Id);
            Assert.True(dbUser.IsBlocked);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void ChangePassword_RequiresCorrectCurrentPassword()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);
            var user = service.Register("pwduser", "OldPass@123", "Pass", "User");

            // Act - wrong current password
            var wrongResult = service.ChangePassword(user.Id, "WrongCurrent", "NewPass@456");
            Assert.False(wrongResult);

            // Act - correct current password
            var correctResult = service.ChangePassword(user.Id, "OldPass@123", "NewPass@456");
            Assert.True(correctResult);

            // Verify new password works
            var auth = service.Authenticate("pwduser", "NewPass@456");
            Assert.NotNull(auth);
        }
        finally { cleanup(); }
    }

    [Fact]
    public void UpdateProfile_ChangesUserNames()
    {
        // Arrange
        var (ctx, cleanup) = TestDbHelper.CreateContext();
        try
        {
            var service = new UserService(ctx);
            var user = service.Register("profileuser", "Test@123", "Old", "Name");

            // Act
            var result = service.UpdateProfile(user.Id, "New", "Name");

            // Assert
            Assert.True(result);
            var updated = service.GetById(user.Id);
            Assert.Equal("New", updated.FirstName);
            Assert.Equal("Name", updated.LastName);
        }
        finally { cleanup(); }
    }
}