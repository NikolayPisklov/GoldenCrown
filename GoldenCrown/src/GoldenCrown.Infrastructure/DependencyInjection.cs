using GoldenCrown.Application.Abstractions;
using GoldenCrown.Infrastructure.BackgroundServices;
using GoldenCrown.Infrastructure.Messaging.RabbitMQ;
using GoldenCrown.Infrastructure.Persistence;
using GoldenCrown.Infrastructure.Services.ExchangeRate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace GoldenCrown.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<GoldenCrownDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("GoldenCrownDbConnection"));
            });
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<GoldenCrownDbContext>());

            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
            services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

            services.AddHostedService<SessionCleanupService>();

            services.AddHttpClient();

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1000;
                options.CompactionPercentage = 0.25;
            });

            services.AddSingleton(_ =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
            services.AddSingleton<IConnectionMultiplexer>(sp => sp.GetRequiredService<ConnectionMultiplexer>());

            services.AddSingleton<IDistributedLockFactory>(sp =>
                RedLockFactory.Create(new List<RedLockMultiplexer>
                {
                    sp.GetRequiredService<ConnectionMultiplexer>()
                })
            );
            
            services.AddScoped<CurrescyExchangeRateProvider>();
            services.AddScoped<IExchangeRateProvider>(sp => ActivatorUtilities.CreateInstance<CachedExchangeRateProvider>(
                sp, sp.GetRequiredService<CurrescyExchangeRateProvider>()));

            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.Configuration = configuration.GetConnectionString("Redis");
            });

            return services;
        }
    }
}
