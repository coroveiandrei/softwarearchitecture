using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DDDCqrsEs.Persistance.DataModel
{
    public class EventEntity : TableEntity
    {
        public EventEntity() { }

        public EventEntity(Guid aggregateId, int version)
            : base(aggregateId.ToString(), version.ToString("D10")) { }

        public DateTime TimeCreated { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
        public bool IsPublished { get; set; }
    }
}
