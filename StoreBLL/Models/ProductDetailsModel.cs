namespace StoreBLL.Models
{
    /// <summary>
    /// Detailed product model (розширює ProductModel).
    /// Використовується сервісом для повернення детальної інформації про товар.
    /// </summary>
    public class ProductDetailsModel : ProductModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDetailsModel"/> class.
        /// </summary>
        public ProductDetailsModel()
        {
        }

        /// <summary>
        /// Initializes a new instance with model-typed category/manufacturer.
        /// </summary>
        public ProductDetailsModel(
            int id,
            string title,
            CategoryModel category,
            ManufacturerModel manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id, title, category, manufacturer, sku, description, price, stock)
        {
        }

        /// <summary>
        /// Initializes a new instance with string category/manufacturer (для сумісності зі старими викликами).
        /// </summary>
        public ProductDetailsModel(
            int id,
            string title,
            string category,
            string manufacturer,
            string sku,
            string description,
            decimal price,
            int stock)
            : base(id, title, category, manufacturer, sku, description, price, stock)
        {
        }
    }
}
