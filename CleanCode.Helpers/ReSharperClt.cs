using System;
using CleanCode.Cli;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class ReSharperClt
    {
        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private static string PathToTransformSettings
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1");

        private static string PathToXsltFile
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt");

        public static Result<None> RunCleanupCodeTool(string pathToSlnFile, string outFile, Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperCleanupCodeCli,
                $"{pathToSlnFile} --o={outFile}",
                callBack);

        public static Result<None> ConvertXmlReportToHtml(string pathToXmlReport)
            => Cmd.RunProcess(
                "powershell",
                $"{PathToTransformSettings} {pathToXmlReport} {PathToXsltFile} code-inspections.html"
            );
    }
}