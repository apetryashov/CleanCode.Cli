using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.Cleanup
{
    internal static class ReSharperCodeStyleValidator
    {
        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\cleanupcode.exe");

        public static Result<None> Run(FileInfo fileInfo, IEnumerable<string> validateFiles)
        {
            if (!fileInfo.Directory!.GetFiles("*.DotSettings").Any())
                return $"Folder: {fileInfo.Directory.FullName} should contain .DotSettings file";

            var progressBar = new FilesCheckingProgressBar(validateFiles);
            return Cmd.RunProcess(ReSharperCleanupCodeCli, fileInfo.FullName, progressBar.RegisterFile)
                .Then(_ => ConsoleHelper.ClearCurrentConsoleLine())
                .Then(_ => ConsoleHelper.LogInfo("Finish file checking"));
        }
    }
}