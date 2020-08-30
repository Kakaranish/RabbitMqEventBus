using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace RabbitMqTest3.EventBus
{
    public class EventBusRabbitMq : IEventBus
    {
        private readonly IPersistentRabbitMqConnection _persistentRabbitMqConnection;
        private readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly RabbitMqConfig _rabbitMqConfig;
        
        private IModel _consumerChannel;

        private bool _exchangeDeclared;
        private bool _queueDeclared;

        public EventBusRabbitMq(IPersistentRabbitMqConnection persistentRabbitMqConnection, IEventBusSubscriptionManager eventBusSubscriptionManager,
            IOptionsMonitor<RabbitMqConfig> rabbitMqConfig, ILogger<EventBusRabbitMq> logger)
        {
            _persistentRabbitMqConnection = persistentRabbitMqConnection ?? throw new ArgumentNullException(nameof(_persistentRabbitMqConnection));
            _eventBusSubscriptionManager = eventBusSubscriptionManager ?? throw new ArgumentNullException(nameof(_eventBusSubscriptionManager));
            _rabbitMqConfig = rabbitMqConfig?.CurrentValue ?? throw new ArgumentNullException(nameof(rabbitMqConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Publish(IntegrationEventBase @event)
        {
            EnsureConnected();

            using (var publisherChannel = _persistentRabbitMqConnection.CreateChannel())
            {
                var eventType = @event.GetType().Name;
                var message = JsonConvert.SerializeObject(@event);
                var messageBody = Encoding.UTF8.GetBytes(message);

                publisherChannel.BasicPublish(
                    exchange: _rabbitMqConfig.ExchangeName,
                    routingKey: eventType,
                    mandatory: true,
                    body: messageBody);
            }
        }

        public void Subscribe<TEvent, TEventHandler>() 
            where TEvent : IntegrationEventBase 
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            EnsureConnected();
            
            var subscriptionAdded = _eventBusSubscriptionManager.AddSubscription<TEvent, TEventHandler>();
            if (!subscriptionAdded)
            {
                return;
            }

            using (var channel = _persistentRabbitMqConnection.CreateChannel())
            {
                EnsureExchangeDeclared(channel);
                EnsureQueueDeclared(channel);

                var eventName = typeof(TEvent).Name;
                BindQueueToRoutingKey(channel, eventName);   
            }
        }

        public void StartConsuming()
        {
            if (IsConsuming)
            {
                return;
            }

            EnsureConnected();
            EnsureConsumerChannelCreated();

            EnsureExchangeDeclared(_consumerChannel);
            EnsureQueueDeclared(_consumerChannel);

            if (_consumerChannel == null)
            {
                InitializeConsumerChannel();
            }

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
            consumer.Received += ConsumerOnReceived;

            _consumerChannel.BasicConsume(_rabbitMqConfig.QueueName, false, consumer);

            IsConsuming = true;
        }

        public bool IsConsuming { get; private set; }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs @event)
        {
            // Processing event
        }

        private void EnsureConnected()
        {
            if (_persistentRabbitMqConnection.IsConnected)
            {
                return;
            }

            if (_persistentRabbitMqConnection.IsDisposed)
            {
                throw new InvalidOperationException("Unable to connect RabbitMq - connection is disposed");
            }

            _persistentRabbitMqConnection.TryConnect();
        }

        private void EnsureConsumerChannelCreated()
        {
            if (_consumerChannel != null)
            {
                return;
            }

            InitializeConsumerChannel();
        }

        private void InitializeConsumerChannel()
        {
            _consumerChannel = _persistentRabbitMqConnection.CreateChannel();
            _consumerChannel.CallbackException += (sender, eventArgs) =>
            {
                _logger.LogError($"Consumer channel callback failed");
                _consumerChannel.Dispose();

                InitializeConsumerChannel();

                if (IsConsuming)
                {
                    IsConsuming = false;
                    StartConsuming();
                }
            };
        }

        private void EnsureExchangeDeclared(IModel channel)
        {
            if (_exchangeDeclared)
            {
                return;
            }
            
            DeclareExchange(channel);
            _exchangeDeclared = true;
        }

        private void DeclareExchange(IModel channel)
        {
            channel.ExchangeDeclare(
                exchange: _rabbitMqConfig.ExchangeName,
                type: "direct");
        }

        private void EnsureQueueDeclared(IModel channel)
        {
            if (_queueDeclared)
            {
                return;
            }

            DeclareQueue(channel);
            _queueDeclared = true;
        }

        private void DeclareQueue(IModel channel)
        {
            channel.QueueDeclare(
                queue: _rabbitMqConfig.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        private void BindQueueToRoutingKey(IModel channel, string routingKey)
        {
            channel.QueueBind(
                queue: _rabbitMqConfig.QueueName,
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: routingKey);
        }
    }
}
