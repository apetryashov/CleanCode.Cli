using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class FileUtils
    {
        public static Result<FileInfo> GetPathToSlnFile(string pathToFolderWithSln)
        {
            const string slnSearchPatter = "*.sln";

            var directoryInfo = new DirectoryInfo(pathToFolderWithSln);

            if (!directoryInfo.Exists)
                return $"Folder: {pathToFolderWithSln} not found";

            var slnFiles = directoryInfo
                .GetFiles(slnSearchPatter);

            if (slnFiles.Length != 1)
                return $"Folder: {pathToFolderWithSln} should contains one sln file";

            return slnFiles.First();
        }

        public static IReadOnlyCollection<FileInfo> GetAllValuableCsFiles(DirectoryInfo directoryInfo)
        {
            var tokensToFilterOut = new[]
            {
                $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}",
                $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"
            };

            return directoryInfo
                .GetFiles("*.cs", SearchOption.AllDirectories)
                .Where(x => !tokensToFilterOut.Any(x.FullName.Contains))
                .ToList();
        }

        public static string CalculateFileHash(FileInfo fileInfo)
        {
            using var md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(fileInfo.FullName)));
        }
    }
}