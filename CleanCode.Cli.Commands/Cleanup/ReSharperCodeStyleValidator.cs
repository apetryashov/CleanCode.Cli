using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.Cleanup
{
    internal class ReSharperCodeStyleValidator
    {
        private readonly IDirectory rootDirectory;

        public ReSharperCodeStyleValidator(IDirectory rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public Result<None> Run(FileInfo fileInfo, IReadOnlyCollection<FileInfo> validateFiles)
        {
            if (!fileInfo.Directory!.GetFiles("*.DotSettings").Any())
                return $"Folder: {fileInfo.Directory.FullName} should contain .DotSettings file";

            if (!validateFiles.Any())
                return Result.Ok();

            var progressBar = new FilesCheckingProgressBar(validateFiles);

            var relativeFilePaths = validateFiles.Select(file => file.GetRelativePath(fileInfo.Directory));
            return new ReSharperClt(rootDirectory)
                .RunCleanupTool(fileInfo.FullName, relativeFilePaths, progressBar.RegisterFile);
        }
    }
}