using CleanCode.Cli;

namespace CleanCode.Tool
{
    //TODO: Затащить это все в chocolatey
    //TODO: Добавить команду generate-dot-settings
    //TODO: Разделить логику и логирование
    //TODO: Добавить тесты
    //TODO: Перевести на DI (будет полезно для тестов)
    //TODO: Добавить автооткрытие репорта после завершения
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CommandProvider.StartCommand(args);
        }
    }    
}