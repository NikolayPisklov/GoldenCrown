namespace GoldenCrown.Domain.Entities
{
    public class Currency
    {
        private Currency() { }
        public static Currency Create(int id, string name) => new()
        {
            Id = id,
            Name = name
        };

        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
    }
}
