using GoldenCrown.Application.Abstractions;
using GoldenCrown.Infrastructure.BackgroundServices;
using GoldenCrown.Infrastructure.Messaging.RabbitMQ;
using GoldenCrown.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            return services;
        }
    }
}
