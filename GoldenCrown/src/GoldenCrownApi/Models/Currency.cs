namespace GoldenCrownApi.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public IEnumerable<Account> Accounts { get; set; } = new List<Account>();
    }
}
