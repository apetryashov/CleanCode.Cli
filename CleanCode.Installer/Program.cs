using System;
using CleanCode.Cli.Common;

namespace CleanCode.Installer
{
    class Program
    {
        private static readonly IDirectory CliDirectory = new CleanCodeDirectory();

        static void Main(string[] args)
        {
            var versionProvider = new CleanCodeToolVersionProvider();
            var meta = versionProvider.GetLastVersion();
            versionProvider.DownloadAndExtractToDirectory(meta, CliDirectory);
            SetToPathIfNeed();
        }

        private static void SetToPathIfNeed()
        {
            var name = "PATH";
            var scope = EnvironmentVariableTarget.Machine;
            var oldValue = Environment.GetEnvironmentVariable(name, scope);
            var cliDirectory = CliDirectory.GetPath();
            if (oldValue.Contains(cliDirectory))
                return;
            var newValue = oldValue + $";{cliDirectory}";
            Environment.SetEnvironmentVariable(name, newValue, scope);
        }
    }
}