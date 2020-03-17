using CleanCode.Cli.Commands;

namespace CleanCode.Tool
{
    internal static class Program
    {
        private static void Main(string[] args) => CommandProvider.StartCommand(args);
    }
}