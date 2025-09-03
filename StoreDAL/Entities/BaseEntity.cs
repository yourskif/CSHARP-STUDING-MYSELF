namespace StoreDAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseEntity
{
    protected BaseEntity(int id)
    {
        this.Id = id;
    }

    protected BaseEntity()
    {
        this.Id = 0;
    }

    [Column("id")]
    public int Id { get; set; }
}
