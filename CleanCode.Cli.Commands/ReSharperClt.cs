using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands
{
    public static class ReSharperClt
    {
        private static string ReSharperInspectCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\cleanupcode.exe");

        public static Result<None> RunInspectCodeTool(string pathToSlnFile, string outFile, Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperInspectCodeCli,
                $"{pathToSlnFile} --o={outFile}",
                callBack);

        public static Result<None> RunCleanupTool(string pathToSlnFile, IEnumerable<string> relativeFilePaths,
            Action<string> callBack) => Cmd.RunProcess(
            ReSharperCleanupCodeCli,
            $"{pathToSlnFile} --include=\"{string.Join(';', relativeFilePaths)}\"",
            callBack
        );
    }
}