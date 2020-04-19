using CleanCode.Cli.Common;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.GenerateDotSettings
{
    [PublicAPI]
    [Verb("gds", HelpText = "Generate .DotSettings file in sln directory")]
    public class GenerateDotSettingsCommand : ICommand
    {
        public Result<None> Run() => throw new System.NotImplementedException();
    }
}