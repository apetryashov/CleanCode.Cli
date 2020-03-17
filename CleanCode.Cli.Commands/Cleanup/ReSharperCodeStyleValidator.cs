using System.IO;
using System.Linq;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.Cleanup
{
    internal static class ReSharperCodeStyleValidator
    {
        public static Result<None> Run(FileInfo fileInfo)
        {
            return fileInfo.Directory!.GetFiles("*.DotSettings").Any()
                ? Cmd.RunProcess(ReSharperCleanupCodeCli, fileInfo.FullName)
                : $"Folder: {fileInfo.Directory.FullName} should contain .DotSettings file";
        }

        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\cleanupcode.exe");
    }
}