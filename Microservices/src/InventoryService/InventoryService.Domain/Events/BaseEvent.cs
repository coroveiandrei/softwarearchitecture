using Newtonsoft.Json;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Domain.Events
{
    public class BaseEvent : IBaseEvent
    {
        public DateTime TimeStamp { get; private set; }
        public string EventType { get; set; }
        public Guid AggregateId { get; set; }
        public StockModel Stock { get; set; }
        public int Version { get; set; }

        public BaseEvent()
        {
            TimeStamp = DateTime.Now;
        }

        public BaseEvent(string eventType)
        {
            TimeStamp = DateTime.Now;
            EventType = eventType;
        }
    }
}
