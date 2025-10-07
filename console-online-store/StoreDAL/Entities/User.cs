// Path: console-online-store/StoreDAL/Entities/User.cs
namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Application user (admin or registered/guest).
/// Table structure matches TZ diagram exactly.
/// </summary>
[Table("users")]
public class User : BaseEntity
{
    public User()
        : base()
    {
    }

    public User(int id, string name, string lastName, string login, string password, int roleId)
        : base(id)
    {
        this.Name = name;
        this.LastName = lastName;
        this.Login = login;
        this.Password = password;
        this.RoleId = roleId;
    }

    /// <summary>
    /// Gets or sets user's first name.
    /// Database column: first_name (per TZ diagram)
    /// </summary>
    [Column("first_name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's last name.
    /// Database column: last_name
    /// </summary>
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user's login (username).
    /// Database column: login
    /// </summary>
    [Column("login")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets password hash (stored as PBKDF2 hash in DB).
    /// Database column: password
    /// </summary>
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets user role ID (foreign key to user_roles).
    /// Database column: user_role_id (per TZ diagram)
    /// </summary>
    [Column("user_role_id")]
    public int RoleId { get; set; }

    /// <summary>
    /// Gets or sets whether the user is blocked from accessing the system.
    /// Database column: is_blocked
    /// </summary>
    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Gets or sets the collection of orders placed by this user.
    /// Navigation property for EF Core relationship.
    /// </summary>
    public virtual IList<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
}
