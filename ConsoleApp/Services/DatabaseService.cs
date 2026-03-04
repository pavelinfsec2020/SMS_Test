using ConsoleApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ClientMenuItem = HttpClientLib.Models.MenuItem;

namespace ConsoleApp.Services
{
    public class DatabaseService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public DatabaseService(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeDatabaseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.EnsureCreatedAsync();
            _logger.Information("Database initialized");
        }

        public async Task SaveMenuItemsAsync(ClientMenuItem[] items)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            foreach (var item in items)
            {
                var entity = await context.MenuItems.FindAsync(item.Id);

                if (entity == null)
                {
                    entity = new MenuItemEntity
                    {
                        Id = item.Id,
                        Article = item.Article,
                        Name = item.Name,
                        Price = item.Price,
                        IsWeighted = item.IsWeighted,
                        FullPath = item.FullPath,
                        Barcodes = string.Join(",", item.Barcodes)
                    };
                    await context.MenuItems.AddAsync(entity);
                }
                else
                {
                    entity.Article = item.Article;
                    entity.Name = item.Name;
                    entity.Price = item.Price;
                    entity.IsWeighted = item.IsWeighted;
                    entity.FullPath = item.FullPath;
                    entity.Barcodes = string.Join(",", item.Barcodes);
                    context.MenuItems.Update(entity);
                }
            }

            await context.SaveChangesAsync();
            _logger.Information($"Saved {items.Length} items to database");
        }

        public async Task<Dictionary<string, ClientMenuItem>> GetMenuItemsDictAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var items = await context.MenuItems
                .Select(m => new ClientMenuItem
                {
                    Id = m.Id,
                    Article = m.Article,
                    Name = m.Name,
                    Price = m.Price,
                    IsWeighted = m.IsWeighted
                })
                .ToDictionaryAsync(m => m.Id);

            return items;
        }
    }
}
