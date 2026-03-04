using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using HttpClientLib.Services;
using ConsoleApp.Configuration;
using ConsoleApp.Data;
using ConsoleApp.Mappers;
using ConsoleApp.Services;
using ConsoleApp.Models;
using ConsoleApp;
using GrpcClientLib.Services;


namespace SmsTest.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        SetupLogging();

        try
        {
            Log.Information("Application started");

            var settings = LoadConfiguration();
            var serviceProvider = ConfigureServices(settings);

            await InitializeDatabaseAsync(serviceProvider);

            var menuItems = await GetMenuFromServerAsync(settings);

            await SaveMenuToDatabaseAsync(serviceProvider, menuItems);

            DisplayMenu(menuItems);

            var order = await CreateOrderFromUserInputAsync(serviceProvider);

            await SendOrderToServerAsync(settings, order);

            Log.Information(Constants.Messages.Success);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application error");
            Log.Information(Constants.Messages.ProgramTerminated, ex.Message);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void SetupLogging()
    {
        var logFileName = $"{Constants.LogFilePrefix}{DateTime.Now:yyyyMMdd}.log";
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFileName)
            .CreateLogger();
    }

    private static AppSettings LoadConfiguration()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(Constants.AppSettingsFile, optional: false)
            .Build();

        return new AppSettings
        {
            ApiType = config["ApiType"] ?? "Http",
            HttpSettings = new HttpSettings
            {
                Endpoint = config["HttpSettings:Endpoint"] ?? "",
                Username = config["HttpSettings:Username"] ?? "",
                Password = config["HttpSettings:Password"] ?? ""
            },
            GrpcSettings = new GrpcSettings
            {
                Endpoint = config["GrpcSettings:Endpoint"] ?? ""
            },
            ConnectionStrings = new ConnectionStrings
            {
                DefaultConnection = config.GetConnectionString("DefaultConnection") ?? ""
            }
        };
    }

    private static IServiceProvider ConfigureServices(AppSettings settings)
    {
        var services = new ServiceCollection();

        services.AddSingleton(Log.Logger);
        services.AddSingleton(settings);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(settings.ConnectionStrings.DefaultConnection));

        services.AddSingleton<DatabaseService>();
        services.AddSingleton<OrderParser>();
        services.AddSingleton<OrderService>();

        return services.BuildServiceProvider();
    }

    private static async Task InitializeDatabaseAsync(IServiceProvider services)
    {
        var dbService = services.GetRequiredService<DatabaseService>();
        await dbService.InitializeDatabaseAsync();
    }

    private static async Task<MenuItem[]> GetMenuFromServerAsync(AppSettings settings)
    {
        if (settings.ApiType == Constants.ApiTypes.Grpc)
        {
            using var client = new GrpcClientService(settings.GrpcSettings.Endpoint);
            var items = await client.GetMenuAsync(true);
            return items.Select(MenuItemMapper.MapFromGrpc).ToArray();
        }
        else
        {
            using var client = new HttpClientService(
                settings.HttpSettings.Endpoint,
                settings.HttpSettings.Username,
                settings.HttpSettings.Password);
            var items = await client.GetMenuAsync();
            return items.Select(MenuItemMapper.MapFromClient).ToArray();
        }
    }

    private static async Task SaveMenuToDatabaseAsync(IServiceProvider services, MenuItem[] menuItems)
    {
        var dbService = services.GetRequiredService<DatabaseService>();
        await dbService.SaveMenuItemsAsync(menuItems);
    }

    private static void DisplayMenu(MenuItem[] menuItems)
    {
        Log.Information(Constants.Messages.MenuHeader);
        foreach (var item in menuItems)
        {
            Log.Information(Constants.Messages.MenuFormat,
                item.Name, item.Article, item.Price);
        }
    }

    private static async Task<Order> CreateOrderFromUserInputAsync(IServiceProvider services)
    {
        var orderService = services.GetRequiredService<OrderService>();
        return await orderService.CreateOrderFromInputAsync();
    }

    private static async Task SendOrderToServerAsync(AppSettings settings, Order order)
    {
        if (settings.ApiType == Constants.ApiTypes.Grpc)
        {
            using var client = new GrpcClientService(settings.GrpcSettings.Endpoint);
            var grpcOrder = MenuItemMapper.MapToGrpc(order);
            await client.SendOrderAsync(grpcOrder);
        }
        else
        {
            using var client = new HttpClientService(
                settings.HttpSettings.Endpoint,
                settings.HttpSettings.Username,
                settings.HttpSettings.Password);
            var clientOrder = MenuItemMapper.MapToClient(order);
            await client.SendOrderAsync(clientOrder);
        }
    }
}