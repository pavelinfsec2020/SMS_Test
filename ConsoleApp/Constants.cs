namespace ConsoleApp
{
    public static class Constants
    {
        public const string LogFilePrefix = "test-sms-console-app-";
        public const string LogFileExtension = ".log";
        public const string DateFormat = "yyyyMMdd";
        public const string AppSettingsFile = "appsettings.json";

        public static class ApiTypes
        {
            public const string Http = "Http";
            public const string Grpc = "Grpc";
        }

        public static class Messages
        {
            public const string EnterOrder = "Введите позиции заказа (Код:Количество;Код2:Количество2;...):";
            public const string InputEmpty = "Ввод не может быть пустым";
            public const string CodeNotFound = "Один или несколько кодов не найдены в меню";
            public const string OrderCreated = "Заказ успешно сформирован";
            public const string Success = "SUCCESS";
            public const string ProgramTerminated = "Program terminated: {Message}";
            public const string MenuHeader = "Список блюд:";
            public const string MenuFormat = "{Name} – {Article} – {Price} руб.";
        }

        public static class Errors
        {
            public const string ParseError = "Ошибка при разборе ввода: {Message}";
            public const string InvalidFormat = "Неверный формат: {pair}";
            public const string InvalidQuantity = "Неверное количество: {quantity}";
            public const string QuantityPositive = "Количество должно быть больше нуля: {quantity}";
        }
    }
}
