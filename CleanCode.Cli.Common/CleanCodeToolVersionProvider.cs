using System.Linq;
using System.Threading.Tasks;
using CleanCode.Helpers;
using CleanCode.Results;
using Octokit;

namespace CleanCode.Cli.Common
{
    public class CleanCodeToolVersionProvider : IVersionProvider
    {
        private readonly bool downloadPrerelease;
        private readonly string toolZipName = "clean-code.zip";

        public CleanCodeToolVersionProvider(bool downloadPrerelease = false)
        {
            this.downloadPrerelease = downloadPrerelease;
        }

        public Result<ToolMeta> GetLastVersion()
        {
            try
            {
                var release = GetRelease().Result;
                var releaseVersion = release.TagName;
                var asset = release.Assets.FirstOrDefault(x => x.Name == toolZipName);

                if (asset == null)
                    return $"Can't find clean-code.zip file in release assets. Release version: {releaseVersion}";

                return new ToolMeta
                {
                    Version = releaseVersion,
                    DownloadUrl = asset.BrowserDownloadUrl
                };
            }
            catch
            {
                return "Something went wrong. Check your internet connection.";
            }
        }

        private async Task<Release> GetRelease()
        {
            var client = new GitHubClient(new ProductHeaderValue("clean-code.tool"));
            if (!downloadPrerelease)
                return await client.Repository.Release.GetLatest("apetryashov", "CleanCode.Cli");

            var releases = await client.Repository.Release.GetAll("apetryashov", "CleanCode.Cli");

            return releases.First();
        }

        public Result<None> DownloadAndExtractToDirectory(ToolMeta meta, IDirectory outDirectory) =>
            ZipHelper.DownloadAndExtractZipFile(meta.DownloadUrl, $"{outDirectory.GetPath()}")
                .Then(_ => ConsoleHelper.LogInfo($"New clean-code.tool was installed. New version: {meta.Version}"));
    }
}