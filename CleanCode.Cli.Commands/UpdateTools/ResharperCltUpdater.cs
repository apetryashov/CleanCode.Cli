using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.UpdateTools
{
    [PublicAPI]
    [Verb("update", HelpText = "Check new resharper-clt version and install if need")]
    public class ResharperCltUpdater
    {
        private readonly IDirectory rootDirectory;
        private IDirectory ToolDir => rootDirectory.WithSubDirectory("Tools\\resharper-clt");

        private const string StateCollectionName = "State";

        public ResharperCltUpdater() => rootDirectory = new CleanCodeDirectory();

        public Result<None> UpdateIfNeed(bool force = false)
        {
            var meta = ResharperCltHelper.GetInformationAboutLastVersion();

            if (!force && !NeedUpdate(meta.Version))
            {
                ConsoleHelper.LogInfo($"You have the last version of resharper-clt. Version - {meta.Version}");
                return Result.Ok();
            }

            ConsoleHelper.LogInfo("Please wait. A New version of resharper-clt will be installed in a few seconds");
            return Update(meta);
        }

        public Result<None> Update(ReSharperCltToolMeta meta) => ZipHelper
            .DownloadAndExtractZipFile(meta.DownloadUrl, ToolDir.GetPath())
            .Then(_ => UpdateState(meta.Version))
            .Then(_ =>
            {
                FilesHashCacheStorage.ClearCache();
                ConsoleHelper.LogInfo("The file cache has been cleared :(");
            });

        private static bool NeedUpdate(string currentVersion)
        {
            var lastInstalledVersion = GerLastInstalledVersion();

            return lastInstalledVersion == null || lastInstalledVersion.Version != currentVersion;
        }

        private static State GerLastInstalledVersion()
        {
            using var db = LiteDbHelper.DataBase;

            return db.GetCollection<State>(StateCollectionName).Query().FirstOrDefault();
        }

        private static void UpdateState(string newVersion)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<State>(StateCollectionName);
            collection.Upsert(new State {Version = newVersion});

            ConsoleHelper.LogInfo(
                $"A new version of resharper-clt was installed. New version - {newVersion}");
        }

        private class State
        {
#nullable disable //because initialized from db
            public string Version { get; set; }
        }
    }
}