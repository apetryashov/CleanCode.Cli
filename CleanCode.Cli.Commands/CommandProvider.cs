using System.Collections.Generic;
using CleanCode.Cli.Commands.Cleanup;
using CleanCode.Cli.Commands.CodeInspections;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli.Commands
{
    public static class CommandProvider
    {
        public static void StartCommand(IEnumerable<string> args)
        {
            Parser.Default
                .ParseArguments<CleanupCommandOptions, CodeInspectionsCommandOptions, UpdateToolCommandOptions>(args)
                .WithParsed<CleanupCommandOptions>(ExecuteCommand)
                .WithParsed<CodeInspectionsCommandOptions>(ExecuteCommand)
                .WithParsed<UpdateToolCommandOptions>(ExecuteCommand);
        }

        private static void ExecuteCommand<TOpt>(ICommandOptions<TOpt> commandOptions)
            where TOpt : ICommandOptions<TOpt>
        {
            commandOptions
                .GenerateCommand()
                .Then(command => command.Run())
                .OnFail(ConsoleHelper.LogError);
        }
    }
}