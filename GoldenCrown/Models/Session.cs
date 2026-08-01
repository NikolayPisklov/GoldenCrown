namespace GoldenCrown.Models
{
    public class Session
    {
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
