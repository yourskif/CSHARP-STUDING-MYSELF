// Path: C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreBLL\Models\UserModel.cs
namespace StoreBLL.Models
{
    /// <summary>
    /// Business-layer user model.
    /// Mirrors essential user fields exposed to upper layers.
    /// </summary>
    public class UserModel : AbstractModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets stores the password hash that comes from DAL.User.Password.
        /// Avoid using plain-text passwords.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the user is blocked from authenticating.
        /// </summary>
        public bool IsBlocked { get; set; }
    }
}
