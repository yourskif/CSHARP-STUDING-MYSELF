// Path: console-online-store/StoreBLL/Services/ValidationService.cs
namespace StoreBLL.Services;

using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Centralized validation service for business rules.
/// Provides validation methods for common data types and business constraints.
/// </summary>
public static class ValidationService
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        RegexOptions.Compiled);

    private static readonly Regex LoginRegex = new Regex(
        @"^[a-zA-Z0-9_-]{3,20}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Validates email format.
    /// </summary>
    /// <param name="email">Email address to validate.</param>
    /// <returns>True if valid email format, otherwise false.</returns>
    public static bool IsValidEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Validates password strength.
    /// Requirements: 8+ characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character.
    /// </summary>
    /// <param name="password">Password to validate.</param>
    /// <returns>True if password meets requirements, otherwise false.</returns>
    public static bool IsValidPassword(string? password)
    {
        return !string.IsNullOrWhiteSpace(password) && PasswordRegex.IsMatch(password);
    }

    /// <summary>
    /// Gets password validation error message.
    /// </summary>
    /// <returns>Descriptive error message for password requirements.</returns>
    public static string GetPasswordRequirements()
    {
        return "Password must be at least 8 characters long and contain: " +
               "1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character (@$!%*?&)";
    }

    /// <summary>
    /// Validates login format.
    /// Requirements: 3-20 characters, alphanumeric with underscore and hyphen allowed.
    /// </summary>
    /// <param name="login">Login to validate.</param>
    /// <returns>True if valid login format, otherwise false.</returns>
    public static bool IsValidLogin(string? login)
    {
        return !string.IsNullOrWhiteSpace(login) && LoginRegex.IsMatch(login);
    }

    /// <summary>
    /// Validates product price.
    /// </summary>
    /// <param name="price">Price to validate.</param>
    /// <returns>True if price is positive and less than 1,000,000, otherwise false.</returns>
    public static bool IsValidPrice(decimal price)
    {
        return price > 0 && price < 1000000m;
    }

    /// <summary>
    /// Validates stock quantity.
    /// </summary>
    /// <param name="quantity">Quantity to validate.</param>
    /// <returns>True if quantity is non-negative and less than 1,000,000, otherwise false.</returns>
    public static bool IsValidQuantity(int quantity)
    {
        return quantity >= 0 && quantity < 1000000;
    }

    /// <summary>
    /// Validates user name (first name or last name).
    /// Requirements: 2-50 characters, no digits.
    /// </summary>
    /// <param name="name">Name to validate.</param>
    /// <returns>True if valid name, otherwise false.</returns>
    public static bool IsValidName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        if (name.Length < 2 || name.Length > 50)
        {
            return false;
        }

        return !name.Any(c => char.IsDigit(c));
    }

    /// <summary>
    /// Validates product title.
    /// Requirements: 3-200 characters.
    /// </summary>
    /// <param name="title">Title to validate.</param>
    /// <returns>True if valid title, otherwise false.</returns>
    public static bool IsValidProductTitle(string? title)
    {
        return !string.IsNullOrWhiteSpace(title) &&
               title.Length >= 3 &&
               title.Length <= 200;
    }

    /// <summary>
    /// Validates SKU code.
    /// Requirements: 3-50 characters, alphanumeric with hyphen allowed.
    /// </summary>
    /// <param name="sku">SKU to validate.</param>
    /// <returns>True if valid SKU, otherwise false.</returns>
    public static bool IsValidSku(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return false;
        }

        if (sku.Length < 3 || sku.Length > 50)
        {
            return false;
        }

        return sku.All(c => char.IsLetterOrDigit(c) || c == '-');
    }

    /// <summary>
    /// Validates category or manufacturer name.
    /// Requirements: 2-100 characters.
    /// </summary>
    /// <param name="name">Name to validate.</param>
    /// <returns>True if valid name, otherwise false.</returns>
    public static bool IsValidCategoryOrManufacturerName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               name.Length >= 2 &&
               name.Length <= 100;
    }
}
