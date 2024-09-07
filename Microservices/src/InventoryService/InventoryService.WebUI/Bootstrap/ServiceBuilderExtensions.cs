using Microsoft.Extensions.DependencyInjection;
using InventoryService.Common;
using System.Reflection;

namespace InventoryService.WebUI
{
    public static class ServiceBuilderExtensions
    {
        public static void RegisterWebAPIServices(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(t => t.WithAttribute<MapServiceDependency>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
