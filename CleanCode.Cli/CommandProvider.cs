using System;
using System.Collections.Generic;
using System.Linq;
using CleanCode.Cli.Commands.Cleanup;
using CleanCode.Cli.Commands.CodeInspections;
using CleanCode.Cli.Commands.UpdateTools;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using CleanCode.Results;
using CommandLine;
using CommandLine.Text;

namespace CleanCode.Cli
{
    public static class CommandProvider //TODO: Научиться нормально форматировать help
    {
        private static readonly string[] DefaultArgs = {"--help"};

        public static void StartCommand(string[] args)
        {
            args = args.Any() ? args : DefaultArgs;

            var parserResult = new Parser(with => with.HelpWriter = null)
                .ParseArguments<CleanupCommand, CodeInspectionsCommand, UpdateToolCommand>(args);

            parserResult
                .WithParsed<ICommand>(ExecuteCommand)
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> _)
        {
            var helpText = HelpText.AutoBuild(result, h => new HelpText
                    {
                        AdditionalNewLineAfterOption = false,
                        AddDashesToOption = true,
                        MaximumDisplayWidth = 100,
                        AutoVersion = false
                    }.AddPreOptionsLine("Info:")
                    .AddPostOptionsLine($"{CopyrightInfo.Default}"),
                e => e,
                true);
            Console.WriteLine(helpText.ToString().Trim());
        }

        private static void ExecuteCommand(ICommand command)
        {
            command.Run()
                .OnFail(ConsoleHelper.LogError);
        }
    }

    class Options
    {
        [Option('r', "read", Required = false, HelpText = "Input files to be processed.")]
        public IEnumerable<string> InputFiles { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option(
            Default = false,
            HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option("stdin",
            Default = false,
            HelpText = "Read from stdin")]
        public bool stdin { get; set; }

        [Value(0, MetaName = "offset", HelpText = "File offset.")]
        public long? Offset { get; set; }
    }
}