using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CleanCode.Helpers;
using JetBrains.Annotations;
using LiteDB;

namespace CleanCode.Cli.Commands
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public static class FilesHashCacheStorage
    {
        private const string CacheCollectionName = "Cache";

        public static IReadOnlyCollection<FileInfo> GetNewAndUpdatedFiles(DirectoryInfo directory)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);

            var files = FileUtils.GetAllValuableCsFiles(directory);

            return files.Where(IsNewOrChangedFile).ToList();

            bool IsNewOrChangedFile(FileInfo fileInfo)
            {
                var cacheFile = collection.FindById(fileInfo.FullName);

                return cacheFile == null || cacheFile.Hash != fileInfo.CalculateMd5Hash();
            }
        }

        public static void UpdateFilesHash(IEnumerable<FileInfo> files)
        {
            using var db = LiteDbHelper.DataBase;
            var collection = db.GetCollection<FileWithHash>(CacheCollectionName);

            var newOrChangedFiles = files.Select(file => new FileWithHash
            {
                FilePath = file.FullName,
                Hash = file.CalculateMd5Hash()
            });

            collection.Upsert(newOrChangedFiles);
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