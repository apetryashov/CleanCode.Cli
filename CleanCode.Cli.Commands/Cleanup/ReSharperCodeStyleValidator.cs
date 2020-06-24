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
        private readonly DotSettingsFileProvider dotSettingsFileProvider;
        private readonly IDirectory rootDirectory;

        public ReSharperCodeStyleValidator(IDirectory rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            dotSettingsFileProvider = new DotSettingsFileProvider();
        }

        public Result<None> Run(FileInfo fileInfo, IReadOnlyCollection<FileInfo> validateFiles)
        {
            if (!validateFiles.Any())
                return Result.Ok();

            var progressBar = new FilesCheckingProgressBar(validateFiles);

            var relativeFilePaths = validateFiles.Select(file => file.GetRelativePath(fileInfo.Directory));

            return dotSettingsFileProvider.GetDotSettingsFile(fileInfo)
                .Then(dsFile => new ReSharperClt(rootDirectory)
                    .RunCleanupTool(fileInfo.FullName, relativeFilePaths, dsFile.GetPath(), progressBar.RegisterFile));
        }
    }
}