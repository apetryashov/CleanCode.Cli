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

        public static Result<None> Run(FileInfo fileInfo, IReadOnlyCollection<FileInfo> validateFiles)
        {
            if (!fileInfo.Directory!.GetFiles("*.DotSettings").Any())
                return $"Folder: {fileInfo.Directory.FullName} should contain .DotSettings file";

            if (!validateFiles.Any())
                return Result.Ok();

            var progressBar = new FilesCheckingProgressBar(validateFiles);

            var relativeFilePaths = validateFiles.Select(file => file.GetRelativePath(fileInfo.Directory));
            return Cmd.RunProcess(ReSharperCleanupCodeCli, GetArgs(), progressBar.RegisterFile)
                .Then(_ => ConsoleHelper.ClearCurrentConsoleLine()) //TODO
                .Then(_ => ConsoleHelper.LogInfo("Finish file checking"));

            string GetArgs() => $"{fileInfo.FullName} --include=\"{string.Join(';', relativeFilePaths)}\"";
        }
    }
}