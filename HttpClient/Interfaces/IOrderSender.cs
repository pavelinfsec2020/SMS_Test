using HttpClientLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientLib.Interfaces
{
    public interface IOrderSender
    {
        Task SendOrderAsync(Order order);
        void SendOrder(Order order);
    }
}
