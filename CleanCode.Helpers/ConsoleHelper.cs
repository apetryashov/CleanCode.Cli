using System;

namespace CleanCode.Helpers
{
    public static class ConsoleHelper
    {
        public static void LogError(string errorMessage)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {errorMessage}");
            Console.ForegroundColor = currentColor;
        }

        public static void LogInfo(string infoMessage)
        {
            Console.WriteLine($"[INFO] {infoMessage}");
        }

        public static void LogWithTag(string tag, string message, bool withNewLine = true)
        {
            ClearCurrentConsoleLine();
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{tag}] ");
            Console.ForegroundColor = currentColor;
            Console.Write(message);

            if (withNewLine)
                Console.WriteLine();
        }

        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}