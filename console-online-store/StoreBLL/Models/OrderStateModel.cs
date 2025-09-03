namespace StoreBLL.Models;
using System;
using System.Collections.Generic;

public class OrderStateModel : AbstractModel
{
    public OrderStateModel(int id, string stateName)
        : base(id)
    {
        this.Id = id;
        this.StateName = stateName;
    }

    public string StateName { get; set; }

    public override string ToString()
    {
        return $"Id:{this.Id} {this.StateName}";
    }
}