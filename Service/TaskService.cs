using Conda_AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Service
{
    internal class TaskService
    {
        public async Task<TaskModel> CreateTaskAsync(string name, DateTime? dueDate, string category, string priority)
        {
            return null;
        }

        public async Task<List<TaskModel>> GetTasksByDateAsync(DateTime date)
        {
            return null;
        }

        public async Task DeleteTaskAsync(int taskId)
        {

        }
    }
}
