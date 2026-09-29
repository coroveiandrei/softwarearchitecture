using Newtonsoft.Json;
using DDDCqrsEs.Domain.Entities;
using DDDCqrsEs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDDCqrsEs.Domain.Events
{
    public class BaseEvent : IBaseEvent
    {
        public DateTime TimeStamp { get; private set; }
        public string EventType { get; private set; }
        public Guid AggregateId { get; private set; }
        public StockModel Stock { get; private set; }
        public int Version { get; private set; }

        public BaseEvent()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public BaseEvent(string eventType)
        {
            TimeStamp = DateTime.UtcNow;
            EventType = eventType;
        }

        protected BaseEvent(string eventType, Guid aggregateId, StockModel stock, int version, DateTime timeStamp)
        {
            EventType = eventType;
            AggregateId = aggregateId;
            Stock = stock;
            Version = version;
            TimeStamp = timeStamp;
        }

        protected BaseEvent(string eventType, Guid aggregateId, StockModel stock = null)
            : this(eventType, aggregateId, stock, 0, DateTime.UtcNow)
        {
        }

        internal void AssignVersion(int version)
        {
            if (Version != 0)
            {
                throw new InvalidOperationException("An event version can only be assigned once.");
            }

            Version = version;
        }
    }
}
