using System;
using System.IO;

namespace CleanCode.Helpers
{
    public class TempDirectory : IDisposable
    {
        public readonly string PathToTempDirectory;

        public TempDirectory()
        {
            PathToTempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(PathToTempDirectory);
        }

        public void Dispose()
        {
            DeleteDirectory(PathToTempDirectory);
        }

        private static void DeleteDirectory(string targetDir)
        {
            if (!Directory.Exists(targetDir))
                return;

            File.SetAttributes(targetDir, FileAttributes.Normal);

            var files = Directory.GetFiles(targetDir);
            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}