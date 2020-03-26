using System.Collections.Generic;
using CleanCode.Cli.Commands.Cleanup;
using CleanCode.Cli.Commands.CodeInspections;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;

namespace CleanCode.Cli
{
    public static class CommandProvider //TODO: Научиться нормально форматировать help
    {
        public static void StartCommand(IEnumerable<string> args)
        {
            Parser.Default
                .ParseArguments<CleanupCommand, CodeInspectionsCommand, UpdateToolCommand>(args)
                .WithParsed<ICommand>(ExecuteCommand);
        }

        private static void ExecuteCommand(ICommand command)
        {
            command.Run()
                .OnFail(ConsoleHelper.LogError);
        }
    }
}