using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;
using Directory = System.IO.Directory;

namespace CleanCode.Cli.Commands.Cleanup
{
    [PublicAPI]
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    public class CleanupCommand : CleanupCommandOptions, ICommand
    {
        private readonly IDirectory cliDirectory;

        public CleanupCommand() => cliDirectory = new CleanCodeDirectory();

        public Result<None> Run() => new ResharperCltUpdater()
            .UpdateIfNeed()
            .Then(_ => FileUtils.GetPathToSlnFile(PathToSlnFolder ?? Directory.GetCurrentDirectory()))
            .Then(StartCleanup);

        private Result<None> StartCleanup(FileInfo sln)
        {
            var scanningFiles = GetFiles(sln.Directory);
            var scanningFilesWithHash = scanningFiles.Select(file => (file, hash: file.CalculateMd5Hash())).ToList();

            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return new ReSharperCodeStyleValidator(cliDirectory).Run(sln, scanningFiles)
                .Then(_ =>
                {
                    ConsoleHelper.ClearCurrentConsoleLine();
                    ConsoleHelper.LogInfo("Finish file checking");
                })
                .Then(_ => FilesHashCacheStorage.UpdateFilesHash(scanningFiles))
                .Then(_ => GetFailedFiles())
                .Then(failedFiles => FailIfHasDirtyFiles(sln.Directory, failedFiles));

            IEnumerable<FileInfo> GetFailedFiles() => scanningFilesWithHash
                .Where(x => x.hash != x.file.CalculateMd5Hash())
                .Select(x => x.file);
        }

        private IReadOnlyCollection<FileInfo> GetFiles(DirectoryInfo directory) => Force
            ? FileUtils.GetAllValuableCsFiles(directory)
            : FilesHashCacheStorage.GetNewAndUpdatedFiles(directory);

        private static Result<None> FailIfHasDirtyFiles(DirectoryInfo slnDirectory, IEnumerable<FileInfo> dirtyFiles)
        {
            var relativeFiles = dirtyFiles.Select(file => file.GetRelativePath(slnDirectory));

            var allDirtyFilesString = string.Join(Environment.NewLine, relativeFiles);

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