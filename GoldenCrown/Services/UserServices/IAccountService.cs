namespace GoldenCrown.Services.UserServices
{
    public interface IAccountService
    {
        public Task CreateAccountAsync(int userId);
    }
}
