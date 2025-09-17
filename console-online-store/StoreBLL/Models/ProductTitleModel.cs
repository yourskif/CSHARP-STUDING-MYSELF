namespace StoreBLL.Models
{
    /// <summary>
    /// Product title (catalog item without SKU/price).
    /// </summary>
    public class ProductTitleModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTitleModel"/> class.
        /// </summary>
        public ProductTitleModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductTitleModel"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="title">Title text.</param>
        /// <param name="manufacturerId">Manufacturer identifier.</param>
        public ProductTitleModel(int id, string title, int manufacturerId)
        {
            this.Id = id;
            this.Title = title;
            this.ManufacturerId = manufacturerId;
        }

        /// <summary>
        /// Gets or sets product title text.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets manufacturer identifier.
        /// </summary>
        public int ManufacturerId { get; set; }
    }
}
