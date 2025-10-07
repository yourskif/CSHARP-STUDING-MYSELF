// Path: console-online-store/Store.Tests/UserServiceTests.cs
#nullable disable
using System;
using System.Linq;
using StoreBLL.Services;
using StoreBLL.Models;
using StoreBLL.Security;
using Xunit;

namespace Store.Tests
{
    /// <summary>
    /// Unit tests for UserService class.
    /// Tests user registration, authentication, profile management, and admin operations.
    /// Verifies password hashing integration and business logic validation.
    /// </summary>
    public class UserServiceTests
    {
        /// <summary>
        /// Tests that Register creates a new user with hashed password.
        /// Verifies user creation with proper password encryption and default role assignment.
        /// </summary>
        [Fact]
        public void Register_CreatesNewUserWithHashedPassword()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string firstName = "John";
                string lastName = "Doe";
                string login = "johndoe";
                string password = "SecurePass123!";

                // Act
                var result = service.Register(firstName, lastName, login, password);

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Id > 0);
                Assert.Equal(firstName, result.FirstName);
                Assert.Equal(lastName, result.LastName);
                Assert.Equal(login, result.Login);
                Assert.Equal(2, result.RoleId); // Default role: Registered

                // Verify password is hashed (not plain text)
                Assert.NotEqual(password, result.Password);
                Assert.StartsWith("PBKDF2$", result.Password);

                // Verify hash works
                Assert.True(PasswordHasher.Verify(password, result.Password));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Register rejects duplicate login.
        /// Verifies uniqueness constraint enforcement.
        /// </summary>
        [Fact]
        public void Register_RejectsDuplicateLogin()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "duplicateuser";

                // Create first user
                service.Register("First", "User", login, "Password1");

                // Act - try to create second user with same login
                var result = service.Register("Second", "User", login, "Password2");

                // Assert
                Assert.Null(result);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Register validates required fields.
        /// Verifies proper exception handling for invalid inputs.
        /// </summary>
        [Fact]
        public void Register_ValidatesRequiredFields()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);

                // Act & Assert - empty firstName
                Assert.Throws<ArgumentException>(() =>
                    service.Register("", "Last", "login", "pass"));

                // Act & Assert - empty lastName
                Assert.Throws<ArgumentException>(() =>
                    service.Register("First", "", "login", "pass"));

                // Act & Assert - empty login
                Assert.Throws<ArgumentException>(() =>
                    service.Register("First", "Last", "", "pass"));

                // Act & Assert - empty password
                Assert.Throws<ArgumentException>(() =>
                    service.Register("First", "Last", "login", ""));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Authenticate succeeds with correct credentials.
        /// Verifies successful login with valid username and password.
        /// </summary>
        [Fact]
        public void Authenticate_SucceedsWithCorrectCredentials()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "testuser";
                string password = "TestPass123!";

                service.Register("Test", "User", login, password);

                // Act
                var result = service.Authenticate(login, password);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(login, result.Login);
                Assert.Equal("Test", result.FirstName);
                Assert.Equal("User", result.LastName);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Authenticate fails with incorrect password.
        /// Verifies rejection of invalid credentials.
        /// </summary>
        [Fact]
        public void Authenticate_FailsWithWrongPassword()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "testuser";
                string correctPassword = "CorrectPass123";
                string wrongPassword = "WrongPass456";

                service.Register("Test", "User", login, correctPassword);

                // Act
                var result = service.Authenticate(login, wrongPassword);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Authenticate fails for non-existent user.
        /// Verifies proper handling of invalid login attempts.
        /// </summary>
        [Fact]
        public void Authenticate_FailsForNonExistentUser()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);

                // Act
                var result = service.Authenticate("nonexistent", "password");

                // Assert
                Assert.Null(result);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Authenticate fails for blocked user.
        /// Verifies that blocked users cannot log in.
        /// </summary>
        [Fact]
        public void Authenticate_FailsForBlockedUser()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "blockeduser";
                string password = "Password123";

                var user = service.Register("Blocked", "User", login, password);

                // Block the user
                service.BlockUser(user.Id);

                // Act
                var result = service.Authenticate(login, password);

                // Assert
                Assert.Null(result);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that Authenticate handles null/empty credentials.
        /// Verifies proper validation of input parameters.
        /// </summary>
        [Fact]
        public void Authenticate_HandlesNullOrEmptyCredentials()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);

                // Act & Assert
                Assert.Null(service.Authenticate(null, "password"));
                Assert.Null(service.Authenticate("login", null));
                Assert.Null(service.Authenticate("", "password"));
                Assert.Null(service.Authenticate("login", ""));
                Assert.Null(service.Authenticate("   ", "password"));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that UpdateProfile modifies user information correctly.
        /// Verifies successful profile update for existing user.
        /// </summary>
        [Fact]
        public void UpdateProfile_ModifiesUserInformation()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                var user = service.Register("Original", "Name", "testuser", "Password123");

                // Act
                bool result = service.UpdateProfile(user.Id, "Updated", "NameChanged");

                // Assert
                Assert.True(result);

                var updated = service.GetById(user.Id) as UserModel;
                Assert.NotNull(updated);
                Assert.Equal("Updated", updated.FirstName);
                Assert.Equal("NameChanged", updated.LastName);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that UpdateProfile validates input fields.
        /// Verifies rejection of empty names.
        /// </summary>
        [Fact]
        public void UpdateProfile_ValidatesInputFields()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                var user = service.Register("Test", "User", "testuser", "Password123");

                // Act & Assert
                Assert.False(service.UpdateProfile(user.Id, "", "Last"));
                Assert.False(service.UpdateProfile(user.Id, "First", ""));
                Assert.False(service.UpdateProfile(user.Id, "   ", "Last"));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that ChangePassword validates old password before changing.
        /// Verifies security check for password changes.
        /// </summary>
        [Fact]
        public void ChangePassword_ValidatesOldPassword()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string oldPassword = "OldPass123";
                string newPassword = "NewPass456";

                var user = service.Register("Test", "User", "testuser", oldPassword);

                // Act - correct old password
                bool result = service.ChangePassword(user.Id, oldPassword, newPassword);

                // Assert
                Assert.True(result);

                // Verify new password works
                var auth = service.Authenticate("testuser", newPassword);
                Assert.NotNull(auth);

                // Verify old password no longer works
                var authOld = service.Authenticate("testuser", oldPassword);
                Assert.Null(authOld);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that ChangePassword fails with incorrect old password.
        /// Verifies rejection of unauthorized password changes.
        /// </summary>
        [Fact]
        public void ChangePassword_FailsWithWrongOldPassword()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string correctPassword = "CorrectPass123";
                string wrongOldPassword = "WrongPass";
                string newPassword = "NewPass456";

                var user = service.Register("Test", "User", "testuser", correctPassword);

                // Act
                bool result = service.ChangePassword(user.Id, wrongOldPassword, newPassword);

                // Assert
                Assert.False(result);

                // Verify original password still works
                var auth = service.Authenticate("testuser", correctPassword);
                Assert.NotNull(auth);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that BlockUser prevents authentication.
        /// Verifies user blocking functionality.
        /// </summary>
        [Fact]
        public void BlockUser_PreventsAuthentication()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "testuser";
                string password = "Password123";

                var user = service.Register("Test", "User", login, password);

                // Verify can authenticate before blocking
                Assert.NotNull(service.Authenticate(login, password));

                // Act
                bool result = service.BlockUser(user.Id);

                // Assert
                Assert.True(result);
                Assert.Null(service.Authenticate(login, password));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that UnblockUser restores authentication ability.
        /// Verifies user unblocking functionality.
        /// </summary>
        [Fact]
        public void UnblockUser_RestoresAuthentication()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                string login = "testuser";
                string password = "Password123";

                var user = service.Register("Test", "User", login, password);
                service.BlockUser(user.Id);

                // Verify blocked
                Assert.Null(service.Authenticate(login, password));

                // Act
                bool result = service.UnblockUser(user.Id);

                // Assert
                Assert.True(result);
                Assert.NotNull(service.Authenticate(login, password));
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that GetAll returns all registered users.
        /// Verifies user list retrieval.
        /// </summary>
        [Fact]
        public void GetAll_ReturnsAllUsers()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);

                // Act
                var users = service.GetAll().Cast<UserModel>().ToList();

                // Assert
                Assert.NotNull(users);
                Assert.NotEmpty(users);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that GetById returns correct user.
        /// Verifies user retrieval by ID.
        /// </summary>
        [Fact]
        public void GetById_ReturnsCorrectUser()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);
                var created = service.Register("Test", "User", "testuser", "Password123");

                // Act
                var retrieved = service.GetById(created.Id) as UserModel;

                // Assert
                Assert.NotNull(retrieved);
                Assert.Equal(created.Id, retrieved.Id);
                Assert.Equal(created.Login, retrieved.Login);
            }
            finally
            {
                cleanup();
            }
        }

        /// <summary>
        /// Tests that GetById throws exception for non-existent user.
        /// Verifies proper error handling.
        /// </summary>
        [Fact]
        public void GetById_ThrowsForNonExistentUser()
        {
            // Arrange
            var (ctx, cleanup) = TestDbHelper.CreateContext();
            try
            {
                var service = new UserService(ctx);

                // Act & Assert - UserRepository throws InvalidOperationException when user not found
                Assert.Throws<InvalidOperationException>(() => service.GetById(99999));
            }
            finally
            {
                cleanup();
            }
        }
    }
}
