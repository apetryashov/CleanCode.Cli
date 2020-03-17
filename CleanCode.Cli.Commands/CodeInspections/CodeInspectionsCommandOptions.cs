using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands.CodeInspections
{
    [Verb("code-inspections", HelpText = "Start ReSharper code-inspection tool for given directory")]
    public class CodeInspectionsCommandOptions : ICommandOptions<CodeInspectionsCommandOptions>
    {
        [Option('s', "solution",
            Required = false,
            Default = ".",
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string? PathToSlnFolder { get; set; }

        public Result<ICommand> GenerateCommand() => new CodeInspectionsCommand(this);
    }
}