using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
            return GetChangedFiles(FileUtils.GetAllValuableCsFiles(directory))
                .Select(x => new FileInfo(x.FilePath))
                .ToList();
        }

        public static IEnumerable<FileInfo> UpdateFilesHash(IEnumerable<FileInfo> files)
        {
            var changedFiles = GetChangedFiles(files);

            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);
            collection.Upsert(changedFiles);

            return changedFiles.Select(x => new FileInfo(x.FilePath));
        }

        private static IReadOnlyCollection<FileWithHash> GetChangedFiles(IEnumerable<FileInfo> files)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);

            return files.Select(file => new FileWithHash
            {
                FilePath = file.FullName,
                Hash = FileUtils.CalculateFileHash(file)
            }).Where(x =>
            {
                return collection.FindOne(file =>
                    file.FilePath == x.FilePath &&
                    file.Hash == x.Hash) == null;
            }).ToList();
        }

        public static void ClearCache()
        {
            using var db = LiteDbHelper.DataBase;
            db.DropCollection(CacheCollectionName);
        }

        [PublicAPI]
        private class FileWithHash
        {
#nullable disable //because initialized from db
            [BsonId] public string FilePath { get; set; }
            public string Hash { get; set; }
        }
    }
}