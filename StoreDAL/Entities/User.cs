namespace StoreDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToDo: add atribute here
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

    // ToDo: add atribute here
    public string Name { get; set; }

    // ToDo: add atribute here
    public string LastName { get; set; }

    // ToDo: add atribute here
    public string Login { get; set; }

    // ToDo: add atribute here
    public string Password { get; set; }

    // ToDo: add atribute here
    public int RoleId { get; set; }

    public UserRole Role { get; set; }

    public virtual IList<CustomerOrder> Order { get; set; }
}
