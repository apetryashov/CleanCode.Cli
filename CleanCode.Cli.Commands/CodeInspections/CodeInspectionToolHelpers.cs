using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.CodeInspections
{
    public class CodeInspectionToolHelpers
    {
        private static string PathToTransformSettings
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1");

        private static string PathToXsltFile
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt");

        public static Result<None> ConvertXmlReportToHtml(string pathToXmlReport, string outFileName)
            => Cmd.RunProcess(
                "powershell",
                $"{PathToTransformSettings} {pathToXmlReport} {PathToXsltFile} {outFileName}"
            );
    }
}