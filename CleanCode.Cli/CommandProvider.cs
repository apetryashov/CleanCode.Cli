using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static string CurrentVersion => Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
        private static readonly string[] DefaultArgs = {"--help"};

        private readonly Type[] commands =
        {
            typeof(CleanupCommand),
            typeof(CodeInspectionsCommand),
            typeof(GenerateDotSettingsCommand),
            typeof(UpdateToolCommand),
        };

        public Result<None> StartCommand(string[] args)
        {
            args = args.Any() ? args : DefaultArgs;

            var parserResult = new Parser(with =>
                {
                    with.IgnoreUnknownArguments = false;
                    with.HelpWriter = null;
                })
                .ParseArguments(args, commands);

            return parserResult
                .MapResult<ICommand, Result<None>>(
                    command => command.Run(),
                    errs => DisplayHelp(parserResult, errs));
        }

        private static Result<None> DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> _)
        {
            var helpText = HelpText.AutoBuild(result, h => new HelpText
                    {
                        AdditionalNewLineAfterOption = false,
                        AddDashesToOption = true,
                        MaximumDisplayWidth = 100,
                        AutoVersion = false
                    }
                    .AddPreOptionsLine("Info:")
                    .AddPostOptionsLine($"{CopyrightInfo.Default} v{CurrentVersion}"),
                e => e,
                true);

            Console.WriteLine(helpText.ToString().Trim());
            return Result.Ok();
        }
    }
}