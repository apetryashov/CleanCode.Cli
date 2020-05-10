using System;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Installer
{
    class Program
    {
        private static readonly IDirectory CliDirectory = new CleanCodeDirectory();

        static void Main(string[] args)
        {
            var versionProvider = new CleanCodeToolVersionProvider();

            versionProvider.GetLastVersion()
                .Then(meta => versionProvider.DownloadAndExtractToDirectory(meta, CliDirectory)
                    .Then(_ => SetToPathIfNeed())
                    .Then(_ => ConsoleHelper.LogInfo(
                        $"'clean-code' has been successfully installed. Current Version - {meta.Version}"))
                )
                .OnFail(ConsoleHelper.LogError);
        }

        private static void SetToPathIfNeed()
        {
            var name = "PATH";
            var scope = EnvironmentVariableTarget.User;
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            var cliDirectory = CliDirectory.GetPath();
            if (oldValue!.Contains(cliDirectory))
                return;
            var newValue = oldValue + $";{cliDirectory}";
            Environment.SetEnvironmentVariable(name, newValue, scope);
        }
    }
}