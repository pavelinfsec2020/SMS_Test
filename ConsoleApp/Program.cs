using ConsoleApp.Data;
using ConsoleApp.Services;
using GrpcClientLib.Services;
using HttpClientLib.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ClientMenuItem = HttpClientLib.Models.MenuItem;
using ClientOrder = HttpClientLib.Models.Order;
using ClientOrderItem = HttpClientLib.Models.OrderItem;

namespace SmsTest.ConsoleApp;

class Program
{
    public static async Task Main(string[] args)
    {
        var logFileName = $"test-sms-console-app-{DateTime.Now:yyyyMMdd}.log";
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFileName)
            .CreateLogger();

        try
        {
            Log.Information("Application started");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            var serviceProvider = services.BuildServiceProvider();
            var dbService = new DatabaseService(serviceProvider, Log.Logger);

            await dbService.InitializeDatabaseAsync();

            ClientMenuItem[] menuItems;
            var apiType = config["ApiType"];

            if (apiType == "Grpc")
            {
                var grpcEndpoint = config["GrpcSettings:Endpoint"];
                var grpcClient = new GrpcClientService(grpcEndpoint);
                var grpcItems = await grpcClient.GetMenuAsync(true);

                menuItems = grpcItems.Select(m => new ClientMenuItem
                {
                    Id = m.Id,
                    Article = m.Article,
                    Name = m.Name,
                    Price = (decimal)m.Price,
                    IsWeighted = m.IsWeighted,
                    FullPath = m.FullPath,
                    Barcodes = m.Barcodes.ToList()
                }).ToArray();
            }
            else
            {
                var httpEndpoint = config["HttpSettings:Endpoint"];
                var username = config["HttpSettings:Username"];
                var password = config["HttpSettings:Password"];

                var httpClient = new HttpClientService(httpEndpoint, username, password);
                menuItems = await httpClient.GetMenuAsync();
            }

            await dbService.SaveMenuItemsAsync(menuItems);

            foreach (var item in menuItems)
            {
                Log.Information("{Name} – {Article} – {Price} rub",
                    item.Name, item.Article, item.Price);
            }

            var order = new ClientOrder
            {
                OrderId = Guid.NewGuid().ToString()
            };

            var menuDict = await dbService.GetMenuItemsDictAsync();
            var inputValid = false;
           
            while (!inputValid)
            {
                Log.Information("Enter order items (Code:Quantity;Code2:Quantity2;...):");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Log.Warning("Input cannot be empty");
                    continue;
                }

                try
                {
                    var items = ParseOrderItems(input);
                    inputValid = ValidateOrderItems(items, menuDict);

                    if (inputValid)
                    {
                        foreach (var item in items)
                        {
                            var menuItem = menuDict[item.Key];
                            order.MenuItems.Add(new ClientOrderItem
                            {
                                Id = menuItem.Id,
                                Quantity = item.Value.ToString("F3")
                            });
                        }
                        Log.Information("Order created successfully");
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("Error parsing input: {Message}", ex.Message);
                }
            }

            try
            {
                if (apiType == "Grpc")
                {
                    var grpcEndpoint = config["GrpcSettings:Endpoint"];
                    var grpcClient = new GrpcClientService(grpcEndpoint);

                    var grpcOrder = new SmsTest.GrpcClient.Order
                    {
                        Id = order.OrderId
                    };

                    foreach (var item in order.MenuItems)
                    {
                        grpcOrder.OrderItems.Add(new SmsTest.GrpcClient.OrderItem
                        {
                            Id = item.Id,
                            Quantity = double.Parse(item.Quantity)
                        });
                    }

                    await grpcClient.SendOrderAsync(grpcOrder);
                }
                else
                {
                    var httpEndpoint = config["HttpSettings:Endpoint"];
                    var username = config["HttpSettings:Username"];
                    var password = config["HttpSettings:Password"];

                    var httpClient = new HttpClientService(httpEndpoint, username, password);
                    await httpClient.SendOrderAsync(order);
                }

                Log.Information("SUCCESS");
            }
            catch (Exception ex)
            {
                Log.Error("Error sending order: {Message}", ex.Message);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application error");
            Log.Information("Program terminated: {Message}", ex.Message);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static Dictionary<string, decimal> ParseOrderItems(string input)
    {
        var result = new Dictionary<string, decimal>();
        var pairs = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            var parts = pair.Split(':');
           
            if (parts.Length != 2)
                throw new FormatException($"Invalid format: {pair}");

            var code = parts[0].Trim();
            
            if (!decimal.TryParse(parts[1].Trim(), out var quantity))
                throw new FormatException($"Invalid quantity: {parts[1]}");

            if (quantity <= 0)
                throw new ArgumentException($"Quantity must be positive: {quantity}");

            result[code] = quantity;
        }

        return result;
    }

    private static bool ValidateOrderItems(Dictionary<string, decimal> items, Dictionary<string, ClientMenuItem> menuDict)
    {
        foreach (var code in items.Keys)
        {
            if (!menuDict.ContainsKey(code))
            {
                Log.Warning($"Code {code} not found in menu");
                return false;
            }
        }
        return true;
    }
}