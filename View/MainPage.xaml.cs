using Conda_AI.ViewModel;

namespace Conda_AI.View;

[QueryProperty("Username", "username")]
public partial class MainPage
{
    public string Username
    {
        set => (BindingContext as MainViewModel).Username = value;
    }

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainViewModel vm)
        {
            vm.RefreshTasksCommand.Execute(null);
        }
    }
}