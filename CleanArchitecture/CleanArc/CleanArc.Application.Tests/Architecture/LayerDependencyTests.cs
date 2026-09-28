using CleanArc.Application.Services;
using System;
using System.Linq;
using Xunit;

namespace CleanArc.Application.Tests.Architecture
{
    public class LayerDependencyTests
    {
        [Fact]
        public void ApplicationLayer_DoesNotContainNotificationAdapters()
        {
            var applicationAssembly = typeof(INotificationService).Assembly;

            var concreteNotificationAdapters = applicationAssembly
                .GetTypes()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(INotificationService).IsAssignableFrom(type))
                .Select(type => type.FullName)
                .ToArray();

            Assert.True(
                concreteNotificationAdapters.Length == 0,
                $"Application contains notification adapter(s): {string.Join(", ", concreteNotificationAdapters)}");
        }
    }
}
