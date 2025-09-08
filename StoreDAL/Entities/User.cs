namespace StoreDAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User : BaseEntity
{
    public User() : base()
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

    [Column("first_name")]
    public string Name { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("login")]
    public string Login { get; set; }

    [Column("password")]
    public string Password { get; set; }

    [Column("user_role_id")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public UserRole Role { get; set; }

    public virtual IList<CustomerOrder> Orders { get; set; }
}