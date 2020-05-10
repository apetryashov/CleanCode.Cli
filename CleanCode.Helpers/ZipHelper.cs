using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using CleanCode.Results;

namespace CleanCode.Helpers
{
    public static class ZipHelper
    {
        public static Result<None> DownloadAndExtractZipFile(string downloadUrl, string targetDirectory)
        {
            try
            {
                using var tempDir = new TempDirectory();
                using var client = new WebClient();

                var tempFileName = $"{Guid.NewGuid()}.zip";
                var fileName = Path.Combine(tempDir.PathToTempDirectory, tempFileName);
                client.DownloadFile(downloadUrl, fileName);
                ZipFile.ExtractToDirectory(fileName, targetDirectory, Encoding.Default, true);

                return Result.Ok();
            }
            catch
            {
                return "Something went wrong. Check your internet connection.";
            }
        }
    }
}