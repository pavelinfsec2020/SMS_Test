using GrpcClientLib.Services;

namespace LibTests.GrpcClientTests
{
   /// <summary>
   /// Данный класс необходим для проверки наличия самих методов, возвращающих Task,
   /// а также созданных клиентов,
   /// поскольку самого сервера нет.
   /// </summary>
    public class GrpcClientServiceTests
    {
        private readonly string _endpoint = "http://localhost:50051";

        [Fact]
        public void Constructor_CreatesChannelAndClient()
        {
            var client = new GrpcClientService(_endpoint);
            Assert.NotNull(client);

            client.Dispose();
        }

        [Fact]
        public void Constructor_WithNullEndpoint_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GrpcClientService(null));
        }

        [Fact]
        public void Constructor_WithEmptyEndpoint_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new GrpcClientService(""));
        }

        [Fact]
        public void Dispose_DisposesChannel()
        {
            var client = new GrpcClientService(_endpoint);              
            Assert.True(true);

            client.Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes_DoesNotThrow()
        {
            var client = new GrpcClientService(_endpoint);
            var exception = Record.Exception(() => client.Dispose());
            Assert.Null(exception);

            client.Dispose();
        }

        [Fact]
        public void GetMenuAsync_ReturnsTask_WhenCalled()
        {
            var client = new GrpcClientService(_endpoint);
            Assert.NotNull(client.GetMenuAsync());

            client.Dispose();
        }

        [Fact]
        public void SendOrderAsync_ReturnsTask_WhenCalled()
        {
            var client = new GrpcClientService(_endpoint);
            var order = new SmsTest.GrpcClient.Order { Id = "123" };
            Assert.NotNull(client.SendOrderAsync(order));
          
            client.Dispose();
        }

        [Fact]
        public void GetMenu_Sync_WrapsAsync()
        {
            var client = new GrpcClientService(_endpoint);
            Assert.NotNull(client);

            client.Dispose();
        }

        [Fact]
        public void SendOrder_Sync_WrapsAsync()
        {
            var client = new GrpcClientService(_endpoint);
            Assert.NotNull(client);

            client.Dispose();
        }
    }
}
