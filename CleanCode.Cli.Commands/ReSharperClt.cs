using System;
using System.Collections.Generic;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands
{
    public class ReSharperClt
    {
        private readonly IDirectory rootDirectory;

        private IDirectory UtilDirectory
            => rootDirectory.WithSubDirectory("Utils").WithSubDirectory("resharper-clt");

        private IDirectory ReSharperInspectCodeCli
            => UtilDirectory.WithSubDirectory("inspectcode.exe");

        private IDirectory ReSharperCleanupCodeCli
            => UtilDirectory.WithSubDirectory("cleanupcode.exe");
        
        public ReSharperClt(IDirectory rootDirectory) => this.rootDirectory = rootDirectory;

        public Result<None> RunInspectCodeTool(string pathToSlnFile, string outFile, Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperInspectCodeCli.GetPath(),
                $"{pathToSlnFile} --o={outFile}",
                callBack);

        public Result<None> RunCleanupTool(string pathToSlnFile, IEnumerable<string> relativeFilePaths,
            Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperCleanupCodeCli.GetPath(),
                $"{pathToSlnFile} --include=\"{string.Join(';', relativeFilePaths)}\"",
                callBack
            );
    }
}