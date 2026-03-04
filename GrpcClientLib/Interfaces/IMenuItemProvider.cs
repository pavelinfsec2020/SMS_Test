using SmsTest.GrpcClient;

namespace GrpcClientLib.Interfaces
{
    public interface IMenuItemProvider
    {
        Task<MenuItem[]> GetMenuAsync(bool withPrice = true);
        MenuItem[] GetMenu(bool withPrice = true);
    }
}