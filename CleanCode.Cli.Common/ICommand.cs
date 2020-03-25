using CleanCode.Results;

namespace CleanCode.Cli.Common
{
    public interface ICommand
    {
        Result<None> Run();
    }
}