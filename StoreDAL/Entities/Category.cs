// Path: console-online-store/StoreDAL/Entities/Category.cs
namespace StoreDAL.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product category entity.
/// Table structure matches TZ diagram exactly.
/// </summary>
[Table("categories")]
public class Category : BaseEntity
{
    public Category()
        : base()
    {
    }

    public Category(int id, string name)
        : base(id)
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets or sets the category name.
    /// Database column: category_name (per TZ diagram)
    /// </summary>
    [Column("category_name")]
    public string? Name { get; set; }
}
