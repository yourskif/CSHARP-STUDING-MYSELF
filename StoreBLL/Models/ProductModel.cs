namespace StoreBLL.Models
{
    /// <summary>
    /// Full product model (concrete item with SKU/price/stock).
    /// </summary>
    public class ProductModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel"/> class.
        /// </summary>
        public ProductModel()
        {
        }

        /// <summary>
        /// Initializes a new instance with model-typed category/manufacturer.
        /// </summary>
        public ProductModel(
            int id,
            string title,
            CategoryModel category,
            ManufacturerModel manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id)
        {
            this.Title = title;
            this.Category = category;
            this.Manufacturer = manufacturer;
            this.Sku = sku;
            this.Description = description;
            this.Price = price;
            this.Stock = stock;
        }

        /// <summary>
        /// Initializes a new instance with string category/manufacturer (for legacy callers).
        /// </summary>
        public ProductModel(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id)
        {
            this.Title = title;
            this.Category = new CategoryModel { Name = category };
            this.Manufacturer = new ManufacturerModel { Name = manufacturer };
            this.Sku = sku;
            this.Description = description;
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
        /// Gets or sets SKU.
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

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
