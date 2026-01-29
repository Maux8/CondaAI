using CommunityToolkit.Maui;
using Conda_AI.Database;
using Conda_AI.Database.Repositories;
using Conda_AI.Service;
using Conda_AI.View;
using Conda_AI.ViewModel;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace Conda_AI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        builder.ConfigureSyncfusionCore();

        // Environment Variables Loading
            Env.TraversePath().Load();
            
            // ViewModels
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<ChatViewModel>();
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddSingleton<TaskListViewModel>();
            builder.Services.AddTransient<TaskViewModel>();
            builder.Services.AddTransient<DiscussionViewModel>();
            // Views
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<CalendarPage>();
            builder.Services.AddTransient<TasksListsPage>();
            builder.Services.AddTransient<TaskModelPage>();
            builder.Services.AddTransient<DiscussionsPage>();
            builder.Services.AddTransient<DiscussionChatPage>();
            // Database 
            builder.Services.AddSingleton<Database.DatabaseConnection>();
            builder.Services.AddSingleton<Database.Repositories.TasksRepository>();
            builder.Services.AddSingleton<Database.Repositories.CategoriesRepository>();
            builder.Services.AddSingleton<Database.Repositories.MessagesRepository>();
            builder.Services.AddSingleton<Database.Repositories.DiscussionsRepository>();
            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<ChatService>();
            builder.Services.AddSingleton<OllamaApiService>();
            builder.Services.AddSingleton<OpenAiService>();
            return builder.Build();
        }
}