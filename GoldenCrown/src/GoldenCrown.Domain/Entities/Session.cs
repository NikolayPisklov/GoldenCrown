namespace GoldenCrown.Domain.Entities
{
    public class Session
    {
        private Session() { }
        
        private static readonly TimeSpan Lifetime = TimeSpan.FromHours(1);
        public static Session Start(int userId, string token, DateTime utcNow) => new()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = utcNow + Lifetime
        };

        public void Refresh(string token, DateTime utcNow)
        {
            Token = token;
            ExpiresAt = utcNow + Lifetime;
        }

        public bool IsExpired(DateTime utcNow) => ExpiresAt < utcNow;
        
        public int UserId { get; private set; }
        public string Token { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
    }
}
