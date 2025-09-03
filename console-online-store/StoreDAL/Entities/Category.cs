namespace StoreDAL.Entities;
using StoreDAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ToDo: add atribute here
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

    // ToDo: add atribute here
    public string Name { get; set; }

    public virtual IList<ProductTitle> Titles { get; set; }
}
