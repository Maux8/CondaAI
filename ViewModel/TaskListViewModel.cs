using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Conda_AI.Model;
using Conda_AI.Model.Enums;
using Conda_AI.Service;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Conda_AI.ViewModel
{
    public partial class TaskListViewModel : ObservableObject
    {
        private DatabaseService _databaseService;

        
        private List<TaskModel> taskModels = new List<TaskModel>();
        public ObservableCollection<TaskModel> Tasks { get; } = [];
        public ObservableCollection<string> Attribute { get; } = ["Name", "Priority", "Status", "DueDate", "Effort"];
       
        
        [ObservableProperty] 
        private TaskCategoryEnum _selectedCategory;
        [ObservableProperty] 
        private string _selectedAttribute;      
        [ObservableProperty] 
        private ObservableCollection<TaskModel> _filteredTasks = new();
        public ObservableCollection<TaskCategoryEnum> Categories { get; } =
            new ObservableCollection<TaskCategoryEnum>(Enum.GetValues(typeof(TaskCategoryEnum)).Cast<TaskCategoryEnum>());


        [ObservableProperty] 
        private bool _isRefreshing;                     // für Refresh Button (späteres Feature)
        [ObservableProperty]
        private bool _isDone;                           //für Checkbox funktion (noch nicht funktional)


        public IRelayCommand RefreshTasksCommand { get; }
        public IRelayCommand<TaskModel> GoToTaskPageCommand { get; }
        public IRelayCommand AddTaskCommand { get; }
        public IAsyncRelayCommand<TaskModel> UpdateTaskStatusCommand { get; }           //für Checkbox funktion (noch nicht funktional)


        public TaskListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Commands
            RefreshTasksCommand = new AsyncRelayCommand(RefreshTasksAsync);
            GoToTaskPageCommand = new AsyncRelayCommand<TaskModel>(GoToTaskPageAsync);
            AddTaskCommand = new AsyncRelayCommand(AddTaskAsync);
            UpdateTaskStatusCommand = new AsyncRelayCommand<TaskModel>(UpdateTaskStatusAsync);          //für Checkbox funktion (noch nicht funktional)

            Tasks.CollectionChanged += (s, e) => UpdateFilteredTasks();

            // Kategorien laden (asynchron)  
            _ = InitializeAsync();
        }


        public async Task InitializeAsync()
        {
            await LoadTasksAsync();

            _selectedCategory = TaskCategoryEnum.Alle;
            _selectedAttribute = Attribute.FirstOrDefault() ?? "Name";
            UpdateFilteredTasks();
        }

        partial void OnSelectedCategoryChanged(TaskCategoryEnum value)
        {
            UpdateFilteredTasks();
        }

        partial void OnSelectedAttributeChanged(string value)
        {
            UpdateFilteredTasks();
        }


        /// <summary>
        /// updates the FilteredTasks collection based on the selected category and attribute
        /// </summary>
        private void UpdateFilteredTasks()
        {
            IEnumerable<TaskModel> filtered = Tasks;

            // Filterung nach Kategorie
            switch (SelectedCategory)
            {
                case TaskCategoryEnum.Erledigt:
                    filtered = filtered.Where(t => t.Status == TaskStatusConda.Done);
                    break;

                case TaskCategoryEnum.Alle:
                    // alle, außer erledigte
                    filtered = filtered.Where(t => t.Status != TaskStatusConda.Done);
                    break;

                default:
                    // nur die ausgewählte Kategorie, außer erledigte
                    filtered = filtered.Where(t =>
                        t.Category == SelectedCategory &&
                        t.Status != TaskStatusConda.Done
                    );
                    break;
            }

            //Sortierung nach Attribut
            if (!string.IsNullOrEmpty(SelectedAttribute))
            {
                filtered = SelectedAttribute switch
                {
                    "Name" => filtered.OrderBy(t => t.Name),
                    "Priority" => filtered.OrderBy(t => t.Priority),
                    "Status" => filtered.OrderBy(t => t.Status),
                    "DueDate" => filtered.OrderBy(t => t.DueDate),
                    "Effort" => filtered.OrderBy(t => t.EstimatedEffort),
                    _ => filtered
                };
            }

            FilteredTasks = new ObservableCollection<TaskModel>(filtered);
            
        }

        /// <summary>
        /// refreshes the tasks and categories from the database
        /// </summary>
        /// <returns></returns>
        private async Task RefreshTasksAsync()
        {
            //Logic for refreshing a task
            IsRefreshing = true;    
            try
            {
                await LoadTasksAsync();
                UpdateFilteredTasks();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// fetches all tasks from the database and updates the local taskModels list
        /// </summary>
        /// <returns></returns>
        private async Task LoadTasksAsync()
        {
            var allTasks = await _databaseService.Tasks.GetAllAsync();
            taskModels = allTasks ?? new List<TaskModel>();  // ?? -> wenn allTasks null ist, dann leere Liste verwenden
            UpdateTasksFromModel();
        }

        /// <summary>
        /// Updates the Tasks collection from the local taskModels list
        /// </summary>
        private void UpdateTasksFromModel()
        {
            Tasks.Clear();
            foreach (var task in taskModels)
            {
                Tasks.Add(task);
            }
        }

        /// <summary>
        /// goes to the TaskModelPage for the selected task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task GoToTaskPageAsync(TaskModel task)
        {
            if (task == null)
                return;

            await Shell.Current.GoToAsync($"taskmodelpage?taskId={task.TaskModel_Id}");
        }

        /// <summary>
        /// adds a new task and navigates to the TaskModelPage for editing
        /// </summary>
        /// <returns></returns>
        private async Task AddTaskAsync()
        {
            TaskModel newTask = new TaskModel(){};
            GoToTaskPageAsync(newTask);
        }

        /// <summary>
        /// for further implementation of checkbox functionality
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task UpdateTaskStatusAsync(TaskModel task)
        {
            await _databaseService.Tasks.UpdateAsync(task);
            //await LoadTasksAsync(); // Reload tasks after updating status
        }
    }
}
