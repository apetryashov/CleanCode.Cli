using CleanCode.Cli.Common;
using LiteDB;

namespace CleanCode.Cli.Commands
{
    public class LiteDbHelper
    {
        private static readonly string ConnectionString = new CleanCodeDirectory()
            .WithSubDirectory("MyCache.db")
            .GetPath();

        public static LiteDatabase DataBase => new LiteDatabase(ConnectionString);
    }
}