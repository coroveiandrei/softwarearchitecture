using System;
using System.Collections.Generic;
using System.Text;
using InventoryService.Domain.Models;
using InventoryService.Domain.Entities.Base;
using InventoryService.Domain.Events;

namespace InventoryService.Domain.Projections
{
    public class StockProjection
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public int QuantityOnHand { get; set; }
        public string Status { get; set; }
        public string Lot { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public string CountryOfOrigin { get; set; }
        public int Version { get; set; }
    }
}
