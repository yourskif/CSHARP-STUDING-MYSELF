namespace StoreBLL.Models
{
    /// <summary>
    /// User role (e.g. Admin, User).
    /// </summary>
    public class UserRoleModel : AbstractModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleModel"/> class.
        /// </summary>
        public UserRoleModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleModel"/> class with values.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="roleName">Role display name.</param>
        public UserRoleModel(int id, string roleName)
            : base(id)
        {
            this.RoleName = roleName;
        }

        /// <summary>
        /// Gets or sets role display name.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
    }
}
