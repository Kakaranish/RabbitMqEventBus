using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace RabbitMqTest3.EventBus
{
    public class PersistentRabbitMqConnection : IPersistentRabbitMqConnection, IDisposable
    {
        private readonly object _lockObject = new object();
        private readonly ILogger<IPersistentRabbitMqConnection> _logger;
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;

        public PersistentRabbitMqConnection(ILogger<IPersistentRabbitMqConnection> logger, IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public bool TryConnect()
        {
            if (IsDisposed)
            {
                _logger.LogWarning("Connection is already disposed");
                return false;
            }

            if (IsConnected)
            {
                _logger.LogWarning("Connection is already established");
                return false;
            }

            lock (_lockObject)
            {
                _connection = _connectionFactory.CreateConnection();
                ConfigureConnectionCallbacks();
                
                return true;
            }
        }

        private void ConfigureConnectionCallbacks()
        {
            _connection.ConnectionShutdown += (sender, args) =>
            {
                _logger.LogWarning("Connection shutdown occured. Trying to reconnect...");
                TryConnect();
            };

            _connection.CallbackException += (sender, args) =>
            {
                _logger.LogWarning("Callback exception occured. Trying to reconnect...");
                TryConnect();
            };
        }

        public IModel CreateChannel()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Failed to create channel - connection is not established");
            }

            return _connection.CreateModel();
        }

        public bool IsConnected => _connection?.IsOpen ?? false;
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                _logger.LogWarning("Connection is already disposed");
                return;
            }

            IsDisposed = true;

            _connection?.Dispose();
        }
    }
}
