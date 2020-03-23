using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands.CodeInspections
{
    [Verb("code-inspections", HelpText = "Start ReSharper code-inspection tool for given directory")]
    public class CodeInspectionsCommand : ICommand
    {
        [Option('s', "solution",
            Required = false,
            Default = ".",
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string PathToSlnFolder { get; set; } = ".";
        
        private static string ReSharperCleanupCodeCli
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\resharper-clt\\inspectcode.exe");

        private static string PathToTransformSettings
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\Transform-Xslt.ps1");

        private static string PathToXsltFile
            => CleanCodeDirectory.GetWithSubDirectory("Tools\\TransformSettingsReSharperCLT\\ic.xslt");
        
        private static readonly Regex ExtractCsFile = new Regex("(?<=)(\\w*\\.cs)$", RegexOptions.Compiled);
        

        public Result<None> Run()
        {
            var files = FileUtils.GetAllValuableCsFiles(new DirectoryInfo(PathToSlnFolder)).ToList();
            var progressBar = new FilesCheckingProgressBar(files);

            return ResharperCltUpdater.UpdateIfNeed()
                .Then(_ => FileUtils.GetPathToSlnFile(PathToSlnFolder))
                .Then(sln =>
                {
                    using var tempDir = new TempDirectory();
                    var tempFile = $"{tempDir.PathToTempDirectory}/temp";

                    ConsoleHelper.LogInfo("Start code inspection. Please waiting.");
                    var args = ArgsForCodeInspections(tempFile, sln.FullName);
                    return Cmd.RunProcess(ReSharperCleanupCodeCli, args, progressBar.RegisterFile)
                        .Then(_ => ConsoleHelper.ClearCurrentConsoleLine())
                        .Then(_ => CheckXmlReport(tempFile))
                        .Then(_ => ConsoleHelper.LogInfo("All files are clean"));
                });
        }

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
                return Result.Ok();

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