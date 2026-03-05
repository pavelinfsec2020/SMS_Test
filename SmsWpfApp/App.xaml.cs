using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SmsWpfApp.Services;
using System.IO;
using System.Windows;

namespace SmsWpfApp
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public static IConfiguration? Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // optional: true для отладки
                    .Build();

                // Logging
                var logFileName = $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log";
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(
                        path: Path.Combine(basePath, logFileName),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();

                //  DI 
                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddSingleton<ILogger>(Log.Logger);
                services.AddSingleton<EnvironmentVariableService>();
                services.AddSingleton<MainWindow>();

                ServiceProvider = services.BuildServiceProvider();

                Log.Information("Приложение запущено");

                var mainWindow = ServiceProvider.GetService<MainWindow>();
                mainWindow?.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error(ex, "Ошибка при запуске приложения");
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Приложение завершено");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
