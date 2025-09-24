namespace StoreBLL.Models
{
    /// <summary>
    /// Order state (e.g. Created, Paid, Shipped).
    /// </summary>
    public class OrderStateModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStateModel"/> class.
        /// </summary>
        public OrderStateModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStateModel"/> class with values.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="stateName">State display name.</param>
        public OrderStateModel(int id, string stateName)
            : base(id)
        {
            this.StateName = stateName;
        }

        /// <summary>
        /// Gets or sets state display name.
        /// </summary>
        public string StateName { get; set; } = string.Empty;
    }
}
