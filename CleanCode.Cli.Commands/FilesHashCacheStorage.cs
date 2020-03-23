using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using CleanCode.Helpers;
using Newtonsoft.Json;

namespace CleanCode.Cli.Commands
{
    public static class FilesHashCacheStorage
    {
        private static readonly IDictionary<string, string> FilesHashCache = ReadHashes();

        public static IReadOnlyCollection<string> GetChangedFiles(DirectoryInfo directory)
        {
            return FileUtils.GetAllValuableCsFiles(directory)
                .Select(fileName => (fileName, hash: FileUtils.CalculateFileHash(fileName)))
                .Where(x => IsChangedFile(x.fileName, x.hash))
                .Select(x => x.fileName)
                .ToList();

            static bool IsChangedFile(string fullFileName, string fileHash)
            {
                if (!FilesHashCache.TryGetValue(fullFileName, out var currentHash))
                    return true;

                return currentHash != fileHash;
            }
        }

        private static IDictionary<string, string> ReadHashes()
        {
            if (!File.Exists("filesHash"))
                return new Dictionary<string, string>();

            var changedFile = File.ReadAllText("filesHash");
            var abc = JsonConvert.DeserializeObject<Dictionary<string, string>>(changedFile);

            return abc;
        }

        public static IEnumerable<string> UpdateFilesHash(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var currentHash = FileUtils.CalculateFileHash(file);
                if (FilesHashCache.TryGetValue(file, out var hash) && hash != currentHash)
                    yield return file;

                FilesHashCache[file] = FileUtils.CalculateFileHash(file);
            }

            var serializeObject = JsonConvert.SerializeObject(FilesHashCache);

            File.WriteAllText("filesHash", serializeObject);
        }
    }
}