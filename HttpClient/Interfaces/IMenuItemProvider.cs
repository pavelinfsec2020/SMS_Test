using HttpClientLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpClientLib.Interfaces
{
    public interface IMenuItemProvider
    {
        Task<MenuItem[]> GetMenuAsync();
        MenuItem[] GetMenu();
    }
}
