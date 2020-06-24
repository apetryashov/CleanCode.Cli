using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CleanCode.Helpers
{
    public class FilesCheckingProgressBar
    {
        private static readonly Regex ExtractCsFile = new Regex("(?<=)(\\w*\\.cs)$", RegexOptions.Compiled);
        private readonly int totalFiles;
        private readonly HashSet<string> uncheckedFiles;

        public FilesCheckingProgressBar(IEnumerable<FileInfo> validatedFiles)
        {
            uncheckedFiles = validatedFiles.Select(info => info.Name).ToHashSet();
            totalFiles = uncheckedFiles.Count;
        }

        public void RegisterFile(string line)
        {
            var match = ExtractCsFile.Match(line);
            if (!match.Success)
                return;
            var currentFile = match.Value;
            if (!uncheckedFiles.Contains(currentFile))
                return;
            uncheckedFiles.Remove(currentFile);
            ConsoleHelper.LazyLog($"{totalFiles - uncheckedFiles.Count}\\{totalFiles}", currentFile);
        }
    }
}