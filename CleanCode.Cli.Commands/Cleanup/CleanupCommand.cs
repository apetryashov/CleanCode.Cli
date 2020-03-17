using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.Cleanup
{
    public class CleanupCommand : ICommand
    {
        private readonly string PathToSlnFolder;

        public CleanupCommand(CleanupCommandOptions options)
        {
            PathToSlnFolder = options.PathToSlnFolder;
        }

        public Result<None> Run()
        {
            return ResharperCltUpdater.UpdateIfNeed()
                .Then(_ => FileUtils.GetPathToSlnFile(PathToSlnFolder))
                .Then(StartCleanup);
        }

        private Result<None> StartCleanup(FileInfo sln)
        {
            var files = FileUtils.GetAllValuableCsFiles(sln.Directory);
            var oldFilesHashes = files.ToDictionary(x => x, FileUtils.CalculateFileHash);
            ConsoleHelper.LogInfo("Start cleanup. Please waiting.");

            return ReSharperCodeStyleValidator.Run(sln)
                .Then(__ => GetDirtyFiles())
                .Then(FailIfHasDirtyFiles);

            IReadOnlyCollection<string> GetDirtyFiles()
            {
                ConsoleHelper.LogInfo("Start files hash checking");
                return files.Where(file => !IsCleanFile(file))
                    .Select(file => file.Remove(0, PathToSlnFolder.Length + 1))
                    .ToList();

                bool IsCleanFile(string file)
                    => oldFilesHashes[file] == FileUtils.CalculateFileHash(file);
            }

            static Result<None> FailIfHasDirtyFiles(IReadOnlyCollection<string> dirtyFiles)
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
}