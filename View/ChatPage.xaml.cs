using Conda_AI.Service;
using Conda_AI.ViewModel;

namespace Conda_AI.View;

[QueryProperty(nameof(DiscussionId), "discussionId")]
public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;
    private readonly DatabaseService _databaseService;

	public ChatPage(ChatViewModel viewModel, DatabaseService databaseService)
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

	private async void LoadDiscussion(int discussionId)
	{
	    var discussionService = _databaseService.Discussions;
	    var discussion = await discussionService.GetByIdAsync(discussionId);
	    if (discussion != null)
	    {
	        _viewModel.CurrentDiscussion = discussion;
	    }
	}
}