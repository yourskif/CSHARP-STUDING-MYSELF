namespace StoreDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ToDo: add atribute here
public class Product : BaseEntity
{
    public Product()
        : base()
    {
    }

    public Product(int id, int titleId, int manufacturerId, string description, decimal price)
        : base(id)
    {
        this.TitleId = titleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description;
        this.UnitPrice = price;
    }

    // ToDo: add atribute here
    public int TitleId { get; set; }

    // ToDo: add atribute here
    public int ManufacturerId { get; set; }

    // ToDo: add atribute here
    public decimal UnitPrice { get; set; }

    // ToDo: add atribute here
    public string Description { get; set; }

    public ProductTitle Title { get; set; }

    public Manufacturer Manufacturer { get; set; }

    public virtual IList<OrderDetail> OrderDetails { get; set; }
}