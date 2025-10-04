namespace StoreBLL.Models
{
    /// <summary>
    /// Manufacturer reference model.
    /// </summary>
    public class ManufacturerModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturerModel"/> class.
        /// </summary>
        public ManufacturerModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturerModel"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="name">Manufacturer name.</param>
        public ManufacturerModel(int id, string name)
            : base(id)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets manufacturer name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Implicit conversion from string to <see cref="ManufacturerModel"/> (maps to <see cref="Name"/>).
        /// Lets you pass a plain string where a ManufacturerModel is expected.
        /// </summary>
        /// <param name="name">Manufacturer name.</param>
        public static implicit operator ManufacturerModel(string name) => new ManufacturerModel { Name = name };
    }
}
