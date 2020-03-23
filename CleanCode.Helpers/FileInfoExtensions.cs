using System.IO;

namespace CleanCode.Helpers
{
    public static class FileInfoExtensions
    {
        public static string GetRelativePath(this FileInfo fileInfo, DirectoryInfo directoryInfo) =>
            Path.GetRelativePath(directoryInfo.FullName, fileInfo.FullName);
    }
}