using System;
using System.Collections.Generic;

namespace RabbitMqTest3.EventBus
{
    public class EventSubscription : IEqualityComparer<EventSubscription>
    {
        public string EventName { get; }
        public Type EventType { get; }

        public EventSubscription(string eventName, Type eventType)
        {
            EventName = eventName;
            EventType = eventType;
        }

        protected bool Equals(EventSubscription other)
        {
            return EventName == other.EventName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventSubscription) obj);
        }

        public override int GetHashCode()
        {
            return EventName != null ? EventName.GetHashCode() : 0;
        }

        public bool Equals(EventSubscription x, EventSubscription y)
        {
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode(EventSubscription obj)
        {
            return obj.GetHashCode();
        }
    }
}