using GoldenCrown.Domain.Common;

namespace GoldenCrown.Domain.Entities
{
    public class Account
    {
        private Account() { }

        public static Account Open(int userId, int currencyId) => new()
        {
            UserId = userId,
            CurrencyId = currencyId,
            Balance = 0m,
        };

        public int Id { get; private set; }
        public int UserId { get; private set; }
        public int CurrencyId { get; private set; }
        public decimal Balance { get; private set; }

        public Result Withdraw(decimal amount)
        {
            if(amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Сумма должна быть положительной.");
            } 
            if (Balance - amount < 0)
            {
                return Result.Failure("Недостаточно средств.");
            }
            Balance -= amount;
            return Result.Success();
        }
        public void Deposit(decimal amount) 
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Сумма должна быть положительной.");
            }
            Balance += amount;
        }
    }
}
