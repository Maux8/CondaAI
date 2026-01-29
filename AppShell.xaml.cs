using Conda_AI.Database;
using Conda_AI.View;

namespace Conda_AI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("taskmodelpage", typeof(TaskModelPage));
        Routing.RegisterRoute("discussionspage", typeof(DiscussionsPage));
        Routing.RegisterRoute("chatpage", typeof(ChatPage));
        Routing.RegisterRoute("discussionchatpage", typeof(DiscussionChatPage));

        // Register nested routes for better navigation
        Routing.RegisterRoute("chat/chatpage", typeof(ChatPage));
        Routing.RegisterRoute("chat/discussionchatpage", typeof(DiscussionChatPage));
        Routing.RegisterRoute("calendarpage", typeof(CalendarPage));
        Routing.RegisterRoute(nameof(OnBoardingPage), typeof(OnBoardingPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }

    protected override void OnAppearing()
    {
        // Call SetupRouting after initialization
        _ = SetupRouting();
    }

    private static async Task SetupRouting()
    {
        if (!File.Exists(Constants.GetUserFilePath())) await Current.GoToAsync(nameof(OnBoardingPage), animate: false);
    }
}