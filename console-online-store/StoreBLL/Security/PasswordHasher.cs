namespace StoreBLL.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Utility for hashing and verifying passwords.
    /// Uses PBKDF2 (SHA-256) with salt and iterations.
    /// Includes a legacy SHA-256 hex fallback for old stored hashes.
    /// </summary>
    public static class PasswordHasher
    {
        // --- constants (must be placed before non-constant members for SA1203) ---
        private const int SaltSize = 16;          // 128-bit
        private const int KeySize = 32;           // 256-bit
        private const int Iterations = 100_000;   // PBKDF2 iterations
        private const char DelimiterChar = '$';
        private const string Delimiter = "$";
        private const string Scheme = "PBKDF2";

        // Readonly struct value (HashAlgorithmName cannot be const)
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <returns>Hash string in format "PBKDF2$iterations$saltBase64$keyBase64".</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="password"/> is null.</exception>
        public static string Hash(string password)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            var key = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                Algorithm,
                KeySize);

            return string.Join(
                Delimiter,
                Scheme,
                Iterations.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        /// <summary>
        /// Verifies that a plain password matches the stored hash.
        /// Supports PBKDF2 format and a legacy SHA-256 hex hash.
        /// </summary>
        /// <param name="password">Plain text password to check.</param>
        /// <param name="hash">Stored hash (PBKDF2 format or legacy SHA-256 hex).</param>
        /// <returns><see langword="true"/> if the password matches; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="password"/> is null.</exception>
        public static bool Verify(string password, string hash)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            var parts = hash.Split(DelimiterChar);
            if (parts.Length == 4 && parts[0] == Scheme)
            {
                if (!int.TryParse(parts[1], out var iterations))
                {
                    return false;
                }

                var salt = Convert.FromBase64String(parts[2]);
                var key = Convert.FromBase64String(parts[3]);

                var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(password),
                    salt,
                    iterations,
                    Algorithm,
                    key.Length);

                return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
            }

            // Legacy fallback: compare with SHA-256 hex (uppercase/lowercase ignored).
            using var sha = SHA256.Create();
            var computed = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hex = BitConverter.ToString(computed).Replace("-", string.Empty, StringComparison.Ordinal);
            return string.Equals(hex, hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Backward-compatible alias for <see cref="Hash(string)"/>.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <returns>Hash string in format "PBKDF2$iterations$saltBase64$keyBase64".</returns>
        public static string HashPassword(string password) => Hash(password);

        /// <summary>
        /// Backward-compatible alias for <see cref="Verify(string, string)"/>.
        /// </summary>
        /// <param name="password">Plain text password to check.</param>
        /// <param name="hash">Stored hash (PBKDF2 format or legacy SHA-256 hex).</param>
        /// <returns><see langword="true"/> if the password matches; otherwise, <see langword="false"/>.</returns>
        public static bool VerifyPassword(string password, string hash) => Verify(password, hash);
    }
}
