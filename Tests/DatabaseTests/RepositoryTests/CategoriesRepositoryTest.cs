using Conda_AI.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.DatabaseTests.RepositoryTests
{
    public class CategoriesRepositoryTest
    {
        private CategoriesRepository _categoriesRepo;
        private TasksRepository _tasksRepo;

        public CategoriesRepositoryTest()
        {
            this._categoriesRepo = new CategoriesRepository(TestsUtils.GetDatabaseConnection());
            this._tasksRepo = new TasksRepository(TestsUtils.GetDatabaseConnection(), this._categoriesRepo);
        }

        [Fact]
        public async Task AssertCategoryIsFetchedWithTasks()
        {
            TestsUtils.CleanUpDatabase(); // Ensure the database is clean before the test

            // Insert a category and some tasks
            var index = await _categoriesRepo.CreateAsync(new Conda_AI.Model.TaskCategory
            {
                Name = "Test Category"
            });
            for (int i = 0; i < 3; i++)
            {
                await _tasksRepo.CreateAsync(new Conda_AI.Model.TaskModel
                {
                    Name = $"Test Task {i + 1}",
                    Category_Id = index
                });
            }

            var category = await _categoriesRepo.GetCategoryWithTasksAsync(1);
            Assert.NotNull(category);
            Assert.Equal("Test Category", category.Name);
            Assert.NotNull(category.Tasks);
            Assert.Equal(3, category.Tasks.Count);
        }
    }
}
