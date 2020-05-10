using CleanCode.Results;

namespace CleanCode.Cli.Common
{
    public interface IVersionProvider
    {
        public Result<ToolMeta> GetLastVersion();
        public Result<None> DownloadAndExtractToDirectory(ToolMeta meta, IDirectory outDirectory);

        // public Result<None> UpdateIfNeed() TODO
        // {
        //     var meta = GetLastVersion();
        // }
    }
}