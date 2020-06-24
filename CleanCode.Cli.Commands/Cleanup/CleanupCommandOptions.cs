using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using JetBrains.Annotations;

namespace CleanCode.Cli.Commands.Cleanup
{
    [PublicAPI]
    public class CleanupCommandOptions
    {
        [Option('s', "solution",
            Required = false,
            HelpText = "Custom path to .sln file. Current directory by default ")]
        public string PathToSlnFolder { get; set; }

        [Option('f', "force",
            Required = false,
            HelpText = "State force cleanup. It is slow but will check all files again")]
        public bool Force { get; set; }

        #region Examples

        [Usage(ApplicationAlias = "clean-code")]
        public static IEnumerable<Example> Examples => new List<Example>
        {
            new Example("Start cleanup in current directory", new CleanupCommand()),
            new Example("Start cleanup in given directory",
                new CleanupCommandOptions {PathToSlnFolder = "<path to .sln file>"}),
            new Example("Start cleanup without cache", new CleanupCommandOptions {Force = true})
        };

        #endregion
    }
}