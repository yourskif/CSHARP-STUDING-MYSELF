namespace StoreBLL.Models
{
    /// <summary>
    /// Model for user registration.
    /// </summary>
    public class UserRegisterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegisterModel"/> class.
        /// </summary>
        public UserRegisterModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegisterModel"/> class.
        /// </summary>
        /// <param name="login">Login name.</param>
        /// <param name="password">Plain or hashed password (depending on pipeline).</param>
        public UserRegisterModel(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        /// <summary>
        /// Gets or sets login name.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
