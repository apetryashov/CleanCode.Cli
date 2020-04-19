using System;
using System.IO;

namespace CleanCode.Cli.Common
{
    public class CleanCodeDirectory : IDirectory
    {
        private const string DirectoryName = ".clean-code";

        private static readonly bool OsIsUnix = Environment.OSVersion.Platform == PlatformID.Unix;

        private static string HomeDirectory => OsIsUnix
            ? Environment.GetEnvironmentVariable("HOME")
            : Environment.GetEnvironmentVariable("USERPROFILE");

        public string GetPath() => Path.Combine(HomeDirectory, DirectoryName);
    }
}