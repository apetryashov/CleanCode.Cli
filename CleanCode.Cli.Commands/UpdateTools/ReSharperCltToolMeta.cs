namespace CleanCode.Cli.Commands.UpdateTools
{
    public class ReSharperCltToolMeta
    {
        public ReSharperCltToolMeta(string version, string downloadUrl)
        {
            Version = version;
            DownloadUrl = downloadUrl;
        }

        public string Version { get;  }
        public string DownloadUrl { get; }
    }
}