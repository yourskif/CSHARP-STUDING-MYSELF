namespace StoreDAL.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
        }

        protected BaseEntity(int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }
    }
}
