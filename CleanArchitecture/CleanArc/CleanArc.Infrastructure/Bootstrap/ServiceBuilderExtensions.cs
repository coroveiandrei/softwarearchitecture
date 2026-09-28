using CleanArc.Application.Services;
using CleanArc.Common;
using CleanArc.Infrastructure.RequestBehaviours;
using CleanArc.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CleanArc.Infrastructure.Bootstrap
{
    public static class ServiceBuilderExtensions
    {
        public static void RegisterInfrastructureComponents(this IServiceCollection services, IConfiguration configuration)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(t => t.WithAttribute<MapServiceDependency>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CurrentUserBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            var webhookUrl = configuration["Notifications:WebhookUrl"]
                ?? throw new InvalidOperationException("Notifications:WebhookUrl is not configured.");

            services.AddHttpClient<INotificationService, NotificationService>(client =>
                client.BaseAddress = new Uri(webhookUrl));
        }
    }
}
