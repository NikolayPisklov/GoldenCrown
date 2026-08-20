using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace GoldenCrown.Api.RabbitMQ
{
    public interface IRabbitMqConnectionManager
    {
        Task<IChannel> CreateChannelAsync(CancellationToken token);
    }
    public class RabbitMqConnectionManager : IRabbitMqConnectionManager, IAsyncDisposable
    {
        private readonly RabbitMqSettings _settings;
        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public RabbitMqConnectionManager(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IChannel> CreateChannelAsync(CancellationToken token)
        {
            var connection = await GetConnectionAsync(token);
            return await connection.CreateChannelAsync(cancellationToken: token);
        }
        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
                await _connection.DisposeAsync();
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken token)
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }
            await _lock.WaitAsync(token);
            try
            {
                if (_connection is { IsOpen: true })
                {
                    return _connection;
                }
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true
                };
                _connection = await factory.CreateConnectionAsync(token);
                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
