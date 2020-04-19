using System.IO;

namespace CleanCode.Cli.Common
{
    public static class DirectoryExtensions
    {
        public static IDirectory WithSubDirectory(this IDirectory directory, string subDirectory)
            => new Directory(Path.Combine(directory.GetPath(), subDirectory));
    }
}