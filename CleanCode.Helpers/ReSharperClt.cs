using System;
using CleanCode.Cli;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class ReSharperClt
    {
        private static string ReSharperInspectCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private static string PathToTransformSettings
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1");

        private static string PathToXsltFile
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt");

        public static Result<None> RunInspectCodeTool(string pathToSlnFile, string outFile, Action<string> callBack)
            => Cmd.RunProcess(
                ReSharperInspectCodeCli,
                $"{pathToSlnFile} --o={outFile}",
                callBack);

        public static Result<None> ConvertXmlReportToHtml(string pathToXmlReport)
            => Cmd.RunProcess(
                "powershell",
                $"{PathToTransformSettings} {pathToXmlReport} {PathToXsltFile} code-inspections.html"
            );
    }
}