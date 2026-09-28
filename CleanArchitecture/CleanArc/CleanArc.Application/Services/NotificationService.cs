using CleanArc.Common;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArc.Application.Services
{
    [MapServiceDependency(nameof(INotificationService))]
    public class NotificationService : INotificationService
    {
        public async Task NotifyAsync(string message, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(message);
            using var data = new StringContent(json, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            using var response = await client.PostAsync(
                "https://httpbin.org/post",
                data,
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}
