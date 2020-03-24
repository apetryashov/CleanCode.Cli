using CleanCode.Cli.Commands;

namespace CleanCode.Tool
{
    //TODO: Затащить это все в chocolatey
    //TODO: Добавить команду generate-dot-settings
    //TODO: Разделить логику и логирование
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CommandProvider.StartCommand(args);
        }
    }    
}