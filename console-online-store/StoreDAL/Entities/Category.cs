namespace StoreDAL.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product category.
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

    [Column("name")]
    public string? Name { get; set; }
}
