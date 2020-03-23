using System.Collections.Generic;
using System.IO;
using System.Linq;
using CleanCode.Helpers;
using Newtonsoft.Json;

namespace CleanCode.Cli.Commands
{
    public static class FilesHashCacheStorage //TODO: переделать на работу с какой-нибудь базой или еще что
    {
        private static readonly IDictionary<string, string> FilesHashCache = ReadHashes();

        public static IReadOnlyCollection<FileInfo> GetChangedFiles(DirectoryInfo directory)
        {
            return FileUtils.GetAllValuableCsFiles(directory)
                .Select(fileInfo => (fileInfo, hash: FileUtils.CalculateFileHash(fileInfo)))
                .Where(x => IsChangedFile(x.fileInfo, x.hash))
                .Select(x => x.fileInfo)
                .ToList();

            static bool IsChangedFile(FileSystemInfo fileInfo, string fileHash)
            {
                if (!FilesHashCache.TryGetValue(fileInfo.FullName, out var currentHash))
                    return true;

                return currentHash != fileHash;
            }
        }

        private static IDictionary<string, string> ReadHashes()
        {
            if (!File.Exists("filesHash"))
                return new Dictionary<string, string>();

            var changedFile = File.ReadAllText("filesHash");
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(changedFile);
        }

        public static IEnumerable<FileInfo> UpdateFilesHash(IEnumerable<FileInfo> files)
        {
            foreach (var file in files)
            {
                var currentHash = FileUtils.CalculateFileHash(file);
                if (FilesHashCache.TryGetValue(file.FullName, out var hash) && hash != currentHash)
                    yield return file;

                FilesHashCache[file.FullName] = FileUtils.CalculateFileHash(file);
            }

            var serializeObject = JsonConvert.SerializeObject(FilesHashCache);

            File.WriteAllText("filesHash", serializeObject);
        }
    }
}