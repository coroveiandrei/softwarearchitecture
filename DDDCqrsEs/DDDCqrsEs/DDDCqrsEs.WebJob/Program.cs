using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using DDDCqrsEs.Persistance;
using Microsoft.EntityFrameworkCore;
using DDDCqrsEs.Persistance.Bootstrap;
using DDDCqrsEs.WebJob.Services;
using System;
using DDDCqrsEs.Application.EventHandlers;

namespace DDDCqrsEs.WebJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables();

            IConfiguration config = builder.Build();
            var connectionString = config.GetConnectionString("ServiceBusString");
            var queueName = config.GetConnectionString("QueueName");
            var dbString = config.GetConnectionString("ToDoDatabase");
            
            var t = typeof(StockEventHandler);

            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(dbString));
            services.RegisterRepositories();
            var serviceProvider = services.BuildServiceProvider();
            
            var mediator = serviceProvider.GetService<IMediator>();
            var messageHandler = new ServiceBusQueueHandler(connectionString, queueName, mediator);
            await messageHandler.ReceiveMessages();
        }
    }
}
