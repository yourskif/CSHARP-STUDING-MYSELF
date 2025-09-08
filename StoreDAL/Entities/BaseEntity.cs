using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreDAL.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ⚡️ автоінкремент
        [Column("id")]
        public int Id { get; set; }

        protected BaseEntity()
        {
        }

        protected BaseEntity(int id)
        {
            this.Id = id;
        }
    }
}
