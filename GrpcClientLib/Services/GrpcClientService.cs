using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcClientLib.Interfaces;
using SmsTest.GrpcClient;

namespace GrpcClientLib.Services
{
    public class GrpcClientService : IMenuItemProvider, IOrderSender, IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly SmsTestService.SmsTestServiceClient _client;

        public GrpcClientService(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _client = new SmsTestService.SmsTestServiceClient(_channel);
        }

        public async Task<MenuItem[]> GetMenuAsync(bool withPrice = true)
        {
            var response = await _client.GetMenuAsync(new BoolValue { Value = withPrice });

            if (!response.Success)
                throw new InvalidOperationException(response.ErrorMessage);

            return response.MenuItems.ToArray();
        }

        public async Task SendOrderAsync(Order order)
        {
            var response = await _client.SendOrderAsync(order);

            if (!response.Success)
                throw new InvalidOperationException(response.ErrorMessage);
        }

        public MenuItem[] GetMenu(bool withPrice = true)
            => GetMenuAsync(withPrice).GetAwaiter().GetResult();

        public void SendOrder(Order order)
            => SendOrderAsync(order).GetAwaiter().GetResult();

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
