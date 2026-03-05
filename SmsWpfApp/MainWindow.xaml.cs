using Serilog;
using SmsWpfApp.Converters;
using SmsWpfApp.Models;
using SmsWpfApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SmsWpfApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region fields
       
        private readonly EnvironmentVariableService _environmentService;
        private readonly ILogger _logger;
        private string _statusMessage = "Готов к работе";

        #endregion

        #region ctor
        public MainWindow(EnvironmentVariableService environmentService, ILogger logger)
        {
            InitializeComponent();

            _environmentService = environmentService;
            _logger = logger;

            DataContext = this;

            Resources.Add("BoolToStatusConverter", new BoolToStatusConverter());
            Resources.Add("BoolToColorConverter", new BoolToColorConverter());

            Loaded += MainWindow_Loaded;
        }

        #endregion

        #region props

        public ObservableCollection<EnvironmentVariable> Variables => _environmentService.Variables;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region event handlers

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StatusMessage = $"Загружено {Variables.Count} переменных";
        }

        private void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _environmentService.SaveAllVariables();
                StatusMessage = "Все переменные успешно сохранены";

                MessageBox.Show("Переменные среды успешно сохранены!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при сохранении переменных");
                StatusMessage = "Ошибка при сохранении переменных";

                MessageBox.Show($"Ошибка при сохранении переменных: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите сбросить все переменные к значениям по умолчанию?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _environmentService.RestoreDefaults();
                    StatusMessage = "Переменные сброшены к значениям по умолчанию";

                    MessageBox.Show("Переменные сброшены к значениям по умолчанию!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Ошибка при сбросе переменных");
                    StatusMessage = "Ошибка при сбросе переменных";

                    MessageBox.Show($"Ошибка при сбросе переменных: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _environmentService.LoadVariables();
            StatusMessage = "Переменные обновлены";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}