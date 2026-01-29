using Conda_AI.Database;
using Conda_AI.Database.Repositories;
using SQLite;

namespace Conda_AI.Service;

public class DatabaseService(
    CategoriesRepository categories,
    MessagesRepository messages,
    TasksRepository tasks,
    DiscussionsRepository discussions,
    DatabaseConnection connection
)
{
    public readonly CategoriesRepository Categories = categories;
    public readonly MessagesRepository Messages = messages;
    public readonly TasksRepository Tasks = tasks;
    public readonly DiscussionsRepository Discussions = discussions;
    public readonly SQLiteAsyncConnection Connection = connection.Database;
}