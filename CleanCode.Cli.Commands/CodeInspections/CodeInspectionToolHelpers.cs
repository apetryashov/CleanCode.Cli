using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.CodeInspections
{
    public class CodeInspectionToolHelpers
    {
        private readonly IDirectory rootDirectory;

        private IDirectory transformSettingsReSharperCLT => rootDirectory
            .WithSubDirectory("Tool\\TransformSettingsReSharperCLT");

        public CodeInspectionToolHelpers(IDirectory rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        private string PathToTransformSettings
            => transformSettingsReSharperCLT
                .WithSubDirectory("Transform-Xslt.ps1")
                .GetPath();

        private string PathToXsltFile
            => transformSettingsReSharperCLT
                .WithSubDirectory("ic.xslt")
                .GetPath();

        public Result<None> ConvertXmlReportToHtml(string pathToXmlReport, string outFileName)
            => Cmd.RunProcess(
                "powershell",
                $"{PathToTransformSettings} {pathToXmlReport} {PathToXsltFile} {outFileName}"
            );
    }
}