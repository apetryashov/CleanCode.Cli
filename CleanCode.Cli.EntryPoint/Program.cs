using System;
using System.Reflection;
using CleanCode.Cli;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Tool
{
    internal static class Program
    {
        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        //TODO: Затащить это все в chocolatey
        //TODO: Добавить команду generate-dot-settings
        //TODO: Разделить логику и логирование
        //TODO: Добавить тесты
        //TODO: Перевести на DI (будет полезно для тестов)
        //TODO: Добавить автообновление утилиты
        //TODO: Избавиться от всех try catch
        private static void Main(string[] args)
        {
            UpdateIfNeed();
            new CommandProvider().StartCommand(args);
        }

        private static void UpdateIfNeed()
        {
            var versionProvider = new CleanCodeToolVersionProvider(false);

            versionProvider
                .GetLastVersion()
                .Then(meta => meta.Version.Equals(CurrentVersion)
                    ? Result.Ok()
                    : versionProvider.DownloadAndExtractToDirectory(meta, new CleanCodeDirectory()))
                .OnFail(ConsoleHelper.LogError);
        }
    }
}