using CleanArc.Application.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArc.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NotifyAsync(string message, CancellationToken cancellationToken = default)
        {
            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("The notification webhook URL is not configured.");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, _httpClient.BaseAddress)
            {
                Content = JsonContent.Create(message)
            };
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
