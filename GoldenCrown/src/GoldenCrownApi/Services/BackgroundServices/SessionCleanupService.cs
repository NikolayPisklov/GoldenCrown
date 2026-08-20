using GoldenCrown.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Api.Services.BackgroundServices
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionCleanupService(ILogger<SessionCleanupService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<GoldenCrownDbContext>();
                try
                {
                    var count = await db.Sessions
                        .Where(x => x.ExpiresAt < DateTime.UtcNow)
                        .ExecuteDeleteAsync(stoppingToken);
                    if (count > 0)
                    {
                        _logger.LogInformation("{Count} expired sessions were removed.", count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning expired sessions.");
                }

            }
        }
    }
}
