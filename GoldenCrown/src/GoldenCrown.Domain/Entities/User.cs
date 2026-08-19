namespace GoldenCrown.Domain.Entities
{
    public class User
    {
        private User() { }

        public static User Register(string login, string name, string password) => new()
        {
            Login = login,
            Name = name,
            Password = password
        };

        public int Id { get; private set; }
        public string Login { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string Password { get; private set; } = null!;
    }
}
