using Conda_AI.Database;
using Conda_AI.Model;
using Newtonsoft.Json;

namespace Conda_AI.View;

public partial class OnBoardingPage : ContentPage
{
    public OnBoardingPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        Shell.SetTabBarIsVisible(this, false);
    }

    private async void RegisterButton_OnClicked(object? sender, EventArgs e)
    {
        var userName = NameEntry.Text;

        if (string.IsNullOrWhiteSpace(userName))
        {
            await DisplayAlert("Error", "Please enter a valid name.", "OK");
            return;
        }

        try
        {
            var json = JsonConvert.SerializeObject(new User(userName));
            await File.WriteAllTextAsync(Constants.GetUserFilePath(), json);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save user data: {ex.Message}", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"///{nameof(MainPage)}?username={userName}");
    }
}