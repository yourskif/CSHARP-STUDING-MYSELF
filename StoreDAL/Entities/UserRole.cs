namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a user role in the system.
/// Defines access level and permissions for users.
/// </summary>
/// <remarks>
/// Common roles: Admin (1), Registered User (2), Guest (3).
/// </remarks>
[Table("user_roles")]
public class UserRole : BaseEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRole"/> class.
    /// Default constructor for EF Core.
    /// </summary>
    public UserRole()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRole"/> class with specified values.
    /// </summary>
    /// <param name="id">Unique identifier for the role.</param>
    /// <param name="roleName">Name of the role (e.g., "Admin", "Registered", "Guest").</param>
    public UserRole(int id, string roleName)
        : base(id)
    {
        this.RoleName = roleName;
    }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    /// <example>
    /// "Admin", "Registered", "Guest".
    /// </example>
    [Column("user_role_name")]
    public string RoleName { get; set; }

    /// <summary>
    /// Gets or sets the collection of users assigned to this role.
    /// Navigation property for EF Core relationship.
    /// </summary>
    public virtual IList<User> User { get; set; }
}
