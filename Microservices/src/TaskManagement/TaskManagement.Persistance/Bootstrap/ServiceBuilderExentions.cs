using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common;
using System.Reflection;

namespace TaskManagement.Persistance.Bootstrap
{
    public static class ServiceBuilderExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(t=> t.WithAttribute<MapServiceDependency>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        }
    }
}
