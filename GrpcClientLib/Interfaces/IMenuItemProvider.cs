using SmsTest.GrpcClient;

namespace GrpcClientLib.Interfaces
{
    public interface IMenuItemProvider
    {
        Task<MenuItem[]> GetMenuAsync(bool withPrice = true);    
    }
}