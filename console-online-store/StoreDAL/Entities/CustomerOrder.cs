namespace StoreDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToDo: add atribute here
public class CustomerOrder : BaseEntity
{
    public CustomerOrder()
     : base()
    {
    }

    public CustomerOrder(int id, string operationTime, int userId, int orderStateId)
        : base(id)
    {
        this.OperationTime = operationTime;
        this.UserId = userId;
        this.OrderStateId = orderStateId;
    }

    // ToDo: add atribute here
    public int UserId { get; set; }

    // ToDo: add atribute here
    public string OperationTime { get; set; }

    // ToDo: add atribute here
    public int OrderStateId { get; set; }

    public User User { get; set; }

    public OrderState State { get; set; }

    public virtual IList<OrderDetail> Details { get; set; }
}