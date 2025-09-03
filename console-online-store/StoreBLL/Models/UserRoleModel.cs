namespace StoreBLL.Models;
using System;
using System.Collections.Generic;

public class UserRoleModel : AbstractModel
{
    public UserRoleModel(int id, string roleName)
        : base(id)
    {
        this.Id = id;
        this.RoleName = roleName;
    }

    public string RoleName { get; set; }

    public override string ToString()
    {
        return $"Id:{this.Id} {this.RoleName}";
    }
}
