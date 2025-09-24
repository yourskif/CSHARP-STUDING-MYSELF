namespace StoreBLL.Models
{
    /// <summary>
    /// Base model for BLL layer with common identifier.
    /// </summary>
    public abstract class AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModel"/> class.
        /// </summary>
        protected AbstractModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractModel"/> class with id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        protected AbstractModel(int id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets entity identifier.
        /// </summary>
        public int Id { get; set; }
    }
}
