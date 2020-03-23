using CleanCode.Results;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.UpdateTools
{
    [PublicAPI]
    public class UpdateToolCommand : ICommand
    {
        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed();
    }
}