namespace StoreBLL.Models
{
    /// <summary>
    /// Order line item.
    /// </summary>
    public class OrderDetailModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailModel"/> class.
        /// </summary>
        public OrderDetailModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailModel"/> class with values.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="orderId">Order identifier.</param>
        /// <param name="productId">Product identifier.</param>
        /// <param name="amount">Quantity of items.</param>
        /// <param name="price">Unit price.</param>
        public OrderDetailModel(int id, int orderId, int productId, int amount, decimal price)
            : base(id)
        {
            this.OrderId = orderId;
            this.ProductId = productId;
            this.Quantity = amount;
            this.UnitPrice = price;
        }

        /// <summary>
        /// Gets or sets order identifier.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets product identifier.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets unit price.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets quantity (alias for compatibility with services).
        /// </summary>
        public int ProductAmount
        {
            get => this.Quantity;
            set => this.Quantity = value;
        }

        /// <summary>
        /// Gets or sets unit price (alias for compatibility with services).
        /// </summary>
        public decimal Price
        {
            get => this.UnitPrice;
            set => this.UnitPrice = value;
        }
    }
}
