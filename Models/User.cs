namespace serverSide.Models
{

    public enum Roles
    {
        Customer,
        Admin
    }
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Roles? Role { get; set; }
        public string Password { get; set; }
    }

    public class UserLogIn
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
