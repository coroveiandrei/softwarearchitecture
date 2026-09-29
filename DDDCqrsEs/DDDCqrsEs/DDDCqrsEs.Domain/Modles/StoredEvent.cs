using System;

namespace DDDCqrsEs.Domain.Models
{
    public sealed class StoredEvent
    {
        public StoredEvent(Guid aggregateId, int version, string eventType, string data, DateTime timeStamp)
        {
            AggregateId = aggregateId;
            Version = version;
            EventType = eventType;
            Data = data;
            TimeStamp = timeStamp;
        }

        public Guid AggregateId { get; }
        public int Version { get; }
        public string EventType { get; }
        public string Data { get; }
        public DateTime TimeStamp { get; }
    }
}
