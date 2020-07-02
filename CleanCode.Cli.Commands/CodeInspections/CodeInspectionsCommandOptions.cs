using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.CodeInspections
{
    [PublicAPI]
    public class CodeInspectionsCommandOptions
    {
        [Option('s', "solution",
            Required = false,
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string PathToSlnFolder { get; set; } = ".";

        [Option('o', "output",
            Required = false,
            Default = "code-inspections.html",
            HelpText = "Custom output file name")]
        public string OutFileName { get; set; } = "code-inspections.html";

        [Option('i', "interactive",
            Required = false,
            Default = true,
            HelpText = "Open result in default browser")]
        public bool Interactive { get; set; }

        [Usage(ApplicationAlias = "clean-code")]
        public static IEnumerable<Example> Examples => new List<Example>
        {
            new Example("Start code-inspection in current directory", new CodeInspectionsCommand()),
            new Example("Start code-inspection in given directory",
                new CodeInspectionsCommand {PathToSlnFolder = "<path to .sln file>"}),
            new Example("Start code-inspection with custom output file name",
                new CodeInspectionsCommand {OutFileName = "out.html"})
        };
    }
}