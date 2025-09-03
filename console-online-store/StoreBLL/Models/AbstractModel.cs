namespace StoreBLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public abstract class AbstractModel
{
    protected AbstractModel(int id)
    {
        this.Id = id;
    }

    public int Id { get; set; }
}
