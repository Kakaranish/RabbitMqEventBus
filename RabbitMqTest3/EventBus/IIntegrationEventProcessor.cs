using RabbitMQ.Client.Events;
using System.Threading.Tasks;

namespace RabbitMqTest3.EventBus
{
    public interface IIntegrationEventProcessor
    {
        Task<bool> Process(BasicDeliverEventArgs eventMessage);
    }
}
