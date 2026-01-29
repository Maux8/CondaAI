using Conda_AI.Database;

namespace Tests.DatabaseTests
{
    public static class TestsUtils
    {
        public static string DatabasePath = Constants.GetDatabasePath(Environment.CurrentDirectory);
        static DatabaseConnection _connection;

        public static DatabaseConnection GetDatabaseConnection()
        {
            if (_connection != null)
                return _connection;

            var folderPath = Environment.CurrentDirectory;
            _connection = new DatabaseConnection(folderPath);
            return _connection;
        }

        public static void CleanUpDatabase()
        {
            if (File.Exists(DatabasePath))
            {
                GetDatabaseConnection().Database
                    .CloseAsync()
                    .ContinueWith(task => File.Delete(DatabasePath));
            }
        }
    }
}
