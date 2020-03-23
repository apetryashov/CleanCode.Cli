using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CleanCode.Cli.Commands.UpdateTools
{
    //TODO: тут тоже нужно рефакторить
    //TODO: очищать кэш при установке новой версии
    [PublicAPI]
    [Verb("update", HelpText = "Check new resharper-clt version and install if need")]
    public static class ResharperCltUpdater
    {
        private const string MetaUrl =
            "https://data.services.jetbrains.com/products/releases?code=RSCLT&latest=true&type=release";

        private static readonly string ToolDir = CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt");
        private static string PathToStateFile => CleanCodeDirectory.GetWithSubDirectory("State.json");

        public static Result<None> UpdateIfNeed()
        {
            var (version, downloadUrl) = GetMeta();

            if (!NeedUpdate(version))
            {
                ConsoleHelper.LogInfo($"You have the last version of resharper-clt. Version - {version}");
                return Result.Ok();
            }

            return Update(downloadUrl)
                .Then(_ => UpdateState(version));
        }

        private static bool NeedUpdate(string currentVersion)
        {
            var pathToState = CleanCodeDirectory.GetWithSubDirectory(PathToStateFile);
            if (!File.Exists(pathToState))
                return true;

            var stateFile = File.ReadAllText(pathToState);
            var state = JsonConvert.DeserializeObject<State>(stateFile);

            return state.Version != currentVersion;
        }

        private static void UpdateState(string newVersion)
        {
            var newState = new State {Version = newVersion};

            File.WriteAllText(PathToStateFile, JsonConvert.SerializeObject(newState));
            ConsoleHelper.LogInfo(
                $"A new version of resharper-clt was installed. New version - {newVersion}");
        }

        private static Result<None> Update(string downloadUrl)
        {
            ConsoleHelper.LogInfo("Please wait. A New version of resharper-clt will be installed in a few seconds");

            using var tempDir = new TempDirectory();
            using var client = new WebClient();

            var tempFileName = $"{Guid.NewGuid()}.zip";
            var fileName = Path.Combine(tempDir.PathToTempDirectory, tempFileName);
            client.DownloadFile(downloadUrl, fileName);
            ZipFile.ExtractToDirectory(fileName, ToolDir, Encoding.Default);

            return Result.Ok();
        }

        private static (string version, string downloadUrl) GetMeta()
        {
            using var client = new WebClient();
            var jsonMeta = client.DownloadString(MetaUrl);
            var jObjectMeta = JObject.Parse(jsonMeta);

            var version = jObjectMeta.SelectToken("RSCLT.[0].version").Value<string>();
            var downloadUrl = jObjectMeta.SelectToken("RSCLT.[0].downloads.windows.link").Value<string>();

            return (version, downloadUrl);
        }

        private class State
        {
            public string? Version { get; set; }
        }
    }
}