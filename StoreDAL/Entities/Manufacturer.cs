// Path: console-online-store/StoreDAL/Entities/Manufacturer.cs
namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product manufacturer entity.
/// Table structure matches TZ diagram exactly.
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

    /// <summary>
    /// Gets or sets the manufacturer name.
    /// Database column: manufacturer_name (per TZ diagram)
    /// </summary>
    [Column("manufacturer_name")]
    public string? Name { get; set; }

    /// <summary>
    /// Navigation property to Products collection.
    /// </summary>
    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
