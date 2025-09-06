namespace StoreBLL.Models
{
    public class UserModel : AbstractModel
    {
        public UserModel() : base()
        {
        }

        public UserModel(int id, string firstName, string lastName, string login, string password, int roleId)
            : base(id)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Login = login;
            this.Password = password;
            this.RoleId = roleId;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, Name: {this.FirstName} {this.LastName}, Login: {this.Login}, RoleId: {this.RoleId}";
        }
    }
}