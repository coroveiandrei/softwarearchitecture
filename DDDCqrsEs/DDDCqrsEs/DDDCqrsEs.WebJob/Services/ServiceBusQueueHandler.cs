using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DDDCqrsEs.WebJob.Services
{
    public class ServiceBusQueueHandler : IServiceBusQueueHandler
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly IMediator _mediator;
    
        public ServiceBusQueueHandler(string connectionString, string queueName, IMediator mediator)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            _mediator = mediator;
        }

        public async Task HandleMessage(ProcessMessageEventArgs message)
        {
            var messageBody = message.Message.Body.ToString();
            var eventTypeName = JObject.Parse(messageBody).Value<string>(nameof(BaseEvent.EventType));
            var eventType = eventTypeName switch
            {
                nameof(StockCreated) => typeof(StockCreated),
                nameof(StockUpdated) => typeof(StockUpdated),
                nameof(StockDeleted) => typeof(StockDeleted),
                _ => throw new JsonSerializationException($"Unknown event type '{eventTypeName}'.")
            };
            var typedEvent = (BaseEvent)JsonConvert.DeserializeObject(messageBody, eventType);

            await _mediator.Send(typedEvent);

            Console.WriteLine($"Received {typedEvent.EventType} for stock with id {typedEvent.AggregateId}");

            await message.CompleteMessageAsync(message.Message);
        }

        public async Task HandleError(ProcessErrorEventArgs message)
        {

        }

        public async Task ReceiveMessages()
        {
            await using (var client = new ServiceBusClient(_connectionString))
            { 
                ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

                processor.ProcessMessageAsync += HandleMessage;
                processor.ProcessErrorAsync += HandleError;

                await processor.StartProcessingAsync();

                Console.ReadKey();
                await processor.StopProcessingAsync();
            }
        }

    }
}
