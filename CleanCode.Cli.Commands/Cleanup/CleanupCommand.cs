using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.Cleanup
{
    [PublicAPI]
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    public class CleanupCommand : ICommand
    {
        [Option('s', "solution",
            Required = false,
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string? PathToSlnFolder { get; set; }

        [Option('f', "force",
            Required = false,
            HelpText = "State force cleanup. It is slow but will check all files again")]
        public bool Force { get; set; }

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

        #region Examples

        [Usage(ApplicationAlias = "clean-code")]
        public static IEnumerable<Example> Examples => new List<Example>()
        {
            new Example("Start cleanup in current directory", new CleanupCommand()),
            new Example("Start cleanup in given directory",
                new CleanupCommand {PathToSlnFolder = "<path to .sln file>"}),
            new Example("Start cleanup without cache", new CleanupCommand {Force = true}),
        };

        #endregion
    }
}