using System.IO;
using System.Linq;
using CleanCode.Cli.Common;
using CleanCode.Results;
using Directory = CleanCode.Cli.Common.Directory;

namespace CleanCode.Cli.Commands.Cleanup
{
    public class DotSettingsFileProvider
    {
        public Result<IDirectory> GetDotSettingsFile(FileInfo slnDirectory)
        {
            var files = slnDirectory.Directory!.GetFiles("*.DotSettings").ToArray();
            if (files.Any())
                return new Directory(files.First().FullName);
            if (files.Length > 1)
                return "Found more then one .DotSettings file in sln directory";

            return Result.Ok(new CleanCodeDirectory().WithSubDirectory("Utils\\Default.DotSettings"));
        }
    }
}