namespace RabbitMqTest3.EventBus
{
    public class RabbitMqConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
    }
}
