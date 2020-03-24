using System;
using System.IO;

namespace CleanCode.Cli
{
    //TODO: лучше уйти от статики
    public class CleanCodeDirectory : ICliDirectory
    {
        private const string DirectoryName = ".clean-code";

        private static readonly bool OsIsUnix = Environment.OSVersion.Platform == PlatformID.Unix;

        public static string GetWithSubDirectory(string subdirectory)
        {
            return Path.Combine(HomeDirectory, DirectoryName, subdirectory);
        }

        private static string HomeDirectory => OsIsUnix
            ? Environment.GetEnvironmentVariable("HOME")
            : Environment.GetEnvironmentVariable("USERPROFILE");

        public string GetDirectory()
        {
            return Path.Combine(HomeDirectory, DirectoryName);
        }
    }
}