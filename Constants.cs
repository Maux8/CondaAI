using System.IO.Enumeration;
using SQLite;

namespace Conda_AI.Database
{

    /// <summary>
    /// Provides constant values and settings used for configuration and access.
    /// </summary>
    /// <remarks>This class contains constants for the database filename, access flags, and the computed
    /// database path. It is intended to centralize configuration values for consistent usage across
    /// the application.</remarks>
    public static class Constants
    {
        public const string DatabaseFilename = "CondaAIDatabase.db3";
        
        public const string UserFileName = "CondaAIUser.json";

        public const SQLiteOpenFlags DatabaseFlags =
        // open the database in read/write mode
        SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLiteOpenFlags.SharedCache;

        /// <summary>
        /// Constructs the full file path to the database file.
        /// </summary>
        /// <param name="folderPath">An optional folder path where the database file is located. If not specified, the application's data
        /// directory is used.</param>
        /// <returns>The full file path to the database file, combining the specified or default folder path with the database
        /// file name.</returns>
        public static string GetDatabasePath(string folderPath) => Path.Combine(folderPath, DatabaseFilename);
        
        public static string GetUserFilePath() => Path.Combine(FileSystem.AppDataDirectory, UserFileName);
    }
}
