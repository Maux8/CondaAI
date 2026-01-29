using Conda_AI.Service;
using Conda_AI.ViewModel;

namespace Conda_AI.View;

[QueryProperty(nameof(DiscussionId), "discussionId")]
public partial class DiscussionChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;
    private readonly DatabaseService _databaseService;

    public DiscussionChatPage(ChatViewModel viewModel, DatabaseService databaseService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _databaseService = databaseService;
        BindingContext = viewModel;
    }

    public string DiscussionId
    {
        set
        {
            if (int.TryParse(value, out int id))
            {
                LoadDiscussion(id);
            }
        }
    }

    /// <summary>
    /// Loads a discussion from the database using the specified discussion ID and updates
    /// the view model and page title with the loaded discussion data.
    /// </summary>
    /// <param name="discussionId">
    /// The unique identifier of the discussion to be retrieved from the database.
    /// </param>
    private async void LoadDiscussion(int discussionId)
    {
        var discussionService = _databaseService.Discussions;
        var discussion = await discussionService.GetByIdAsync(discussionId);
        if (discussion != null)
        {
            _viewModel.CurrentDiscussion = discussion;
            Title = discussion.Title;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ensure we have a valid discussion
        if (_viewModel.CurrentDiscussion == null)
        {
            // Try to load the first available discussion or create one
            _viewModel.LoadDiscussionsAsync();
        }
    }
}
