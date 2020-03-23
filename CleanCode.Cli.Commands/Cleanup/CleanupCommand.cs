using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands.Cleanup
{
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
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

        private Result<None> StartCleanup(FileInfo sln)
        {
            var changedFiles = FilesHashCacheStorage.GetChangedFiles(sln.Directory);
            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return ReSharperCodeStyleValidator.Run(sln, changedFiles)
                .Then(files => FilesHashCacheStorage.UpdateFilesHash(changedFiles))
                .Then(files => files.Select(file => Path.GetRelativePath(sln.Directory.FullName, file)))
                .Then(files => FailIfHasDirtyFiles(files.ToList()));
        }

        private static Result<None> FailIfHasDirtyFiles(IEnumerable<string> dirtyFiles)
        {
            var allDirtyFilesString = string.Join("\r\n", dirtyFiles);

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