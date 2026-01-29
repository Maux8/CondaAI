using Conda_AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Database.Repositories
{
    public class TasksRepository(DatabaseConnection databaseConnection, CategoriesRepository categoryRepository) : AbstractRepository<TaskModel>(databaseConnection)
    {
        /// <summary>
        /// Retrieves a task by its identifier and populates its associated category, if available.
        /// </summary>
        /// <remarks>If the task has a valid category identifier, the method retrieves the category and
        /// assigns it to the task. If the category identifier is invalid or no category exists, the task's category
        /// will remain <see langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the task to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="TaskModel"/> representing the task with its associated category populated,  or <see
        /// langword="null"/> if the task does not exist.</returns>
        public async Task<TaskModel?> GetByIdWithCategoryAsync(int id)
        {
            var task = await this.GetByIdAsync(id);
            
            if (task == null) return null;
            
            var category = task.Category_Id > 0 ? 
                await categoryRepository.GetByIdAsync(task.Category_Id) : null;
            
            //if (category != null) task.Category = category;
            return task;
        }
    }
}
