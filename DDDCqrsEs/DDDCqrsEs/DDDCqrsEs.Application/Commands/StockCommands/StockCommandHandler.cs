using MediatR;
using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Application.Services;
using DDDCqrsEs.Common;
using DDDCqrsEs.Domain.Aggregates;
using DDDCqrsEs.Domain.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDDCqrsEs.Application.Commands.StockCommands
{

    [MapServiceDependency(Name: nameof(StockCommandHandler))]
    public class StockCommandHandler :  IRequestHandler<CreateStockCommand, CreateStockCommandResponse>,
                                        IRequestHandler<UpdateStockCommand, UpdateStockCommandResponse>,
                                        IRequestHandler<DeleteStockCommand, DeleteStockCommandResponse>
    {

        private readonly IEventStore _eventStore;

        public StockCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<CreateStockCommandResponse> Handle(CreateStockCommand request, CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            var stockAggregate = new Stock(guid);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockCreatedEvent = stockAggregate.Create(model);

            return new CreateStockCommandResponse { AggregateId = guid, Version = stockCreatedEvent.Version };
        }

        public async Task<UpdateStockCommandResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventStore.GetEventsByAggregateId(request.Id, cancellationToken).ToListAsync(cancellationToken);
            var baseEvents = EventMapper.ConvertStoredEvents(eventsSoFar);
            var stockAggregate = new Stock(request.Id);

            stockAggregate.ReconstituteFromEvents(baseEvents);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockUpdatedEvent = stockAggregate.Update(model); 

            return new UpdateStockCommandResponse { AggregateId = stockUpdatedEvent.AggregateId, Version = stockUpdatedEvent.Version };
        }

        public async Task<DeleteStockCommandResponse> Handle(DeleteStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventStore.GetEventsByAggregateId(request.Id, cancellationToken).ToListAsync(cancellationToken);
            var baseEvents = EventMapper.ConvertStoredEvents(eventsSoFar);
            var stockAggregate = new Stock(request.Id);
            stockAggregate.ReconstituteFromEvents(baseEvents);

            var stockDeletedEvent = stockAggregate.Delete();

            return new DeleteStockCommandResponse {AggregateId = stockDeletedEvent.AggregateId, Version = stockDeletedEvent.Version };
        }
    }
}
