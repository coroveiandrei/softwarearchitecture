using DDDCqrsEs.Application.Commands.StockCommands;
using DDDCqrsEs.Application.EventHandlers;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Projections;
using DDDCqrsEs.Domain.Repositories;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DDDCqrsEs.Tests
{
    public class AssignmentCompletionTests
    {
        private const string ConcurrencyMessage = "Data was changed by another user.";

        [Fact]
        public void Update_validator_rejects_a_stale_projection_version()
        {
            var id = Guid.NewGuid();
            var repository = new FakeProjectionRepository(new StockProjection
            {
                Id = id,
                LicensePlate = "ABC-123",
                Version = 2
            });
            var validator = new UpdateStockCommandValidator(repository);

            var result = validator.Validate(new UpdateStockCommand
            {
                Id = id,
                LicensePlate = "ABC-123",
                Version = 1,
                BestBeforeDate = DateTime.Now.AddDays(1)
            });

            Assert.Contains(result.Errors, error => error.ErrorMessage == ConcurrencyMessage);

            var currentResult = validator.Validate(new UpdateStockCommand
            {
                Id = id,
                LicensePlate = "ABC-123",
                Version = 2,
                BestBeforeDate = DateTime.Now.AddDays(1)
            });
            Assert.DoesNotContain(currentResult.Errors, error => error.ErrorMessage == ConcurrencyMessage);
        }

        [Fact]
        public void Delete_validator_rejects_a_stale_projection_version()
        {
            var id = Guid.NewGuid();
            var validator = new DeleteStockCommandValidator(new FakeProjectionRepository(
                new StockProjection { Id = id, Version = 2 }));

            var result = validator.Validate(new DeleteStockCommand { Id = id, Version = 1 });

            Assert.Contains(result.Errors, error => error.ErrorMessage == ConcurrencyMessage);
        }

        [Fact]
        public async Task Write_path_rejects_a_stale_version_even_when_the_projection_is_lagging()
        {
            var id = Guid.NewGuid();
            var storedEvent = new StoredEvent(id, 2, nameof(StockUpdated),
                JsonConvert.SerializeObject(new StockModel { Item = "Current" }), DateTime.UtcNow);
            var eventStore = new FakeEventStore(storedEvent);
            var handler = new StockCommandHandler(eventStore);

            var exception = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(
                new UpdateStockCommand { Id = id, Version = 1 }, CancellationToken.None));

            Assert.Contains(exception.Errors, error => error.ErrorMessage == ConcurrencyMessage);
            Assert.Null(eventStore.SavedEvent);
        }

        [Fact]
        public async Task Read_path_applies_the_delete_event_version_to_the_projection()
        {
            var id = Guid.NewGuid();
            var repository = new FakeProjectionRepository(new StockProjection { Id = id, Version = 3 });
            var handler = new StockEventHandler(repository);

            await handler.Handle(new StockDeleted(id, 4, DateTime.UtcNow), CancellationToken.None);

            Assert.Equal((id, 4), repository.DeletedStock);
        }

        private sealed class FakeEventStore : IEventStore
        {
            private readonly StoredEvent _storedEvent;
            public FakeEventStore(StoredEvent storedEvent) => _storedEvent = storedEvent;
            public BaseEvent SavedEvent { get; private set; }

            public async IAsyncEnumerable<StoredEvent> GetEventsByAggregateId(Guid id,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                yield return _storedEvent;
                await Task.CompletedTask;
            }

            public IAsyncEnumerable<StoredEvent> GetPendingEvents(CancellationToken cancellationToken = default) =>
                Empty(cancellationToken);

            public Task SaveEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
            {
                SavedEvent = domainEvent;
                return Task.CompletedTask;
            }

            public Task MarkEventPublished(Guid aggregateId, int version, CancellationToken cancellationToken = default) =>
                Task.CompletedTask;

            private static async IAsyncEnumerable<StoredEvent> Empty([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        private sealed class FakeProjectionRepository : IStockProjectionRepository
        {
            private readonly StockProjection _stock;
            public FakeProjectionRepository(StockProjection stock) => _stock = stock;
            public (Guid Id, int Version)? DeletedStock { get; private set; }

            public IEnumerable<StockProjection> GetAllStocks() => new[] { _stock };
            public StockProjection GetStockById(Guid id) => _stock?.Id == id ? _stock : null;
            public StockProjection GetStockByLicensePlate(string licensePlate) =>
                _stock?.LicensePlate == licensePlate ? _stock : null;
            public void CreateStock(StockProjection stock) { }
            public void UpdateStock(Guid id, StockModel model, int version) { }
            public void DeleteStock(Guid id, int version) => DeletedStock = (id, version);
        }
    }
}
