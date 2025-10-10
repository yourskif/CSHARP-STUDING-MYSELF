// Path: console-online-store/Store.Tests/PasswordHasherTests.cs
#nullable disable
using System;

using StoreDAL.Security;

using Xunit;

namespace Store.Tests
{
    /// <summary>
    /// Unit tests for PasswordHasher class.
    /// Tests password hashing, verification, and security properties.
    /// Ensures compliance with PBKDF2 standards and legacy SHA-256 fallback.
    /// </summary>
    public class PasswordHasherTests
    {
        /// <summary>
        /// Tests that Hash creates a valid PBKDF2 hash with correct format.
        /// Verifies that hash contains scheme identifier, iterations, salt, and key.
        /// </summary>
        [Fact]
        public void Hash_CreatesValidPBKDF2Hash()
        {
            // Arrange
            string password = "TestPassword123!";

            // Act
            string hash = PasswordHasher.Hash(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.StartsWith("PBKDF2$", hash);

            var parts = hash.Split('$');
            Assert.Equal(4, parts.Length); // PBKDF2$iterations$salt$key
            Assert.Equal("PBKDF2", parts[0]);
            Assert.True(int.TryParse(parts[1], out int iterations));
            Assert.True(iterations > 0);
            Assert.NotEmpty(parts[2]); // salt
            Assert.NotEmpty(parts[3]); // key
        }

        /// <summary>
        /// Tests that Verify returns true when password matches the hash.
        /// Verifies successful password verification with PBKDF2.
        /// </summary>
        [Fact]
        public void Verify_ReturnsTrueForCorrectPassword()
        {
            // Arrange
            string password = "MySecurePassword!456";
            string hash = PasswordHasher.Hash(password);

            // Act
            bool result = PasswordHasher.Verify(password, hash);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that Verify returns false when password does not match.
        /// Verifies rejection of incorrect passwords.
        /// </summary>
        [Fact]
        public void Verify_ReturnsFalseForIncorrectPassword()
        {
            // Arrange
            string correctPassword = "CorrectPassword123";
            string wrongPassword = "WrongPassword456";
            string hash = PasswordHasher.Hash(correctPassword);

            // Act
            bool result = PasswordHasher.Verify(wrongPassword, hash);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that Hash generates different hashes for the same password.
        /// Verifies that salt randomization works correctly (prevents rainbow table attacks).
        /// </summary>
        [Fact]
        public void Hash_GeneratesDifferentHashesForSamePassword()
        {
            // Arrange
            string password = "SamePassword789";

            // Act
            string hash1 = PasswordHasher.Hash(password);
            string hash2 = PasswordHasher.Hash(password);

            // Assert
            Assert.NotEqual(hash1, hash2);

            // But both should verify correctly
            Assert.True(PasswordHasher.Verify(password, hash1));
            Assert.True(PasswordHasher.Verify(password, hash2));
        }

        /// <summary>
        /// Tests that Verify handles null and empty inputs safely.
        /// Verifies proper validation of input parameters.
        /// </summary>
        [Fact]
        public void Verify_HandlesMalformedInputsSafely()
        {
            // Arrange
            string password = "ValidPassword";

            // Act & Assert - null/empty hash
            Assert.False(PasswordHasher.Verify(password, null));
            Assert.False(PasswordHasher.Verify(password, string.Empty));
            Assert.False(PasswordHasher.Verify(password, "   "));

            // Malformed hash
            Assert.False(PasswordHasher.Verify(password, "InvalidHash"));
            Assert.False(PasswordHasher.Verify(password, "PBKDF2$invalid"));
        }

        /// <summary>
        /// Tests that Hash throws ArgumentNullException for null password.
        /// Verifies proper exception handling for invalid input.
        /// </summary>
        [Fact]
        public void Hash_ThrowsArgumentNullExceptionForNullPassword()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Hash(null));
        }

        /// <summary>
        /// Tests that Verify throws ArgumentNullException for null password.
        /// Verifies proper exception handling for invalid input.
        /// </summary>
        [Fact]
        public void Verify_ThrowsArgumentNullExceptionForNullPassword()
        {
            // Arrange
            string hash = "PBKDF2$100000$somesalt$somekey";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.Verify(null, hash));
        }

        /// <summary>
        /// Tests legacy SHA-256 hex hash verification for backward compatibility.
        /// Verifies that old password hashes still work after migration.
        /// </summary>
        [Fact]
        public void Verify_SupportsLegacySHA256HexHash()
        {
            // Arrange
            string password = "admin123";

            // SHA-256 hash of "admin123" in uppercase hex (legacy format)
            string legacyHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9";

            // Act
            bool result = PasswordHasher.Verify(password, legacyHash);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that legacy SHA-256 verification is case-insensitive.
        /// Verifies backward compatibility with both uppercase and lowercase hashes.
        /// </summary>
        [Fact]
        public void Verify_LegacySHA256IsCaseInsensitive()
        {
            // Arrange
            string password = "test";

            // SHA-256 hash of "test" in different cases
            string uppercaseHash = "9F86D081884C7D659A2FEAA0C55AD015A3BF4F1B2B0B822CD15D6C15B0F00A08";
            string lowercaseHash = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

            // Act & Assert
            Assert.True(PasswordHasher.Verify(password, uppercaseHash));
            Assert.True(PasswordHasher.Verify(password, lowercaseHash));
        }

        /// <summary>
        /// Tests that HashPassword and VerifyPassword aliases work correctly.
        /// Verifies backward compatibility of old method names.
        /// </summary>
        [Fact]
        public void BackwardCompatible_AliasMethodsWork()
        {
            // Arrange
            string password = "TestAlias123";

            // Act
            string hash = PasswordHasher.HashPassword(password);
            bool result = PasswordHasher.VerifyPassword(password, hash);

            // Assert
            Assert.NotNull(hash);
            Assert.True(result);
        }

        /// <summary>
        /// Tests that different passwords produce different hashes.
        /// Verifies basic hash uniqueness property.
        /// </summary>
        [Fact]
        public void Hash_DifferentPasswordsProduceDifferentHashes()
        {
            // Arrange
            string password1 = "Password1";
            string password2 = "Password2";

            // Act
            string hash1 = PasswordHasher.Hash(password1);
            string hash2 = PasswordHasher.Hash(password2);

            // Assert
            Assert.NotEqual(hash1, hash2);
            Assert.True(PasswordHasher.Verify(password1, hash1));
            Assert.True(PasswordHasher.Verify(password2, hash2));
            Assert.False(PasswordHasher.Verify(password1, hash2));
            Assert.False(PasswordHasher.Verify(password2, hash1));
        }

        /// <summary>
        /// Tests that Verify handles edge cases like empty password.
        /// Verifies security behavior for unusual inputs.
        /// </summary>
        [Fact]
        public void Verify_HandlesEmptyPassword()
        {
            // Arrange
            string emptyPassword = string.Empty;
            string hash = PasswordHasher.Hash(emptyPassword);

            // Act
            bool correctResult = PasswordHasher.Verify(emptyPassword, hash);
            bool wrongResult = PasswordHasher.Verify("notEmpty", hash);

            // Assert
            Assert.True(correctResult);
            Assert.False(wrongResult);
        }

        /// <summary>
        /// Tests that hash format includes expected iteration count.
        /// Verifies security parameter configuration.
        /// </summary>
        [Fact]
        public void Hash_ContainsExpectedIterationCount()
        {
            // Arrange
            string password = "TestIterations";

            // Act
            string hash = PasswordHasher.Hash(password);
            var parts = hash.Split('$');
            int iterations = int.Parse(parts[1]);

            // Assert
            Assert.True(iterations >= 100000, "Iteration count should be at least 100,000 for security");
        }
    }
}
