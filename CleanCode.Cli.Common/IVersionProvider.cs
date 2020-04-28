using CleanCode.Results;

namespace CleanCode.Cli.Common
{
    public interface IVersionProvider
    {
        public ToolMeta GetLastVersion();
        public Result<None> DownloadAndExtractToDirectory(ToolMeta meta, IDirectory outDirectory);
    }
}