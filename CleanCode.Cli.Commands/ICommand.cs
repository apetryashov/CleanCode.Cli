using CleanCode.Results;

namespace CleanCode.Cli.Commands
{
    public interface ICommand
    {
        Result<None> Run();
    }
}