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
        [Option('f', "force",
            Required = false,
            HelpText = "Force update")]
        public bool Force { get; set; }

        public Result<None> Run() => ResharperCltUpdater.UpdateIfNeed(Force);
    }
}