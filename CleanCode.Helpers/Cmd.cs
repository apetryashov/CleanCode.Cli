using System;
using System.Diagnostics;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class Cmd
    {
        public static Result<None> RunProcess(string command, string commandArgs,
            Action<string> handleCmdEventLine = null)
        {
            var startInfo = new ProcessStartInfo(command, commandArgs)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using var process = Process.Start(startInfo);

            if (process == null)
                return $"Failed to start command {command} with commandArgs {commandArgs}'";

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.OutputDataReceived +=
                (sender, args) =>
                {
                    var line = args.Data;
                    if (line != null)
                        handleCmdEventLine?.Invoke(line);
                };

            process.WaitForExit();

            return process.ExitCode == 0
                ? Result.Ok()
                : $"CodeInspections failed with exit code: {process.ExitCode}";
        }
    }
}