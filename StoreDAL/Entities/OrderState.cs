namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("order_states")]
public class OrderState : BaseEntity
{
    public OrderState() : base()
    {
    }

    public OrderState(int id, string name) : base(id)
    {
        this.Name = name;
    }

    [Column("state_name")]
    [Required]
    public string Name { get; set; }

    // Alias expected by BLL
    [NotMapped]
    public string StateName
    {
        get => this.Name;
        set => this.Name = value;
    }

    public virtual IList<CustomerOrder> Orders { get; set; }
}
