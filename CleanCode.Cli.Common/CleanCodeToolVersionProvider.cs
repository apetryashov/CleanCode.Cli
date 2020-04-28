using System.Net;
using CleanCode.Helpers;
using CleanCode.Results;
using Newtonsoft.Json.Linq;

namespace CleanCode.Cli.Common
{
    public class CleanCodeToolVersionProvider : IVersionProvider
    {
        private string allReleases = "https://api.github.com/repos/apetryashov/CleanCode.Cli/releases";

        public ToolMeta GetLastVersion()
        {
            using var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            var jsonMeta = client.DownloadString(allReleases);
            var allReleasesInformation = JArray.Parse(jsonMeta);
            var laseRelease = allReleasesInformation.First;
            var lastVersion = laseRelease.SelectToken("tag_name").Value<string>();
            var downloadUrl = laseRelease.SelectToken("assets[0].browser_download_url").Value<string>();

            return new ToolMeta
            {
                Version = lastVersion,
                DownloadUrl = downloadUrl
            };
        }

        public Result<None> DownloadAndExtractToDirectory(ToolMeta meta, IDirectory outDirectory) =>
            ZipHelper.DownloadAndExtractZipFile(meta.DownloadUrl, outDirectory.GetPath());
    }
}