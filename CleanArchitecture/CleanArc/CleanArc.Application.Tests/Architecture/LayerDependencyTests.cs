using CleanArc.Application.Interfaces;
using System;
using System.Linq;
using Xunit;

namespace CleanArc.Application.Tests.Architecture
{
    public class LayerDependencyTests
    {
        [Fact]
        public void ApplicationLayer_DoesNotImplementItsOutboundPorts()
        {
            var applicationAssembly = typeof(IRepository).Assembly;
            var applicationTypes = applicationAssembly.GetTypes();
            var outboundPorts = applicationTypes
                .Where(type =>
                    type.IsInterface &&
                    IsOutboundPortNamespace(type.Namespace))
                .ToArray();

            var violations = applicationTypes
                .Where(type => type.IsClass && !type.IsAbstract)
                .SelectMany(implementation => outboundPorts
                    .Where(port => port.IsAssignableFrom(implementation))
                    .Select(port => $"{implementation.FullName} implements {port.FullName}"))
                .ToArray();

            Assert.True(
                violations.Length == 0,
                $"Application contains implementation(s) of its outbound ports: {string.Join(", ", violations)}");
        }

        private static bool IsOutboundPortNamespace(string namespaceName) =>
            namespaceName != null &&
            (namespaceName.StartsWith("CleanArc.Application.Interfaces", StringComparison.Ordinal) ||
             namespaceName.StartsWith("CleanArc.Application.Services", StringComparison.Ordinal));
    }
}
