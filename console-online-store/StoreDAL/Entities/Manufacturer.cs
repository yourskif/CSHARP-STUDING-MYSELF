namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product manufacturer.
/// </summary>
[Table("manufacturers")]
public class Manufacturer : BaseEntity
{
    public Manufacturer()
        : base()
    {
    }

    public Manufacturer(int id, string name)
        : base(id)
    {
        this.Name = name;
    }

    [Column("name")]
    public string? Name { get; set; }

    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
