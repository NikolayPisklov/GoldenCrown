namespace GoldenCrown.Services
{
    public interface IUserService
    {
        Task<string> LoginAsync(string login, string password);
        public Task<bool> RegisterAsync(string login, string password, string name);
    }
}
