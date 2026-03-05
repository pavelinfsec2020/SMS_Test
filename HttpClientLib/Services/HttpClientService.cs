using HttpClientLib.Interfaces;
using HttpClientLib.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HttpClientLib.Services
{
    public class HttpClientService : IMenuItemProvider, IOrderSender, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpClientService(string endpoint, string username, string password)
        {
            _endpoint = endpoint;
            _httpClient = new HttpClient();

            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
        }

        public async Task<MenuItem[]> GetMenuAsync()
        {
            var request = new GetMenuRequest();
            var response = await _httpClient.PostAsJsonAsync(_endpoint, request, _jsonOptions);
            var content = await response.Content.ReadAsStringAsync();

            var menuResponse = JsonSerializer.Deserialize<GetMenuResponse>(content, _jsonOptions);

            if (menuResponse == null)
                throw new InvalidOperationException("Failed to parse server response");

            if (!menuResponse.Success)
                throw new InvalidOperationException(menuResponse.ErrorMessage);

            return menuResponse.Data?.MenuItems?.ToArray() ?? Array.Empty<MenuItem>();
        }

        public async Task SendOrderAsync(Order order)
        {
            var request = new SendOrderRequest
            {
                CommandParameters = order
            };

            var response = await _httpClient.PostAsJsonAsync(_endpoint, request, _jsonOptions);
            var content = await response.Content.ReadAsStringAsync();

            var orderResponse = JsonSerializer.Deserialize<SendOrderResponse>(content, _jsonOptions);

            if (orderResponse == null)
                throw new InvalidOperationException("Failed to parse server response");

            if (!orderResponse.Success)
                throw new InvalidOperationException(orderResponse.ErrorMessage);
        }

        public MenuItem[] GetMenu() => GetMenuAsync().GetAwaiter().GetResult();

        public void SendOrder(Order order) => SendOrderAsync(order).GetAwaiter().GetResult();

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
