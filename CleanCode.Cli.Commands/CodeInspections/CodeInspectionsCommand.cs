using System.Linq;
using System.Xml;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;

namespace CleanCode.Cli.Commands.CodeInspections
{
    public class CodeInspectionsCommand : ICommand
    {
        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private static string PathToTransformSettings
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1");

        private static string PathToXsltFile
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt");

        private readonly string pathToSlnFolder;

        public CodeInspectionsCommand(CodeInspectionsCommandOptions options)
        {
            pathToSlnFolder = options.PathToSlnFolder!;
        }

        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed()
            .Then(_ => FileUtils.GetPathToSlnFile(pathToSlnFolder))
            .Then(sln =>
            {
                using var tempDir = new TempDirectory();
                var tempFile = $"{tempDir.PathToTempDirectory}/temp";

                ConsoleHelper.LogInfo("Start code inspection. Please waiting.");
                return Cmd.RunProcess(ReSharperCleanupCodeCli, ArgsForCodeInspections(tempFile, sln.FullName))
                    .Then(__ => CheckXmlReport(tempFile));
            });

        private static string ArgsForCodeInspections(string outFile, string slnFile) => $"--o={outFile} {slnFile}";

        private static Result<None> CheckXmlReport(string pathToXmlReport)
        {
            var doc = new XmlDocument();
            doc.Load(pathToXmlReport);
            var failFiles = doc.SelectNodes("Report//Issues//Project//Issue")
                .Cast<XmlElement>()
                .Select(x => x.Attributes["File"].Value)
                .GroupBy(x => x)
                .ToList();

            if (!failFiles.Any())
            {
                ConsoleHelper.LogInfo("All files are clean");
                return Result.Ok();
            }

            return Cmd.RunProcess("powershell", ArgsForHtmlConverter(pathToXmlReport))
                .Then(_ =>
                {
                    var failFilesString = failFiles.Select(x => $"{x.Key} [{x.Count()} problem]").ToArray();
                    return Result.Fail<None>(
                        $@"Not all files are clean. Failed files list:
{string.Join("\r\n", failFilesString)}

You can find the full code inspection report here: code-inspections.html"
                    );
                });
        }

        private static string ArgsForHtmlConverter(string xmpReportPath)
            => $"{PathToTransformSettings} {xmpReportPath} {PathToXsltFile} code-inspections.html";
        
    }
}