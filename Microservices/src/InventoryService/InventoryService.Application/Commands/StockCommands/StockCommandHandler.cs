using MediatR;
using InventoryService.Application.Common;
using InventoryService.Application.Services;
using InventoryService.Common;
using InventoryService.Domain.Aggregates;
using InventoryService.Domain.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.RemoteServices;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Commands.StockCommands
{

    [MapServiceDependency(Name: nameof(StockCommandHandler))]
    public class StockCommandHandler :  IRequestHandler<CreateStockCommand, CreateStockCommandResponse>,
                                        IRequestHandler<UpdateStockCommand, UpdateStockCommandResponse>,
                                        IRequestHandler<DeleteStockCommand, DeleteStockCommandResponse>
    {

        private IEventStore _eventRepository;
        private IBaseEventPublisher _eventPublisher;
        private IPlacementService _placementService;
        private ILogger<StockCommandHandler> _logger;

        public StockCommandHandler(IEventStore eventRepository, IBaseEventPublisher eventPublisher, IPlacementService placementService, ILogger<StockCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _eventPublisher = eventPublisher;
            _placementService = placementService;
            _logger = logger;
        }

        public async Task<CreateStockCommandResponse> Handle(CreateStockCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Creating Stock {request.LicensePlate}");

            var guid = Guid.NewGuid();
            var stockAggregate = new Stock(guid);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockCreatedEvent = stockAggregate.Create(model);

            await _eventRepository.SaveEvent(stockCreatedEvent);
            await _eventPublisher.PublishEvent(stockCreatedEvent);
            var locations = await _placementService.GetEligibleLocations(3);
            _logger.LogInformation($"Called placement service, got {locations.Count()} locations");
            if (locations.Any())
            {
                var firstLocation = locations.First();
                _logger.LogInformation($"Reserving {firstLocation} ");
                var result = await _placementService.ReserveLocation(new PlacementReservationModel(firstLocation, model.LicensePlate));
                if(result)
                    _logger.LogInformation($"Reservation Done for {firstLocation} ");
                else
                    _logger.LogInformation($"Reservation Failed for {firstLocation} ");
            }

            return new CreateStockCommandResponse { AggregateId = guid, Version = stockCreatedEvent.Version };
        }

        public async Task<UpdateStockCommandResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventRepository.GetEventsByAggregateId(request.Id).ToListAsync();
            var baseEvents = EventMapper.ConvertListOfEntityEventsToBase(eventsSoFar);
            var stockAggregate = new Stock(request.Id);

            stockAggregate.ReconstituteFromEvents(baseEvents);

            var model = ModelMapper.ConvertCommandToModel(request);
            var stockUpdatedEvent = stockAggregate.Update(model);

            await _eventRepository.SaveEvent(stockUpdatedEvent);
            return new UpdateStockCommandResponse { AggregateId = stockUpdatedEvent.AggregateId, Version = stockUpdatedEvent.Version };
        }

        public async Task<DeleteStockCommandResponse> Handle(DeleteStockCommand request, CancellationToken cancellationToken)
        {
            var eventsSoFar = await _eventRepository.GetEventsByAggregateId(request.Id).ToListAsync();
            var baseEvents = EventMapper.ConvertListOfEntityEventsToBase(eventsSoFar);
            var stockAggregate = new Stock(request.Id);
            stockAggregate.ReconstituteFromEvents(baseEvents);

            var stockDeletedEvent = stockAggregate.Delete();
            await _eventRepository.SaveEvent(stockDeletedEvent);

            return new DeleteStockCommandResponse {AggregateId = stockDeletedEvent.AggregateId, Version = stockDeletedEvent.Version };
        }
    }
}
