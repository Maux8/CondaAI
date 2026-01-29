using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Conda_AI.Model;
using Conda_AI.ViewModel;
using Conda_AI.Model.Enums;
using Plugin.Maui.Calendar.Models;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Input;
using Conda_AI.View;
using Conda_AI.Service;

public partial class CalendarViewModel : BaseViewModel
{
    private DatabaseService _databaseService;
    private List<TaskModel> taskModels = new List<TaskModel>();
    public ObservableCollection<SchedulerAppointment> CalendarEntries { get; set; }

    public IRelayCommand RefreshCalendarCommand { get; }
    public CalendarViewModel(DatabaseService dbService)
    {
        this.Title = "Kalendar";
        this.CalendarEntries = new ObservableCollection<SchedulerAppointment>();
        this._databaseService = dbService;
        this.RefreshCalendarCommand = new AsyncRelayCommand(RefreshCalendarAsync);

        _ = RefreshCalendarAsync();
    }
    private async Task GetTasks()
    {
        var allTasks = await _databaseService.Tasks.GetAllAsync();
        taskModels = allTasks.ToList();
    }

    private void LoadMockAppointments()
    {
        CalendarEntries.Clear();

        foreach (var task in taskModels)
        {
            CalendarEntries.Add(new SchedulerAppointment
            {
                Subject = task.Name,
                StartTime = task.DueDate,
                EndTime = task.DueDate.AddHours(task.EstimatedEffort > 0 ? task.EstimatedEffort : 1),
                Location = task.Location,
                Notes = $"Task mit Priorität: {task.Priority}",
                Background = task.Priority switch
                {
                    TaskPriority.Hoch => Colors.Red,
                    TaskPriority.Mittel => Colors.Orange,
                    TaskPriority.Gering => Colors.Green,
                    _ => Colors.Gray
                },
                Id = task
            });
        }
    }

    private async Task RefreshCalendarAsync()
    {
        await GetTasks();
        LoadMockAppointments();
    }

    public ICommand OpenTaskPageCommand => new Command<TaskModel>(async (task) =>
    {
        await Shell.Current.GoToAsync(nameof(TaskModelPage), true, new Dictionary<string, object>
        {
            {"TaskModel", task }
        });
    });


}
