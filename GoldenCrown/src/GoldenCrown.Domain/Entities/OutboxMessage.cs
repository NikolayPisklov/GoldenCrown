namespace GoldenCrown.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? Error { get; set; }

        public static OutboxMessage CreateOutboxMessage(Guid id, string type, string payload, DateTime createdAt) => new()
        {
            Id = id,
            Type = type,
            Payload = payload,
            CreatedAt = createdAt
        };
    }
}
