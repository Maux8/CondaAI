using Conda_AI.Database;
using Conda_AI.Database.Repositories;
using Conda_AI.Model;
using SQLite;
using Xunit;

namespace Tests.DatabaseTests.RepositoryTests
{
    public class AbstractRepositoryTest
    {
        private static readonly DatabaseConnection _dbConnection = TestsUtils.GetDatabaseConnection();
        private static readonly ExampleRepository _repo = new ExampleRepository(_dbConnection);

        [Fact]
        public async Task AssertTableIsCreated()
        {
            TestsUtils.CleanUpDatabase();

            List<ExampleModel> res = null;
            await _repo.InitAsync()
                .ContinueWith(async task =>
                {
                    res = await _dbConnection.Database.QueryAsync<ExampleModel>("SELECT * FROM ExampleTable");
                    Assert.NotNull(res);
                });
        }

        [Fact]
        public async Task AssertAllRowsAreFetched()
        {
            TestsUtils.CleanUpDatabase(); // Ensure the database is clean before the test
            await _dbConnection.Database.ExecuteAsync("INSERT INTO ExampleModel (name) VALUES (`TestName`)")
                .ContinueWith(async task =>
                {
                    var res = await _repo.GetAllAsync();
                    Assert.Single(res);
                });
        }

        [Fact]
        public async Task AssertGetByIdReturnsCorrectRow()
        {
            TestsUtils.CleanUpDatabase();

            await _dbConnection.Database.ExecuteAsync("INSERT INTO ExampleModel (name) VALUES ('TestName')")
                .ContinueWith(async task =>
                {
                    var res = await _repo.GetByIdAsync(1);
                    Assert.NotNull(res);
                    Assert.Equal("TestName", res?.Name);
                });
        }

        [Fact]
        public async Task AssertGetByIdReturnsNullForNonExistentRow()
        {
            TestsUtils.CleanUpDatabase();
            var res = await _repo.GetByIdAsync(999); // Assuming 999 does not exist
            Assert.Null(res);
        }

        [Fact]
        public async Task AssertCreateInsertsRow()
        {
            TestsUtils.CleanUpDatabase();
            var newModel = new ExampleModel { Name = "NewTestName" };
            var id = await _repo.CreateAsync(newModel);
            Assert.True(id > 0); // Ensure the ID is valid
            var createdModel = await _repo.GetByIdAsync(id);
            Assert.NotNull(createdModel);
            Assert.Equal("NewTestName", createdModel?.Name);
        }

        [Fact]
        public async Task AssertThrowsErrorUponInsertion()
        {
            TestsUtils.CleanUpDatabase();
            var newModel = new ExampleModel { Name = null }; // This should violate the NotNull constraint
            await Assert.ThrowsAnyAsync<SQLite.NotNullConstraintViolationException>(
                async () => await _repo.CreateAsync(newModel)
            );
        }

        [Fact]
        public async Task AssertThrowsErrorOnDuplicateUniqueProperty()
        {
            TestsUtils.CleanUpDatabase();
            var newModel = new ExampleModel { Name = "UniqueName" };
            var otherModel = new ExampleModel { Name = "UniqueName" };

            await _repo.CreateAsync(newModel); // First insert should succeed
            await Assert.ThrowsAnyAsync<SQLiteException>(
                async () => await _repo.CreateAsync(otherModel) // Second insert should fail due to unique constraint
            );
        }

        //[Fact]
        //public async Task AssertThrowsErrorOnMaxLengthNonRespect()
        //{
        //    TestsUtils.CleanUpDatabase();
        //    var newModel = new ExampleModel { Name = new string('A', 101) }; // Exceeding max length of 100
        //    await Assert.ThrowsAnyAsync<SQLiteException>(
        //        async () => await _repo.CreateAsync(newModel) // This should throw due to max length constraint
        //    );
        //}

        [Fact]
        public async Task AssertUpdateChangesRow()
        {
            TestsUtils.CleanUpDatabase();
            var newModel = new ExampleModel { Name = "InitialName" };
            var id = await _repo.CreateAsync(newModel);
            Assert.True(id > 0); // Ensure the ID is valid
            newModel.Name = "UpdatedName";
            var rowsAffected = await _repo.UpdateAsync(newModel);
            Assert.Equal(1, rowsAffected); // Ensure one row was updated
            var updatedModel = await _repo.GetByIdAsync(id);
            Assert.Equal("UpdatedName", updatedModel?.Name);
        }

        [Fact]
        public async Task AssertDeleteRemovesRow()
        {
            TestsUtils.CleanUpDatabase();
            var newModel = new ExampleModel { Name = "ToBeDeleted" };
            var id = await _repo.CreateAsync(newModel);
            Assert.True(id > 0); // Ensure the ID is valid
            var rowsAffected = await _repo.DeleteAsync(id);
            Assert.Equal(1, rowsAffected); // Ensure one row was deleted
            var deletedModel = await _repo.GetByIdAsync(id);
            Assert.Null(deletedModel); // Ensure the model is no longer in the database
        }
    }

    [Table("ExampleTable")]
    public class ExampleModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull, Unique, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class ExampleRepository : AbstractRepository<ExampleModel>
    {
        public ExampleRepository(DatabaseConnection databaseConnection) : base(databaseConnection)
        { }
    }
}
