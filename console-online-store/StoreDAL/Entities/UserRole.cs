namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user_roles")]
public class UserRole : BaseEntity
{
    public UserRole() : base()
    {
    }

    public UserRole(int id, string name) : base(id)
    {
        this.Name = name;
    }

    [Column("role_name")]
    [Required]
    public string Name { get; set; }

    // Alias expected by BLL
    [NotMapped]
    public string RoleName
    {
        get => this.Name;
        set => this.Name = value;
    }

    public virtual IList<User> Users { get; set; }
}
