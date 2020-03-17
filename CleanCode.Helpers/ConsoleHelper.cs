﻿using System;

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
    }
}