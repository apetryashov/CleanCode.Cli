namespace CleanCode.Cli.Common
{
    public class Directory : IDirectory
    {
        private readonly string path;

        public Directory(string path) => this.path = path;

        public string GetPath() => path;
    }
}