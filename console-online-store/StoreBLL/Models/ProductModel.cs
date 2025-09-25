// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Models\ProductModel.csnamespace StoreBLL.Models
{
    /// <summary>
    /// Full product model with reservation support.
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
        /// Initializes a new instance of the <see cref="ProductModel"/> class.
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
            int stock,
            int reserved = 0)
            : base(id)
        {
            this.Title = title;
            this.Category = category;
            this.Manufacturer = manufacturer;
            this.Sku = sku;
            this.Description = description;
            this.Price = price;
            this.Stock = stock;
            this.Reserved = reserved;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel"/> class.
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
            int stock,
            int reserved = 0)
            : base(id)
        {
            this.Title = title;
            this.Category = new CategoryModel { Name = category };
            this.Manufacturer = new ManufacturerModel { Name = manufacturer };
            this.Sku = sku;
            this.Description = description;
            this.Price = price;
            this.Stock = stock;
            this.Reserved = reserved;
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
        /// Gets or sets total units in stock.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Gets or sets reserved units (in active orders).
        /// </summary>
        public int Reserved { get; set; }

        /// <summary>
        /// Gets available units (Stock - Reserved).
        /// </summary>
        public int Available => this.Stock - this.Reserved;
    }
}