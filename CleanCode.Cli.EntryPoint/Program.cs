using CleanCode.Cli.Commands;

namespace CleanCode.Tool
{
    //TODO: Затащить это все в chocolatey
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CommandProvider.StartCommand(args);
        }
    }    
}