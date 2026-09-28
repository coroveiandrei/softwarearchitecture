using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Application.Common
{
    public class EventMapper
    {
        public static BaseEvent ConvertFromStoredEvent(StoredEvent storedEvent)
        {
            var stockData = JsonConvert.DeserializeObject<StockModel>(storedEvent.Data);

            switch (storedEvent.EventType)
            {
                case "StockCreated":
                    return new StockCreated(stockData, storedEvent.AggregateId, storedEvent.Version, storedEvent.TimeStamp);
                case "StockUpdated":
                    return new StockUpdated(stockData, storedEvent.AggregateId, storedEvent.Version, storedEvent.TimeStamp);
                case "StockDeleted":
                    return new StockDeleted(storedEvent.AggregateId, storedEvent.Version, storedEvent.TimeStamp);
                default:
                    throw new InvalidOperationException($"Unknown event type '{storedEvent.EventType}'.");
            }
        }

        public static List<BaseEvent> ConvertStoredEvents(IEnumerable<StoredEvent> storedEvents)
        {
            List<BaseEvent> baseEvents = new List<BaseEvent>();
            foreach(var storedEvent in storedEvents)
            {
                baseEvents.Add(ConvertFromStoredEvent(storedEvent));
            }
            return baseEvents;
        }

    }
}
