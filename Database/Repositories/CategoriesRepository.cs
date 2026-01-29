using Conda_AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Database.Repositories
{
    public class CategoriesRepository : AbstractRepository<TaskCategory>
    {
        public CategoriesRepository(DatabaseConnection databaseConnection) : base(databaseConnection)
        {
        }

        /// <summary>
        /// Retrieves a task category by its identifier, including its associated tasks.
        /// </summary>
        /// <remarks>This method fetches the task category with the specified <paramref name="id"/> and
        /// populates its  associated tasks. If no category with the given identifier exists, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the task category to retrieve.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains the  <see
        /// cref="TaskCategory"/> with its associated tasks if found; otherwise, <see langword="null"/>.</returns>
        public async Task<TaskCategory?> GetCategoryWithTasksAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                // Load tasks associated with the category
                category.Tasks = await Connection.Table<TaskModel>()
                    .Where(task => task.Category_Id == id)
                    .ToListAsync();
            }
            return category;
        }
    }
}
