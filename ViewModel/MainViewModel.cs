using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm;
using Conda_AI.Model;
using Conda_AI.Service;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Conda_AI.ViewModel;

public class MainViewModel : BaseViewModel
{

    private string _username;
    private List<TaskModel> _todos { get; set; }
    private DatabaseService _databaseService;
    public string WelcomeTitle { get; set; }
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(WelcomeTitle));
        }
    }
    public ObservableCollection<TaskModel> ToDos { get; }
    // commands
    public IRelayCommand<TaskModel> GoToTaskPageCommand { get; }
    public IRelayCommand CreateNewTaskCommand { get; }
    public IRelayCommand GoToCalendarPageCommand { get; }
    public IRelayCommand RefreshTasksCommand { get; }

    public MainViewModel(UserService userService, DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Conda Assistant";
        WelcomeTitle = $"Hallo {Username ?? userService.GetUser()?.Name}";
        ToDos = new ObservableCollection<TaskModel>();

        // commands
        GoToTaskPageCommand = new AsyncRelayCommand<TaskModel>(GoToTaskPageAsync);
        CreateNewTaskCommand = new AsyncRelayCommand(CreateNewTaskAsync);
        GoToCalendarPageCommand = new AsyncRelayCommand(GoToCalendarPageAsync);
        RefreshTasksCommand = new AsyncRelayCommand(LoadTasksAsync);    


    }
    
    private async Task GoToTaskPageAsync(TaskModel task)
    {
        if (task == null)
            return;

        await Shell.Current.GoToAsync($"taskmodelpage?taskId={task.TaskModel_Id}");
    }

    private async Task CreateNewTaskAsync()
    {
        TaskModel emptyTaskModel = new TaskModel();
        // GoToTaskPage needs a TaskModel so we create a new one, the id will be 0 by default so the TaskViewModel will 
        // it has to create a new database entry
        await Shell.Current.GoToAsync($"taskmodelpage?taskId={emptyTaskModel.TaskModel_Id}");
    }

    private async Task GoToCalendarPageAsync()
    {
        await Shell.Current.GoToAsync("calendarpage");
    }

    private async Task LoadTasksAsync()
    {
        ToDos.Clear();
        var allTasks = await _databaseService.Tasks.GetAllAsync();
        var allTasksSorted = allTasks.OrderBy(t => t.DueDate).ToList();
        var allTasksLimited = allTasksSorted.Take(6).ToList<TaskModel>();
        foreach (var task in allTasksLimited)
        {
            ToDos.Add(task);
        }
    }
}