using CleanCode.Cli.Common;
using CleanCode.Results;
using CommandLine;
using JetBrains.Annotations;
namespace CleanCode.Cli.Commands.UpdateTools
{
    [PublicAPI]
    [Verb("update", HelpText = "Update resharper-clt tool")]
    public class UpdateToolCommand : ICommand
    {
        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed();
    }
}