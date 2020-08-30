using System;
using Newtonsoft.Json;

namespace RabbitMqTest3.IntegrationEvents.EventTypes
{
    public class IntegrationEventBase
    {
        [JsonConstructor]
        public IntegrationEventBase(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        [JsonProperty]
        public Guid Id { get; }
        
        [JsonProperty]
        public DateTime CreationDate { get; }
    }
}
