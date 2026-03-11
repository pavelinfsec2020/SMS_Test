using HttpClientLib.Models;

namespace HttpClientLib.Interfaces
{
    public interface IOrderSender
    {
        Task SendOrderAsync(Order order);
    }
}
