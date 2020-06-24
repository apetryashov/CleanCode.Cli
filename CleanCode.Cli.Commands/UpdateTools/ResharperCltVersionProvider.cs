using System.Net;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using Newtonsoft.Json.Linq;

namespace CleanCode.Cli.Commands.UpdateTools
{
    public class ResharperCltVersionProvider : IVersionProvider
    {
        private const string MetaUrl =
            "https://data.services.jetbrains.com/products/releases?code=RSCLT&latest=true&type=release";

        public Result<ToolMeta> GetLastVersion()
        {
            try
            {
                using var client = new WebClient();
                var jsonMeta = client.DownloadString(MetaUrl);
                var jObjectMeta = JObject.Parse(jsonMeta);

                var version = jObjectMeta.SelectToken("RSCLT.[0].version")!.Value<string>();
                var downloadUrl =
                    jObjectMeta.SelectToken("RSCLT.[0].downloads.windows.link")!
                        .Value<string>(); //TODO: сейчас будет работать только для windows

                return new ToolMeta
                {
                    Version = version,
                    DownloadUrl = downloadUrl
                };
            }
            catch
            {
                return "Something went wrong. Check your internet connection.";
            }
        }

        public Result<None> DownloadAndExtractToDirectory(ToolMeta meta, IDirectory outDirectory)
            => ZipHelper.DownloadAndExtractZipFile(meta.DownloadUrl, outDirectory.GetPath());
    }
}