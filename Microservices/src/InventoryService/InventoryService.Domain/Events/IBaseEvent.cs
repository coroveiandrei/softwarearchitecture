using MediatR;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Domain.Events
{
    public interface IBaseEvent : IRequest
    {
        public DateTime TimeStamp { get; }
        public string EventType { get; set; }
        public Guid AggregateId { get; set; }
        public StockModel Stock { get; set; }
        public int Version { get; set; }
    }
}
