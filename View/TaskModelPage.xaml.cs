using System.Threading.Tasks;
using Conda_AI.Model;
using Conda_AI.ViewModel;
//using HomeKit;

namespace Conda_AI.View;

[QueryProperty(nameof(TaskModel_Id), "taskId")]
public partial class TaskModelPage : ContentPage
{
	public int TaskModel_Id 
	{ 
		get => ViewModel.TaskModel_Id;
		set => ViewModel.TaskModel_Id = value; 
	}

	public TaskViewModel ViewModel => BindingContext as TaskViewModel;
    public TaskModelPage(TaskViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadTaskCommand.Execute(null);
    }
}