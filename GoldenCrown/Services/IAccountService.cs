namespace GoldenCrown.Services
{
    public interface IAccountService
    {
        public Task CreateAccountAsync(int userId);
    }
}
