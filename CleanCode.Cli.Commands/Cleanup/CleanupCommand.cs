using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.Cleanup
{
    //TODO: нужно добавить ключ --force
    [PublicAPI]
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    public class CleanupCommand : ICommand
    {
        [Option('s', "solution",
            Required = false,
            Default = ".",
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string PathToSlnFolder { get; set; } = ".";

        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed()
            .Then(_ => FileUtils.GetPathToSlnFile(PathToSlnFolder))
            .Then(StartCleanup);

        private static Result<None> StartCleanup(FileInfo sln)
        {
            var changedFiles = FilesHashCacheStorage.GetChangedFiles(sln.Directory);
            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return ReSharperCodeStyleValidator.Run(sln, changedFiles)
                .Then(_ => FilesHashCacheStorage.UpdateFilesHash(changedFiles))
                .Then(files => FailIfHasDirtyFiles(sln.Directory, files));
        }

        private static Result<None> FailIfHasDirtyFiles(DirectoryInfo slnDirectory, IEnumerable<FileInfo> dirtyFiles)
        {
            var relativeFiles = dirtyFiles.Select(file => file.GetRelativePath(slnDirectory));

            var allDirtyFilesString = string.Join("\r\n", relativeFiles);

            if (string.IsNullOrEmpty(allDirtyFilesString))
            {
                ConsoleHelper.LogInfo("All files are clean");
                return Result.Ok();
            }

            return $@"
Not all files are clean.
Failed files list:
{allDirtyFilesString}

You can restart the process and get successful result
";
        }
    }
}