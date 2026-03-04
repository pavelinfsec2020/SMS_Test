using HttpClientLib.Models;

namespace HttpClientLib.Interfaces
{
    public interface IMenuItemProvider
    {
        Task<MenuItem[]> GetMenuAsync();
        MenuItem[] GetMenu();
    }
}
