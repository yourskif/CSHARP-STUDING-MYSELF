namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Application user (admin or registered/guest).
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

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("login")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets stored as hash in the DB (seeded via hashing step).
    /// </summary>
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("role_id")]
    public int RoleId { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    public virtual IList<CustomerOrder> Orders { get; set; } = new List<CustomerOrder>();
}
