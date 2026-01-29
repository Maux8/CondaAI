using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Database
{

    /// <summary>
    /// Represents a connection to a SQLite database, providing access to asynchronous database operations.
    /// </summary>
    /// <remarks>This class initializes a connection to the SQLite database using the configuration specified in  <see
    /// cref="Constants"/>. The connection is established lazily during the construction of the object.</remarks>
    public class DatabaseConnection
    {
        public SQLiteAsyncConnection Database { get; private set; }

        public DatabaseConnection(string? path = null)
        {
            if (Database is not null)
                return;

            // Use platform-specific code to handle FileSystem.AppDataDirectory
            path ??= FileSystem.AppDataDirectory;
            Database = new SQLiteAsyncConnection(Constants.GetDatabasePath(path), Constants.DatabaseFlags);
        }
    }
}
