using Conda_AI.Model;
using SQLite;

namespace Conda_AI.Database.Repositories
{
    /// <summary>
    /// Provides a base class for repository implementations that interact with a SQLite database.
    /// </summary>
    /// <remarks>This class is intended to be inherited by specific repository implementations. It provides
    /// common functionality for initializing the database table associated with the entity type <typeparamref
    /// name="T"/>.</remarks>
    /// <typeparam name="T">The type of the entity managed by the repository. Must be a reference type with a parameterless constructor.</typeparam>
    public abstract class AbstractRepository<T>(DatabaseConnection databaseConnection)
        where T : new()
    {
        protected readonly SQLiteAsyncConnection Connection = databaseConnection.Database;

        /// <summary>
        /// Initializes the database table asynchronously for the specified type.
        /// </summary>
        /// <remarks>This method creates a table in the database for the type <typeparamref name="T"/>. It
        /// must be called before performing any operations on the table to ensure it exists.</remarks>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public async Task InitAsync()
        {
            await Connection.CreateTableAsync<T>();
        }

        /// <summary>
        /// Retrieves the table query for the specified type.
        /// </summary>
        /// <remarks>
        /// This method provides an asynchronous queryable interface for the database table associated with
        /// the entity type <typeparamref name="T"/>. It allows querying the table without retrieving all data at once.
        /// </remarks>
        /// <returns>
        /// An <see cref="SQLite.AsyncTableQuery{T}"/> instance representing the queryable table.
        /// </returns>
        public AsyncTableQuery<T> GetTable()
        {
            return Connection.Table<T>();
        }

        /// <summary>
        /// Retrieves all records of type <typeparamref name="T"/> from the database asynchronously.
        /// </summary>
        /// <remarks>This method initializes the database connection if it is not already initialized. It
        /// then queries the database table corresponding to the type <typeparamref name="T"/> and returns all
        /// records.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all records of type
        /// <typeparamref name="T"/>. If no records are found, the returned list will be empty.</returns>
        public async Task<List<T>> GetAllAsync()
        {
            await InitAsync();
            return await Connection.Table<T>().ToListAsync();
        }

        /// <summary>
        /// Asynchronously creates a new record in the database for the specified entity.
        /// </summary>
        /// <remarks>This method initializes the database connection if it is not already
        /// initialized.</remarks>
        /// <param name="entity">The entity to be inserted into the database. Cannot be null.</param>
        /// <returns>The ID of the newly created record, as assigned by the database.</returns>
        public async Task<int> CreateAsync(T entity)
        {
            await InitAsync();
            return await Connection.InsertAsync(entity);
        }

        /// <summary>
        /// Updates the specified entity in the database asynchronously.
        /// </summary>
        /// <remarks>This method initializes the database connection before performing the update. Ensure
        /// that the entity provided matches the schema of the database table.</remarks>
        /// <param name="entity">The entity to update. Cannot be null.</param>
        /// <returns>The number of rows affected by the update operation.</returns>
        public async Task<int> UpdateAsync(T entity)
        {
            await InitAsync();
            return await Connection.UpdateAsync(entity);
        }

        /// <summary>
        /// Deletes the specified entity from the database asynchronously.
        /// </summary>
        /// <remarks>This method initializes the database connection if it is not already
        /// initialized.</remarks>
        /// <param name="entity">The entity to delete. Cannot be <see langword="null"/>.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if no rows were deleted.</returns>
        public async Task<int> DeleteAsync(T entity)
        {
            await InitAsync();
            return await Connection.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes the entity with the specified identifier asynchronously.
        /// </summary>
        /// <remarks>This method initializes the connection if it is not already initialized and retrieves
        /// the entity by its identifier before attempting to delete it. If the entity is not found, no delete operation
        /// is performed.</remarks>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns>The number of rows affected by the delete operation. Returns 0 if the entity does not exist.</returns>
        public async Task<int> DeleteAsync(int id)
        {
            await InitAsync();
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return 0;
            return await Connection.DeleteAsync(entity);
        }

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its unique identifier asynchronously.
        /// </summary>
        /// <remarks>This method initializes the connection asynchronously before attempting to retrieve
        /// the entity. Ensure that the connection is properly configured prior to calling this method.</remarks>
        /// <param name="id">The unique identifier of the entity to retrieve. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type 
        /// <typeparamref name="T"/> if found; otherwise, <see langword="null"/>.</returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            await InitAsync();
            return await Connection.FindAsync<T>(id);
        }
    }
}
