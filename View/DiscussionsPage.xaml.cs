using Conda_AI.ViewModel;

namespace Conda_AI.View;

public partial class DiscussionsPage : ContentPage
{
	public DiscussionsPage(DiscussionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
