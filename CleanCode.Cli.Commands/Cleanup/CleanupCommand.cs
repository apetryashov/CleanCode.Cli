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
            var oldFilesHashes = FileUtils.GetAllValuableCsFiles(sln.Directory)
                .ToDictionary(x => x, FileUtils.CalculateFileHash);
            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return ReSharperCodeStyleValidator.Run(sln, oldFilesHashes.Keys)
                .Then(__ => GetDirtyFiles(oldFilesHashes))
                .Then(FailIfHasDirtyFiles);
        }

        private IReadOnlyCollection<string> GetDirtyFiles(IReadOnlyDictionary<string, string> oldFilesHashes)
        {
            return oldFilesHashes.Keys.Where(file => !IsCleanFile(file))
                .Select(file => file.Remove(0, PathToSlnFolder.Length + 1))
                .ToList();

            bool IsCleanFile(string file)
                => oldFilesHashes[file] == FileUtils.CalculateFileHash(file);
        }

        private static Result<None> FailIfHasDirtyFiles(IReadOnlyCollection<string> dirtyFiles)
        {
            if (dirtyFiles.Any())
                return $@"
Not all files are clean.
Failed files list:
{string.Join("\r\n", dirtyFiles)}

You can restart the process and get successful result
";
            ConsoleHelper.LogInfo("All files are clean");
            return Result.Ok();
        }
    }
}