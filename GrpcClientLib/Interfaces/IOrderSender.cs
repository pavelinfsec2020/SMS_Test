using SmsTest.GrpcClient;

namespace GrpcClientLib.Interfaces
{
    public interface IOrderSender
    {
        Task SendOrderAsync(Order order);    
    }
}
