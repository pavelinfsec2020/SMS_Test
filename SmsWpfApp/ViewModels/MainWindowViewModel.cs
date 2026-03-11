using SmsWpfApp.Models;
using SmsWpfApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmsWpfApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly EnvironmentVariableService _environmentService;
        private string _statusMessage = "Готов к работе";

        public ObservableCollection<EnvironmentVariable> Variables { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveAllCommand { get; }
        public ICommand RestoreDefaultsCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel(EnvironmentVariableService environmentService)
        {
            _environmentService = environmentService;
            Variables = _environmentService.Variables;

            SaveAllCommand = new RelayCommand(_ => SaveAll());
            RestoreDefaultsCommand = new RelayCommand(_ => RestoreDefaults(), _ => CanRestoreDefaults());
            RefreshCommand = new RelayCommand(_ => Refresh());

            LoadVariables();
        }

        private void LoadVariables()
        {
            _environmentService.LoadVariables();
            StatusMessage = $"Загружено {Variables.Count} переменных";
        }

        private void SaveAll()
        {
            try
            {
                _environmentService.SaveAllVariables();
                StatusMessage = "Все переменные успешно сохранены";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private bool CanRestoreDefaults()
        {
            return Variables.Count > 0;
        }

        private void RestoreDefaults()
        {
            _environmentService.RestoreDefaults();
            StatusMessage = "Переменные сброшены к значениям по умолчанию";
        }

        private void Refresh()
        {
            _environmentService.LoadVariables();
            StatusMessage = "Переменные обновлены";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
