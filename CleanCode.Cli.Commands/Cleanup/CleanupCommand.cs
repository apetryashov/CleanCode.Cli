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

namespace CleanCode.Cli.Commands.Cleanup
{
    [PublicAPI]
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    public class CleanupCommand : CleanupCommandOptions, ICommand
    {
        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed()
            .Then(_ => FileUtils.GetPathToSlnFile(PathToSlnFolder ?? Directory.GetCurrentDirectory()))
            .Then(StartCleanup);

        private Result<None> StartCleanup(FileInfo sln)
        {
            var scannedFiles = Force
                ? FileUtils.GetAllValuableCsFiles(sln.Directory)
                : FilesHashCacheStorage.GetChangedFiles(sln.Directory);

            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return ReSharperCodeStyleValidator.Run(sln, scannedFiles)
                .Then(_ => FilesHashCacheStorage.UpdateFilesHash(scannedFiles))
                .Then(files => FailIfHasDirtyFiles(sln.Directory, files));
        }

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