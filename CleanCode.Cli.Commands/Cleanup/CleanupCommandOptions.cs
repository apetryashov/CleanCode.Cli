using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands.Cleanup
{
    [Verb("cleanup", HelpText = "Start ReSharper cleanup tool for given directory")]
    public class CleanupCommandOptions : ICommandOptions<CleanupCommandOptions>
    {
        [Option('s', "solution",
            Required = false,
            Default = ".",
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string? PathToSlnFolder { get; set; }

        public Result<ICommand> GenerateCommand() => new CleanupCommand(this);
    }
}