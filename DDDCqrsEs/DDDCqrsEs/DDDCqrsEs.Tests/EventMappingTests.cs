using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace DDDCqrsEs.Tests
{
    public class EventMappingTests
    {
        [Fact]
        public void Replay_restores_event_identity_version_and_timestamp()
        {
            var aggregateId = Guid.NewGuid();
            var timeStamp = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var stock = new StockModel { Item = "Coffee", QuantityOnHand = 12 };
            var storedEvent = new StoredEvent(aggregateId, 7, nameof(StockUpdated),
                JsonConvert.SerializeObject(stock), timeStamp);

            var result = EventMapper.ConvertFromStoredEvent(storedEvent);

            var updated = Assert.IsType<StockUpdated>(result);
            Assert.Equal(nameof(StockUpdated), updated.EventType);
            Assert.Equal(aggregateId, updated.AggregateId);
            Assert.Equal(7, updated.Version);
            Assert.Equal(timeStamp, updated.TimeStamp);
            Assert.Equal("Coffee", updated.Stock.Item);
        }

        [Fact]
        public void Events_do_not_expose_public_setters()
        {
            var mutableProperties = typeof(BaseEvent).GetProperties()
                .Where(property => property.SetMethod?.IsPublic == true)
                .Select(property => property.Name);

            Assert.Empty(mutableProperties);
        }

        [Fact]
        public void Domain_does_not_reference_Azure_Table_Storage()
        {
            var references = typeof(BaseEvent).Assembly.GetReferencedAssemblies()
                .Select(reference => reference.Name);

            Assert.DoesNotContain("Microsoft.WindowsAzure.Storage", references);
        }
    }
}
