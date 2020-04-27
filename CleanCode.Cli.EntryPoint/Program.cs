using CleanCode.Cli;

namespace CleanCode.Tool
{
    internal static class Program
    {
        //TODO: Затащить это все в chocolatey
        //TODO: Добавить команду generate-dot-settings
        //TODO: Разделить логику и логирование
        //TODO: Добавить тесты
        //TODO: Перевести на DI (будет полезно для тестов)
        private static void Main(string[] args)
        {
            new CommandProvider().StartCommand(args);
        }
    }
}