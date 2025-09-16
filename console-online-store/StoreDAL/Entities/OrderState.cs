namespace StoreDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Table("order_states")]
public class OrderState : BaseEntity
{
    public OrderState()
        : base()
    {
    }

    public OrderState(int id, string stateName)
        : base(id)
    {
        this.StateName = stateName;
    }

    [Column("state_name")]
    public string StateName { get; set; }

    public virtual IList<CustomerOrder> Order { get; set; }
}
