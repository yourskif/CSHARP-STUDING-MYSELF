namespace StoreBLL.Models
{
    /// <summary>
    /// Category reference model.
    /// </summary>
    public class CategoryModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryModel"/> class.
        /// </summary>
        public CategoryModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryModel"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="name">Category name.</param>
        public CategoryModel(int id, string name)
            : base(id)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets category name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Implicit conversion from string to <see cref="CategoryModel"/> (maps to <see cref="Name"/>).
        /// Lets you pass a plain string where a CategoryModel is expected.
        /// </summary>
        /// <param name="name">Category name.</param>
        public static implicit operator CategoryModel(string name) => new CategoryModel { Name = name };
    }
}
