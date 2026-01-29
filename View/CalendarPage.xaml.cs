using Conda_AI.ViewModel;
using Conda_AI.Model;
using Syncfusion.Maui.Scheduler;
using Conda_AI.Converters;

namespace Conda_AI.View;

public partial class CalendarPage : ContentPage
{
	public CalendarPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CalendarViewModel vm)
        {
            vm.RefreshCalendarCommand.Execute(null);
        }
    }
}