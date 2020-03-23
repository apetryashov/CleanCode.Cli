using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class CodeInspectionsCommand : ICommand
    {
        [Option('s', "solution",
            Required = false,
            Default = ".",
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string PathToSlnFolder { get; set; } = ".";

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

                    return ReSharperCltHelper.RunCleanupCodeTool(tempFile, sln.FullName, progressBar.RegisterFile)
                        .Then(_ => ConsoleHelper.ClearCurrentConsoleLine())
                        .Then(_ => CheckXmlReport(tempFile))
                        .Then(_ => ConsoleHelper.LogInfo("All files are clean"));
                });
        }

        private static Result<None> CheckXmlReport(string pathToXmlReport)
        {
            var failFiles = GetFailFilesFromXmlReport(pathToXmlReport);

            if (!failFiles.Any())
                return Result.Ok();

            return ReSharperCltHelper.ConvertXmlReportToHtml(pathToXmlReport)
                .Then(_ => GetErrorFilesAsFailResult());

            Result<None> GetErrorFilesAsFailResult()
            {
                var failFilesString = failFiles.Select(x => $"{x.fileName} [{x.errorsCount} problem]").ToArray();
                return Result.Fail<None>(
                    $@"Not all files are clean. Failed files list:
{string.Join("\r\n", failFilesString)}

You can find the full code inspection report here: code-inspections.html"
                );
            }
        }

        private static IReadOnlyCollection<(string fileName, int errorsCount)> GetFailFilesFromXmlReport(
            string pathToXmlReport)
        {
            var doc = new XmlDocument();
            doc.Load(pathToXmlReport);
            return doc.SelectNodes("Report//Issues//Project//Issue")
                .Cast<XmlElement>()
                .Select(x => x.Attributes["File"].Value)
                .GroupBy(fileName => fileName)
                .Select(group => (group.Key, group.Count()))
                .ToList();
        }
    }
}