using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Conda_AI.ViewModel.Messages;
using Conda_AI.Model;
using Conda_AI.Model.Enums;
using Conda_AI.Service;
using Conda_AI.View;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Conda_AI.ViewModel
{
    public partial class TaskViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;
        private int taskId;

        [ObservableProperty]
        private TaskModel currentTaskModel;
        [ObservableProperty]
        private bool isDone;

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand DeleteCommand { get; }
        public IRelayCommand LoadTaskCommand { get; }

        public TaskViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            SaveCommand = new AsyncRelayCommand(SaveTaskAsync);
            LoadTaskCommand = new AsyncRelayCommand(LoadTaskAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteTaskAsync);

            currentTaskModel = new TaskModel()
            {
                DueDate = DateTime.Today,
                Category = TaskCategoryEnum.ToDo
            }; 
        }

        public int TaskModel_Id
        {
            get => taskId;
            set
            {
                if (taskId != value)
                {
                    taskId = value;
                    _ = LoadTaskAsync(); // Async Task laden, wenn ID gesetzt wird
                }
            }
        }

        public string Name
        {
            get => CurrentTaskModel.Name;
            set
            {
                if (CurrentTaskModel.Name != value)
                {
                    CurrentTaskModel.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => CurrentTaskModel.Description;
            set
            {
                if (CurrentTaskModel.Description != value)
                {
                    CurrentTaskModel.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
      
        public TaskStatusConda Status
        {
            get => CurrentTaskModel.Status;
            set
            {
                if (CurrentTaskModel.Status != value)
                {
                    CurrentTaskModel.Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public TaskPriority Priority
        {
            get => CurrentTaskModel.Priority;
            set
            {
                if (CurrentTaskModel.Priority != value)
                {
                    CurrentTaskModel.Priority = value;
                    OnPropertyChanged(nameof(Priority));
                }
            }
        }

        public TaskCategoryEnum CategoryEnum
        {
            get => CurrentTaskModel.Category;
            set
            {
                if (CurrentTaskModel.Category != value)
                {
                    CurrentTaskModel.Category = value;
                    OnPropertyChanged(nameof(CategoryEnum));
                }
            }
        }

        public DateTime DueDate
        {
            get => CurrentTaskModel.DueDate;
            set
            {
                if (CurrentTaskModel.DueDate != value)
                {
                    CurrentTaskModel.DueDate = value;
                    OnPropertyChanged(nameof(DueDate));
                }
            }
        }

        public string Location
        {
            get => CurrentTaskModel.Location;
            set
            {
                if (CurrentTaskModel.Location != value)
                {
                    CurrentTaskModel.Location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public int EstimatedEffort
        {
            get => CurrentTaskModel.EstimatedEffort;
            set
            {
                if (CurrentTaskModel.EstimatedEffort != value)
                {
                    CurrentTaskModel.EstimatedEffort = value;
                    OnPropertyChanged(nameof(EstimatedEffort));
                }
            }
        }

        partial void OnCurrentTaskModelChanged(TaskModel value)
        {
            if (value != null)
                IsDone = value.Status == TaskStatusConda.Done;
        }

        partial void OnIsDoneChanged(bool value)                           //für Checkbox funktion (noch nicht funktional)
        {
            // Enum im Model setzen
            CurrentTaskModel.Status = value
                ? TaskStatusConda.Done
                : TaskStatusConda.ToDo;

            OnPropertyChanged(nameof(Status));

            _ = SaveTaskAsync();

            // Nachricht an TaskListViewModel, damit neu gefiltert wird
            WeakReferenceMessenger.Default
                 .Send(new TaskStatusChangedMessage(CurrentTaskModel.TaskModel_Id));
        }



        private async Task LoadTaskAsync()
        {
            var task = await _databaseService.Tasks.GetByIdAsync(taskId);
            if (task != null)
                CurrentTaskModel = task;
        }

        private async Task SaveTaskAsync()
        {
            if (CurrentTaskModel == null)
                return;

            if (CurrentTaskModel.TaskModel_Id > 0)
            {
                await _databaseService.Tasks.UpdateAsync(CurrentTaskModel);
            } else
            {
                await _databaseService.Tasks.CreateAsync(CurrentTaskModel);
            }

            await Shell.Current.DisplayAlert("Gespeichert", "Änderungen wurden gespeichert.", "OK");
            await Shell.Current.GoToAsync(".."); 
        }

        private async Task DeleteTaskAsync()
        {
            if (CurrentTaskModel == null)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert("Löschen", "Möchten Sie diese Aufgabe wirklich löschen?", "Ja", "Nein");

            if (confirm)
            {
                await _databaseService.Tasks.DeleteAsync(CurrentTaskModel);
            }

            await Shell.Current.DisplayAlert("Gelöscht", "Aufgabe wurde gelöscht.", "OK");
            await Shell.Current.GoToAsync(".."); 
        }


    }
}
