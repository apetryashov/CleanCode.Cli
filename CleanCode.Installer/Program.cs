using System;
using System.IO;
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
                .Then(
                    meta => versionProvider.DownloadAndExtractToDirectory(meta, CliDirectory.WithSubDirectory("Tool")))
                .Then(_ => CreateRunFile())
                .Then(_ => SetToPathIfNeed())
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

        private static void CreateRunFile()
        {
            const string runCmdCommand = @"
@echo off
SET exit_code=%errorlevel%

""%~dp0\Tool\clean-code.exe"" %*
if exist %~dp0\new-tool (
	rmdir %~dp0\Tool /S /Q > nul
	ren %~dp0\new-tool Tool > nul
    ""%~dp0\Tool\clean-code.exe"" %*
)

cmd /C exit %exit_code% > nul
";
            File.WriteAllText(CliDirectory.WithSubDirectory("clean-code.cmd").GetPath(), runCmdCommand);
        }
    }
}