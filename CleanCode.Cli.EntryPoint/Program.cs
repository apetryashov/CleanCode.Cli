using System.IO;
using System.Reflection;
using CleanCode.Cli;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using Newtonsoft.Json;

namespace CleanCode.Tool
{
    internal static class Program
    {
        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        private static readonly IDirectory CliDirectory = new CleanCodeDirectory();
        private static string DeveloperFlagsFile = "developer_flags.json";

        //TODO: Затащить это все в chocolatey
        //TODO: Добавить команду generate-dot-settings
        //TODO: Разделить логику и логирование
        //TODO: Добавить тесты
        //TODO: Перевести на DI (будет полезно для тестов)
        //TODO: Избавиться от всех try catch
        //TODO: Научить работать с ключем /p:PublishTrimmed=true
        private static int Main(string[] args)
        {
            if (NewVersionWasInstalled())
                return 0;

            var result = new CommandProvider().StartCommand(args)
                .OnFail(ConsoleHelper.LogError);

            return result.IsSuccess ? 0 : 1;
        }

        private static bool NewVersionWasInstalled()
        {
            var versionProvider = new CleanCodeToolVersionProvider(IsDeveloperMode());

            return versionProvider.GetLastVersion()
                .Then(meta =>
                {
                    if (meta.Version.Equals(CurrentVersion))
                        return false;

                    return versionProvider
                        .DownloadAndExtractToDirectory(meta, CliDirectory.WithSubDirectory("new-tool"))
                        .Then(_ => true);
                })
                .OnFail(ConsoleHelper.LogError)
                .GetValueOrThrow();
        }

        private static bool IsDeveloperMode()
        {
            var developerFile = CliDirectory.WithSubDirectory(DeveloperFlagsFile).GetPath();

            if (!File.Exists(developerFile))
                return false;

            var developerFlags = JsonConvert.DeserializeObject<DeveloperFlags>(File.ReadAllText(developerFile));

            if (developerFlags.DeveloperMode)
                ConsoleHelper.LogInfo("developer mode enabled");

            return developerFlags.DeveloperMode;
        }
    }
}