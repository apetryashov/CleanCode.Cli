using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.GenerateDotSettings
{
    [PublicAPI]
    // [Verb("gds", HelpText = "Generate .DotSettings file in sln directory")] //TODO
    public class GenerateDotSettingsCommand : ICommand
    {
        public Result<None> Run()
        {
            ConsoleHelper.LogInfo("Not implemented");

            return Result.Ok();
        }
    }
}