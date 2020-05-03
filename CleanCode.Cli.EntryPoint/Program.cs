using System.Reflection;
using CleanCode.Cli;
using CleanCode.Cli.Common;
using CleanCode.Helpers;

namespace CleanCode.Tool
{
    internal static class Program
    {
        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        //TODO: Затащить это все в chocolatey
        //TODO: Добавить команду generate-dot-settings
        //TODO: Разделить логику и логирование
        //TODO: Добавить тесты
        //TODO: Перевести на DI (будет полезно для тестов)
        //TODO: Добавить автообновление утилиты
        private static void Main(string[] args)
        {
            UpdateIfNeed();
            new CommandProvider().StartCommand(args);
        }

        private static void UpdateIfNeed()
        {
            var versionProvider = new CleanCodeToolVersionProvider();
            var meta = versionProvider.GetLastVersion();

            if (meta.Version.Equals(CurrentVersion))
                return;
            versionProvider.DownloadAndExtractToDirectory(meta, new CleanCodeDirectory());

            ConsoleHelper.LogInfo($"New cli version will be installed. New version {meta.Version}");
        }
    }
}