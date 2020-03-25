using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CleanCode.Cli.Common;
using CleanCode.Helpers;
using JetBrains.Annotations;
using LiteDB;

namespace CleanCode.Cli.Commands
{
    //TODO: тут можно прибраться скорее всего
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public static class FilesHashCacheStorage
    {
        private const string CacheCollectionName = "Cache";

        public static IReadOnlyCollection<FileInfo> GetChangedFiles(DirectoryInfo directory)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);

            return FileUtils.GetAllValuableCsFiles(directory)
                .Select(fileInfo => (fileInfo, hash: FileUtils.CalculateFileHash(fileInfo)))
                .Where(x => IsChangedFile(x.fileInfo, x.hash))
                .Select(x => x.fileInfo)
                .ToList();

            bool IsChangedFile(FileSystemInfo fileInfo, string fileHash) => collection.FindOne(x =>
                x.FilePath == fileInfo.FullName &&
                x.Hash == fileHash) == null;
        }

        public static IEnumerable<FileInfo> UpdateFilesHash(IEnumerable<FileInfo> files)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);

            var changedFiles = files.Select(file => new FileWithHash
            {
                FilePath = file.FullName,
                Hash = FileUtils.CalculateFileHash(file)
            }).Where(x =>
            {
                return collection.Include(file =>
                    file.FilePath == x.FilePath &&
                    file.Hash == x.Hash) == null;
            }).ToList();

            collection.Upsert(changedFiles);

            return changedFiles.Select(x => new FileInfo(x.FilePath));
        }

        public static void ClearCache()
        {
            using var db = LiteDbHelper.DataBase;
            db.DropCollection(CacheCollectionName);
        }

        [PublicAPI]
        private class FileWithHash
        {
            [BsonId] public string? FilePath { get; set; }
            public string? Hash { get; set; }
        }
    }
}