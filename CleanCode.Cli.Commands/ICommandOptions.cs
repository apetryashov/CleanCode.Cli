using CleanCode.Results;

namespace CleanCode.Cli.Commands
{
    public interface ICommandOptions<in TOpt>
        where TOpt : ICommandOptions<TOpt>
    {
        Result<ICommand> GenerateCommand();
    }
}