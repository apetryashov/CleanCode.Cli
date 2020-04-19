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

        private IDirectory ReSharperInspectCodeCli
            => rootDirectory.WithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private IDirectory ReSharperCleanupCodeCli
            => rootDirectory.WithSubDirectory("Tools\\resharper-clt\\cleanupcode.exe");


        public ReSharperClt(IDirectory rootDirectory) => this.rootDirectory = rootDirectory;

        public Result<None> RunInspectCodeTool(string pathToSlnFile, string outFile, Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperInspectCodeCli.GetPath(),
                $"{pathToSlnFile} --o={outFile}",
                callBack);

        public Result<None> RunCleanupTool(string pathToSlnFile, IEnumerable<string> relativeFilePaths,
            Action<string> callBack) => Cmd.RunProcess(
            ReSharperCleanupCodeCli.GetPath(),
            $"{pathToSlnFile} --include=\"{string.Join(';', relativeFilePaths)}\"",
            callBack
        );
    }
}