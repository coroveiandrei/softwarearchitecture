using CleanArc.Application.Commands.ToDoCommands;
using CleanArc.Application.Interfaces;
using CleanArc.Application.Models;
using CleanArc.Application.Services;
using CleanArc.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Quotation.Domain.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArc.Application.Tests.Commands
{
    public class ToDoCommandHandlerTests
    {
        [Fact]
        public async Task AddToDo_PersistsDescription()
        {
            var repository = new InMemoryRepository();
            var notifications = new RecordingNotificationService();
            var handler = new ToDoCommandHandler(
                repository,
                NullLogger<ToDoCommandHandler>.Instance,
                notifications);
            var userId = Guid.NewGuid();
            var command = new AddToDoCommand
            {
                User = new CurrentUser { UserId = userId, Username = "student" },
                ToDo = new ToDoModel
                {
                    Name = "Review architecture",
                    Description = "Trace the request across the application boundary"
                }
            };

            var response = await handler.Handle(command, CancellationToken.None);

            var savedToDo = Assert.Single(repository.GetEntities<ToDo>());
            Assert.Equal(response.ToDoId, savedToDo.Id);
            Assert.Equal(command.ToDo.Name, savedToDo.Name);
            Assert.Equal(command.ToDo.Description, savedToDo.Description);
            Assert.Equal(userId, savedToDo.UserId);
        }

        [Fact]
        public async Task AddToDo_ReturnsPersistedIdAndSendsNotification()
        {
            var repository = new InMemoryRepository();
            var notifications = new RecordingNotificationService();
            var handler = new ToDoCommandHandler(
                repository,
                NullLogger<ToDoCommandHandler>.Instance,
                notifications);
            var command = new AddToDoCommand
            {
                User = new CurrentUser { UserId = Guid.NewGuid(), Username = "student" },
                ToDo = new ToDoModel
                {
                    Name = "Review architecture",
                    Description = "Trace the request across the application boundary"
                }
            };

            var response = await handler.Handle(command, CancellationToken.None);

            var savedToDo = Assert.Single(repository.GetEntities<ToDo>());
            Assert.Equal(savedToDo.Id, response.ToDoId);
            Assert.Equal("New to do added", Assert.Single(notifications.Messages));
        }

        private sealed class RecordingNotificationService : INotificationService
        {
            public List<string> Messages { get; } = new List<string>();

            public Task NotifyAsync(string message, CancellationToken cancellationToken = default)
            {
                Messages.Add(message);
                return Task.CompletedTask;
            }
        }

        private sealed class InMemoryRepository : IRepository, IUnitOfWork
        {
            private readonly List<object> _entities = new List<object>();

            public IUnitOfWork CreateUnitOfWork() => this;

            public IQueryable<T> GetEntities<T>() where T : class =>
                _entities.OfType<T>().AsQueryable();

            public void Add<T>(T entity) where T : class => _entities.Add(entity);

            public void Delete<T>(T entity) where T : class => _entities.Remove(entity);

            public void SaveChanges()
            {
            }

            public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public void BeginTransactionScope()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
