namespace GoldenCrown.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;

        public Session Session { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = null!;
    }
}
