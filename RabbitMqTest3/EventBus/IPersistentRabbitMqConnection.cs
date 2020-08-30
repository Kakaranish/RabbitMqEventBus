using RabbitMQ.Client;

namespace RabbitMqTest3.EventBus
{
    public interface IPersistentRabbitMqConnection
    {
        bool TryConnect();
        IModel CreateChannel();
        bool IsConnected { get; }
        bool IsDisposed { get; }
    }
}
