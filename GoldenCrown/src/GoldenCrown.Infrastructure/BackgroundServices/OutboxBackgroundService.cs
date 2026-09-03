using GoldenCrown.Application.Abstractions;
using GoldenCrown.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedLockNet;
using System.Reflection;

namespace GoldenCrown.Infrastructure.BackgroundServices
{
    public class OutboxBackgroundService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan LockExpiry = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan LockWait = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan LockRetry = TimeSpan.FromMilliseconds(200);

        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IDistributedLockFactory _distributedLockFactory;
        public OutboxBackgroundService(IMessagePublisher messagePublisher, IServiceScopeFactory scopeFactory, ILogger<SessionCleanupService> logger, IDistributedLockFactory distributedLockFactory)
        {
            _messagePublisher = messagePublisher;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _distributedLockFactory = distributedLockFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync()) 
            {
                try
                {
                    await PublishOutboxMessage(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "An error occurred during outbox proccessing!");
                }
            }
        }

        private async Task PublishOutboxMessage(CancellationToken stoppingToken)
        {
            await using var redLock = await _distributedLockFactory.CreateLockAsync($"outboxMessagesProccessing", LockExpiry, LockWait, LockRetry, stoppingToken);
            if (!redLock.IsAcquired)
            {
                return;
            }
            var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<GoldenCrownDbContext>();
            var messages = await db.OutboxMessages
                .Where(x => x.SentAt == null)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(stoppingToken);
            foreach (var message in messages)
            {
                try
                {
                    await _messagePublisher.PublishAsync(message.Id, message.Payload, message.Type, stoppingToken);
                    message.SentAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during publishing to message broker!");
                    message.Error = ex.Message;
                }
            }
            await db.SaveChangesAsync();
        }
    }
}
