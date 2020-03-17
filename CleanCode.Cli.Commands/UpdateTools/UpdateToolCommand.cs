using CleanCode.Results;

namespace CleanCode.Cli.Commands.UpdateTools
{
    public class UpdateToolCommand : ICommand
    {
        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed();
    }
}