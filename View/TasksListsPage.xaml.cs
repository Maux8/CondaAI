namespace Conda_AI.View;

using CommunityToolkit.Maui.Markup;
using Conda_AI.ViewModel;

public partial class TasksListsPage : ContentPage
{
	public TasksListsPage(TaskListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TaskListViewModel vm)
        {
            vm.RefreshTasksCommand.Execute(null);
        }
    }
}

