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
        private IDirectory ToolDir => rootDirectory.WithSubDirectory("Utils\\resharper-clt");

        private const string StateCollectionName = "State";
        private IVersionProvider versionProvider = new ResharperCltVersionProvider();

        public ResharperCltUpdater() => rootDirectory = new CleanCodeDirectory();

        //TODO: плохо это, что метод IfNeed, но мы все равно можем это обойти с помощью ключа
        public Result<None> UpdateIfNeed(bool force = false)
            => versionProvider.GetLastVersion()
                .Then(meta =>
                {
                    if (!force && !NeedUpdate(meta.Version))
                    {
                        ConsoleHelper.LogInfo($"You have the last version of resharper-clt. Version - {meta.Version}");
                        return Result.Ok();
                    }

                    ConsoleHelper.LogInfo(
                        "Please wait. A New version of 'resharper-clt' will be installed in a few seconds");
                    return Update(meta);
                });

        public Result<None> Update(ToolMeta meta) =>
            versionProvider.DownloadAndExtractToDirectory(meta, ToolDir)
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

        //TODO: надо это унести
        private static State GerLastInstalledVersion()
        {
            using var db = LiteDbHelper.DataBase;

            return db.GetCollection<State>(StateCollectionName).FindById("state");
        }

        private static void UpdateState(string newVersion)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<State>(StateCollectionName);
            collection.Upsert("state", new State {Version = newVersion});

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