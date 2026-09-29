using DDDCqrsEs.Domain.Models;
using System;

namespace DDDCqrsEs.Domain.Events
{
    public class StockCreated : BaseEvent
    {
        public StockCreated(Guid id, string licensePlate, string item, string location, int quantityOnHand,
            string status, string lot, DateTime bestBeforeDate, string countryOfOrigin)
            : base(nameof(StockCreated), id, CreateStock(licensePlate, item, location, quantityOnHand, status, lot, bestBeforeDate, countryOfOrigin)) { }

        [Newtonsoft.Json.JsonConstructor]
        public StockCreated(StockModel stock, Guid aggregateId, int version, DateTime timeStamp)
            : base(nameof(StockCreated), aggregateId, stock, version, timeStamp) { }

        private static StockModel CreateStock(string licensePlate, string item, string location, int quantityOnHand,
            string status, string lot, DateTime bestBeforeDate, string countryOfOrigin) => new StockModel
        {
            LicensePlate = licensePlate, Item = item, Location = location, QuantityOnHand = quantityOnHand,
            Status = status, Lot = lot, BestBeforeDate = bestBeforeDate, CountryOfOrigin = countryOfOrigin
        };
    }

    public class StockUpdated : BaseEvent
    {
        public StockUpdated(Guid id, string licensePlate, string item, string location, int quantityOnHand,
            string status, string lot, DateTime bestBeforeDate, string countryOfOrigin)
            : base(nameof(StockUpdated), id, CreateStock(licensePlate, item, location, quantityOnHand, status, lot, bestBeforeDate, countryOfOrigin)) { }

        [Newtonsoft.Json.JsonConstructor]
        public StockUpdated(StockModel stock, Guid aggregateId, int version, DateTime timeStamp)
            : base(nameof(StockUpdated), aggregateId, stock, version, timeStamp) { }

        private static StockModel CreateStock(string licensePlate, string item, string location, int quantityOnHand,
            string status, string lot, DateTime bestBeforeDate, string countryOfOrigin) => new StockModel
        {
            LicensePlate = licensePlate, Item = item, Location = location, QuantityOnHand = quantityOnHand,
            Status = status, Lot = lot, BestBeforeDate = bestBeforeDate, CountryOfOrigin = countryOfOrigin
        };
    }

    public class StockDeleted : BaseEvent
    {
        public StockDeleted(Guid id) : base(nameof(StockDeleted), id) { }

        [Newtonsoft.Json.JsonConstructor]
        public StockDeleted(Guid aggregateId, int version, DateTime timeStamp)
            : base(nameof(StockDeleted), aggregateId, null, version, timeStamp) { }
    }
}
