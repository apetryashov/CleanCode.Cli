using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.CodeInspections
{
    public class CodeInspectionToolHelpers
    {
        private readonly IDirectory rootDirectory;

        public CodeInspectionToolHelpers(IDirectory rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }
        
        private string PathToTransformSettings
            => rootDirectory
                .WithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1")
                .GetPath();

        private string PathToXsltFile
            =>  rootDirectory
                .WithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt")
                .GetPath();

        public Result<None> ConvertXmlReportToHtml(string pathToXmlReport, string outFileName)
            => Cmd.RunProcess(
                "powershell",
                $"{PathToTransformSettings} {pathToXmlReport} {PathToXsltFile} {outFileName}"
            );
    }
}