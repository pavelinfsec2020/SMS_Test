using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SmsWpfApp.Services;
using SmsWpfApp.ViewModels;
using SmsWpfApp.Views;
using System.IO;
using System.Windows;

namespace SmsWpfApp
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Настройка конфигурации
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                // Настройка логирования
                var logFileName = $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log";
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(
                        path: Path.Combine(basePath, logFileName),
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();

                // Настройка DI контейнера
                var services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddSingleton<ILogger>(Log.Logger);
                services.AddSingleton<EnvironmentVariableService>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();

                ServiceProvider = services.BuildServiceProvider();

                Log.Information("Приложение запущено");

                // Запускаем главное окно с ViewModel
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
