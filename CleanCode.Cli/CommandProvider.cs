using System;
using System.Collections.Generic;
using System.Linq;
using CleanCode.Cli.Commands.Cleanup;
using CleanCode.Cli.Commands.CodeInspections;
using CleanCode.Cli.Commands.GenerateDotSettings;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using CommandLine.Text;

namespace CleanCode.Cli
{
    public class CommandProvider
    {
        private static readonly string[] DefaultArgs = {"--help"};

        private readonly Type[] commands =
        {
            typeof(CleanupCommand),
            typeof(CodeInspectionsCommand),
            typeof(GenerateDotSettingsCommand),
            typeof(UpdateToolCommand),
        };

        public void StartCommand(string[] args)
        {
            args = args.Any() ? args : DefaultArgs;

            var parserResult = new Parser(with =>
                {
                    with.IgnoreUnknownArguments = false;
                    with.HelpWriter = null;
                })
                .ParseArguments(args, commands);

            parserResult
                .WithParsed<ICommand>(ExecuteCommand)
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> _)
        {
            var helpText = HelpText.AutoBuild(result, h => new HelpText
                    {
                        AdditionalNewLineAfterOption = false,
                        AddDashesToOption = true,
                        MaximumDisplayWidth = 100,
                        AutoVersion = false
                    }
                    .AddPreOptionsLine("Info:")
                    .AddPostOptionsLine($"{CopyrightInfo.Default}"),
                e => e,
                true);

            Console.WriteLine(helpText.ToString().Trim());
        }

        private static void ExecuteCommand(ICommand command) => command
            .Run()
            .OnFail(ConsoleHelper.LogError);
    }
}