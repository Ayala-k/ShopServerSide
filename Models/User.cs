namespace serverSide.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }

    public class UserLogIn
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
