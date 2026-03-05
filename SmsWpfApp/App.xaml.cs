using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Windows;

namespace SmsWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Настройка конфигурации
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Настройка логирования
            var logFileName = $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: logFileName,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            //  Применяю для каждого экземпляра паттерн Singleton
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddSingleton(Log.Logger);
            services.AddSingleton<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            Log.Information("Приложение запущено");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Приложение завершено");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
