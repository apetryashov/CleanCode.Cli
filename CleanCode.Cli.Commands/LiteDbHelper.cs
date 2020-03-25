using CleanCode.Cli.Common;
using LiteDB;

namespace CleanCode.Cli.Commands
{
    public class LiteDbHelper
    {
        private static readonly string ConnectionString = CleanCodeDirectory.GetWithSubDirectory("MyCache.db");

        public static LiteDatabase DataBase => new LiteDatabase(ConnectionString);
    }
}