using HttpClientLib.Models;
using HttpClientLib.Services;
using RichardSzalay.MockHttp;
using System.Text.Json;

namespace LibTests.HttpClientTests
{
    public class HttpClientServiceTests
    {
        private readonly string _endpoint = "http://test.com/api";
        private readonly string _username = "testuser";
        private readonly string _password = "testpass";

        [Fact]
        public async Task GetMenuAsync_ReturnsMenuItems_WhenSuccess()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new GetMenuResponse
            {
                Command = "GetMenu",
                Success = true,
                Data = new MenuData
                {
                    MenuItems = new List<MenuItem>
                {
                    new() { Id = "1", Name = "Test Dish", Price = 100 }
                }
                }
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var result = await client.GetMenuAsync();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("1", result[0].Id);
            Assert.Equal("Test Dish", result[0].Name);
            Assert.Equal(100, result[0].Price);
        }

        [Fact]
        public async Task GetMenuAsync_ReturnsEmptyArray_WhenNoItems()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new GetMenuResponse
            {
                Command = "GetMenu",
                Success = true,
                Data = new MenuData
                {
                    MenuItems = new List<MenuItem>()
                }
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var result = await client.GetMenuAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMenuAsync_ThrowsException_WhenSuccessFalse()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new GetMenuResponse
            {
                Command = "GetMenu",
                Success = false,
                ErrorMessage = "Server error"
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetMenuAsync());
            Assert.Equal("Server error", exception.Message);
        }

        [Fact]
        public async Task GetMenuAsync_ThrowsException_WhenResponseNull()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", "null");

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetMenuAsync());
        }

        [Fact]
        public async Task GetMenuAsync_ThrowsException_WhenInvalidJson()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", "{ invalid json ");

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            await Assert.ThrowsAsync<JsonException>(() => client.GetMenuAsync());
        }

        [Fact]
        public async Task SendOrderAsync_Succeeds_WhenSuccess()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new SendOrderResponse
            {
                Command = "SendOrder",
                Success = true
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var order = new Order { OrderId = "123" };

            var exception = await Record.ExceptionAsync(() => client.SendOrderAsync(order));
            Assert.Null(exception);
        }

        [Fact]
        public async Task SendOrderAsync_ThrowsException_WhenSuccessFalse()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new SendOrderResponse
            {
                Command = "SendOrder",
                Success = false,
                ErrorMessage = "Order error"
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var order = new Order { OrderId = "123" };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => client.SendOrderAsync(order));
            Assert.Equal("Order error", exception.Message);
        }

        [Fact]
        public async Task SendOrderAsync_ThrowsException_WhenResponseNull()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", "null");

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var order = new Order { OrderId = "123" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => client.SendOrderAsync(order));
        }

        [Fact]
        public void Constructor_SetsBasicAuthHeader()
        {
            var client = new HttpClientService(_endpoint, _username, _password);

            var httpClient = GetPrivateField<HttpClient>(client, "_httpClient");
            var authHeader = httpClient.DefaultRequestHeaders.Authorization;

            Assert.NotNull(authHeader);
            Assert.Equal("Basic", authHeader.Scheme);

            var credentials = Convert.FromBase64String(authHeader.Parameter);
            var credentialString = System.Text.Encoding.ASCII.GetString(credentials);
            Assert.Equal($"{_username}:{_password}", credentialString);
        }

        [Fact]
        public void GetMenu_Sync_WrapsAsync()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new GetMenuResponse
            {
                Command = "GetMenu",
                Success = true,
                Data = new MenuData
                {
                    MenuItems = new List<MenuItem>
                {
                    new() { Id = "1", Name = "Test Dish", Price = 100 }
                }
                }
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var result = client.GetMenu();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void SendOrder_Sync_WrapsAsync()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new SendOrderResponse
            {
                Command = "SendOrder",
                Success = true
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            var order = new Order { OrderId = "123" };

            var exception = Record.Exception(() => client.SendOrder(order));
            Assert.Null(exception);
        }

        [Fact]
        public void GetMenu_Sync_ThrowsException_WhenAsyncThrows()
        {
            var mockHttp = new MockHttpMessageHandler();

            var expectedResponse = new GetMenuResponse
            {
                Command = "GetMenu",
                Success = false,
                ErrorMessage = "Server error"
            };

            mockHttp.When(HttpMethod.Post, _endpoint)
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = mockHttp.ToHttpClient();
            SetPrivateField(client, "_httpClient", httpClient);

            Assert.Throws<InvalidOperationException>(() => client.GetMenu());
        }

        [Fact]
        public void Dispose_DisposesHttpClient()
        {
            var client = new HttpClientService(_endpoint, _username, _password);
            var httpClient = GetPrivateField<HttpClient>(client, "_httpClient");

            client.Dispose();

            Assert.Throws<ObjectDisposedException>(() => httpClient.BaseAddress = null);
        }

        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            return (T)field?.GetValue(obj);
        }

        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
