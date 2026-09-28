using MediatR;
using DDDCqrsEs.Domain.Entities;
using DDDCqrsEs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDDCqrsEs.Domain.Events
{
    public interface IBaseEvent : IRequest
    {
        public DateTime TimeStamp { get; }
        public string EventType { get; }
        public Guid AggregateId { get; }
        public StockModel Stock { get; }
        public int Version { get; }
    }
}
