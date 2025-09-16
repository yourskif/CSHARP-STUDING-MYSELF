namespace StoreBLL.Models
{
    /// <summary>
    /// Lightweight product model for list views.
    /// </summary>
    public class ProductMinModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductMinModel"/> class.
        /// </summary>
        public ProductMinModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductMinModel"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="title">Product title text.</param>
        /// <param name="category">Category model.</param>
        /// <param name="manufacturer">Manufacturer model.</param>
        /// <param name="price">Unit price.</param>
        /// <param name="stock">Units in stock.</param>
        public ProductMinModel(
            int id,
            string title,
            CategoryModel category,
            ManufacturerModel manufacturer,
            decimal price,
            int stock)
        {
            this.Id = id;
            this.Title = title;
            this.Category = category;
            this.Manufacturer = manufacturer;
            this.Price = price;
            this.Stock = stock;
        }

        /// <summary>
        /// Gets or sets product title text.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets category.
        /// </summary>
        public CategoryModel Category { get; set; } = new CategoryModel();

        /// <summary>
        /// Gets or sets manufacturer.
        /// </summary>
        public ManufacturerModel Manufacturer { get; set; } = new ManufacturerModel();

        /// <summary>
        /// Gets or sets unit price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets units in stock.
        /// </summary>
        public int Stock { get; set; }
    }
}
