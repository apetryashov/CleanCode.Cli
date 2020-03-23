using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
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

            var shortFiles =
                validateFiles.Select(file
                        => Path.GetRelativePath(fileInfo.Directory.FullName, file))
                    .ToList(); //TODO: мне не нравки, что это здесь

            if (!shortFiles.Any())
                return Result.Ok();

            var progressBar = new FilesCheckingProgressBar(shortFiles);
            var args = GetArgs();
            return Cmd.RunProcess(ReSharperCleanupCodeCli, args, progressBar.RegisterFile)
                .Then(_ => ConsoleHelper.ClearCurrentConsoleLine())
                .Then(_ => ConsoleHelper.LogInfo("Finish file checking"));

            string GetArgs() => $"{fileInfo.FullName} --include=\"{string.Join(';', shortFiles)}\"";
        }
    }
}