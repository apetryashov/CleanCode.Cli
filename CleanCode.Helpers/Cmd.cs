using System;
using System.Diagnostics;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class Cmd
    {
        public static Result<None> RunProcess(string command, string args, Action<string> callBack = null)
        {
            var startInfo = new ProcessStartInfo(command, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using var process = Process.Start(startInfo);

            if (process == null)
                return $"Failed to start command {command} with args {args}'";

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            
            process.OutputDataReceived +=
                (sender, args) =>
            {
                var line = args.Data;
                if (line != null)
                    callBack(line);
            };


            process.WaitForExit();

            return process.ExitCode == 0
                ? Result.Ok()
                : $"CodeInspections failed with exit code: {process.ExitCode}";
        }
    }
}