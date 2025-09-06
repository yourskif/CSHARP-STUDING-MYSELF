namespace StoreBLL.Models
{
    /// <summary>
    /// Customer order aggregate.
    /// </summary>
    public class CustomerOrderModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderModel"/> class.
        /// </summary>
        public CustomerOrderModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOrderModel"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="userId">User identifier.</param>
        /// <param name="orderStateId">Order state identifier.</param>
        /// <param name="operationTime">Operation time.</param>
        public CustomerOrderModel(int id, int userId, int orderStateId, string operationTime)
            : base(id)
        {
            this.UserId = userId;
            this.OrderStateId = orderStateId;
            this.OperationTime = operationTime;
        }

        /// <summary>
        /// Gets or sets user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets order state identifier.
        /// </summary>
        public int OrderStateId { get; set; }

        /// <summary>
        /// Gets or sets order operation time.
        /// </summary>
        public string OperationTime { get; set; } = string.Empty;
    }
}
