using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.Cleanup
{
    internal static class ReSharperCodeStyleValidator
    {
        public static Result<None> Run(FileInfo fileInfo, IReadOnlyCollection<FileInfo> validateFiles)
        {
            if (!fileInfo.Directory!.GetFiles("*.DotSettings").Any())
                return $"Folder: {fileInfo.Directory.FullName} should contain .DotSettings file";

            if (!validateFiles.Any())
                return Result.Ok();

            var progressBar = new FilesCheckingProgressBar(validateFiles);

            var relativeFilePaths = validateFiles.Select(file => file.GetRelativePath(fileInfo.Directory));
            return ReSharperClt.RunCleanupTool(fileInfo.FullName, relativeFilePaths, progressBar.RegisterFile)
                .Then(_ => ConsoleHelper.ClearCurrentConsoleLine())
                .Then(_ => ConsoleHelper.LogInfo("Finish file checking"));
        }
    }
}