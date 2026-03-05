using Microsoft.Extensions.Configuration;
using Serilog;
using SmsWpfApp.Models;
using System.Collections.ObjectModel;

namespace SmsWpfApp.Services
{
    public class EnvironmentVariableService
    {
        #region fields
        
        private readonly ILogger _logger;
        private readonly List<string> _variableNames;

        #endregion

        #region ctor
        public EnvironmentVariableService(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;

            var section = configuration.GetSection("EnvironmentVariables");
            _variableNames = section.Get<List<string>>() ?? new List<string>();

            LoadVariables();
        }

        #endregion

        #region props
        
        public ObservableCollection<EnvironmentVariable> Variables { get; } = new();

        #endregion

        #region public methods

        public void LoadVariables()
        {
            Variables.Clear();

            foreach (var name in _variableNames)
            {
                var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User)
                    ?? Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine)
                    ?? string.Empty;

                Variables.Add(new EnvironmentVariable
                {
                    Name = name,
                    Value = value,
                    Exists = !string.IsNullOrEmpty(value)
                });

                _logger.Information("Загружена переменная: {VariableName} = {VariableValue}", name, value);
            }
        }

        public void SaveVariable(string name, string value)
        {
            try
            {
                Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);

                var variable = Variables.FirstOrDefault(v => v.Name == name);
                if (variable != null)
                {
                    variable.Value = value;
                    variable.Exists = true;
                }

                _logger.Information("Сохранена переменная: {VariableName} = {VariableValue}", name, value);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при сохранении переменной {VariableName}", name);
                throw;
            }
        }

        public void SaveAllVariables()
        {
            foreach (var variable in Variables)
            {
                SaveVariable(variable.Name, variable.Value);
            }
        }

        public void RestoreDefaults()
        {
            foreach (var variable in Variables)
            {
                variable.Value = string.Empty;
                variable.Exists = false;
                Environment.SetEnvironmentVariable(variable.Name, null, EnvironmentVariableTarget.User);
                _logger.Information("Сброшена переменная: {VariableName}", variable.Name);
            }
        }

        #endregion
    }
}
