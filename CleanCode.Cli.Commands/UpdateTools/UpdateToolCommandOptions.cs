using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands.UpdateTools
{
    [Verb("update", HelpText = "Check new resharper-clt version and install if need")]
    public class UpdateToolCommandOptions : ICommandOptions<UpdateToolCommandOptions>
    {
        public Result<ICommand> GenerateCommand() => new UpdateToolCommand();
    }
}