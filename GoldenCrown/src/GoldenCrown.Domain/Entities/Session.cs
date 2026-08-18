namespace GoldenCrown.Domain.Entities
{
    public class Session
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
