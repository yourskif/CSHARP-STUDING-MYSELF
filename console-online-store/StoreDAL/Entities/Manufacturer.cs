namespace StoreDAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StoreDAL.Entities;

// ToDo: add atribute here
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

    // ToDo: add atribute here
    public string Name { get; set; }
}
