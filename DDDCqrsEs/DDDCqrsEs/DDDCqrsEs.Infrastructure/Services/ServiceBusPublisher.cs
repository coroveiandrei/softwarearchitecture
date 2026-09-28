using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;
using System.Threading;

namespace DDDCqrsEs.Application.Services
{
    public class ServiceBusPublisher : IBaseEventPublisher
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public ServiceBusPublisher(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishEvent(BaseEvent domainEvent, CancellationToken cancellationToken = default)
        {
            await using (var client = new ServiceBusClient(_connectionString))
            {
                var sender = client.CreateSender(_queueName);

                var serializedMessage = JsonConvert.SerializeObject(domainEvent);
                var message = new ServiceBusMessage(serializedMessage);
                await sender.SendMessageAsync(message, cancellationToken);
            } 
        }
    }
}
