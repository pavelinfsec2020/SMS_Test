using SmsTest.GrpcClient;

namespace GrpcClientLib.Interfaces
{
    public interface IOrderSender
    {
        Task SendOrderAsync(Order order);
        void SendOrder(Order order);
    }
}
