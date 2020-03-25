using System.Net;
using Newtonsoft.Json.Linq;

namespace CleanCode.Cli.Commands.UpdateTools
{
    public static class ResharperCltHelper
    {
        private const string MetaUrl =
            "https://data.services.jetbrains.com/products/releases?code=RSCLT&latest=true&type=release";

        public static ReSharperCltToolMeta GetInformationAboutLastVersion()
        {
            using var client = new WebClient();
            var jsonMeta = client.DownloadString(MetaUrl);
            var jObjectMeta = JObject.Parse(jsonMeta);

            var version = jObjectMeta.SelectToken("RSCLT.[0].version").Value<string>();
            var downloadUrl = jObjectMeta.SelectToken("RSCLT.[0].downloads.windows.link").Value<string>(); //TODO: сейчас будет работать только для windows

            return new ReSharperCltToolMeta(version, downloadUrl);
        }
    }
}