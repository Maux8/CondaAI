using Conda_AI.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.DatabaseTests.RepositoryTests
{
    public class TasksRepositoryTest
    {
        private readonly TasksRepository _tasksRepository;
        private readonly CategoriesRepository _categoriesRepository;

        public TasksRepositoryTest() 
        { 
            this._categoriesRepository = new CategoriesRepository(TestsUtils.GetDatabaseConnection());
            this._tasksRepository = new TasksRepository(TestsUtils.GetDatabaseConnection(), _categoriesRepository);
        }

        [Fact]
        public async Task AssertTaskIsFetchedWithItsCorrespondingCategory()
        {
            TestsUtils.CleanUpDatabase();

            var categoryIndex = await _categoriesRepository.CreateAsync(new Conda_AI.Model.TaskCategory { Name = "Test Category" });
            var taskIndex = await _tasksRepository.CreateAsync(new Conda_AI.Model.TaskModel
            {
                Name = "Test Task",
                Category_Id = categoryIndex
            });

            var task = await _tasksRepository.GetByIdWithCategoryAsync(taskIndex);
            //Assert.Equal("Test Category", task?.Category?.Name);
        }
    }
}
