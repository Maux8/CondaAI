using Conda_AI.Database;
using System.IO;
using Xunit;

namespace Tests.DatabaseTests
{
    public class DatabaseConnectionTest
    {
        [Fact]
        public void AssertDatabaseIsCreatedUponConnectionTest()
        {
            // Arrange: Ensure the database file does not exist before the test
            TestsUtils.CleanUpDatabase();

            var dbConnection = TestsUtils.GetDatabaseConnection();
            // Act: Get the database connection to trigger the creation of the database file
            var folderPath = Environment.CurrentDirectory; 
            // Get the connection to force the creation of the database file
            var c = dbConnection.Database.GetConnection(); 

            var databasePath = Constants.GetDatabasePath(folderPath);
            // Assert: Check if the database file exists
            Assert.True(File.Exists(databasePath));
        }

        [Fact]
        public void AssertDatabaseIsWritableUponConnectionTest()
        {
            TestsUtils.CleanUpDatabase();

            var c = TestsUtils.GetDatabaseConnection().Database.GetConnection();
            var fileInfo = new FileInfo(TestsUtils.DatabasePath);
            Assert.True((fileInfo.Attributes & FileAttributes.ReadOnly) == 0);
        }
    }
}
