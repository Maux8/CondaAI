using Conda_AI.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conda_AI.Model;
using System.Windows.Input;
using Conda_AI.Model.Enums;
using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using Conda_AI.AIAPI.Ollama;
using System.Globalization;

namespace Conda_AI.ViewModel;

public partial class ChatViewModel : BaseViewModel
{
    private string _currentMessage;
    public string WelcomeText { get; set; }

    private readonly ChatService _chatService;
    private readonly DatabaseService _databaseService;
    private readonly UserService _userService;

    private Discussion _currentDiscussion;
    public Discussion CurrentDiscussion
    {
        get => _currentDiscussion;
        set
        {
            if (_currentDiscussion != value)
            {
                _currentDiscussion = value;
                OnPropertyChanged(nameof(CurrentDiscussion));
                OnPropertyChanged(nameof(HasCurrentDiscussion));
                LoadDiscussionMessagesAsync();
            }
        }
    }

    public bool HasCurrentDiscussion => CurrentDiscussion != null;

    [ObservableProperty]
    private ObservableCollection<Discussion> _discussions = new();

    public ChatViewModel(UserService userService, ChatService chatService, DatabaseService databaseService)
    {
        WelcomeText = $"Hallo {userService.GetUser()?.Name}";
        _chatService = chatService;
        _databaseService = databaseService;
        _userService = userService;

        LoadDiscussionsAsync();
        
        _chatService.OnMessageReceived += async (_, message) =>
        {
            var aiMessage = new Message
            {
                Content = message,
                Author = MessageAuthor.AI,
                DiscussionId = CurrentDiscussion.DiscussionId
            };
            aiMessage.MessageId = await databaseService.Messages.CreateAsync(aiMessage);
            Messages.Add(aiMessage);
            AddMessageToGroup(aiMessage);
            IsBusy = false;
        };

        _chatService.OnTaskCreated += async (_, task) =>
        {
            IsBusy = true;
            await Task.Delay(1000);
            await Shell.Current.GoToAsync($"taskmodelpage?taskId={task.TaskModel_Id}");
            IsBusy = false;       
        };

        _chatService.OnTaskUpdated += async (_, task) =>
        {
            IsBusy = true;
            await Task.Delay(1000);
            await Shell.Current.GoToAsync($"taskmodelpage?taskId={task.TaskModel_Id}");
            IsBusy = false;
        };
    }
    
    public async void LoadDiscussionsAsync()
    {
        IsBusy = true;
        try
        {
            var savedDiscussions = await _databaseService.Discussions.GetAllAsync();
            Discussions.Clear();

            foreach (var discussion in savedDiscussions)
            {
                Discussions.Add(discussion);
            }

            // Set the current discussion to the most recent one, or create a new one if none exist
            if (Discussions.Count > 0)
            {
                CurrentDiscussion = Discussions.First();
                CurrentDiscussion.IsActive = true;
            }
            else
            {
                await CreateNewDiscussionAsync("New Chat");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading discussions: {ex.Message}");
        }
        finally
        {
            IsBusy = false;       
        }
    }

    private async void LoadDiscussionMessagesAsync()
    {
        if (CurrentDiscussion == null) return;

        IsBusy = true;
        try
        {
            var savedMessages = await _databaseService.Messages.GetByDiscussionIdAsync(CurrentDiscussion.DiscussionId);
            Messages.Clear();
            MessageGroups.Clear();

            foreach (var message in savedMessages)
            {
                Messages.Add(message);
                AddMessageToGroup(message);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading discussion messages: {ex.Message}");
        }
        finally
        {
            IsBusy = false;       
        }
    }

    public async Task<Discussion> CreateNewDiscussionAsync(string title)
    {
        var discussion = new Discussion
        {
            Title = title,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now,
            Context = $"Chat started by {_userService.GetUser()?.Name}"
        };

        // Set all existing discussions to not active
        foreach (var existingDiscussion in Discussions)
        {
            existingDiscussion.IsActive = false;
        }

        discussion.IsActive = true;
        discussion.DiscussionId = await _databaseService.Discussions.CreateAsync(discussion);

        // Add to the collection at the beginning (most recent first)
        Discussions.Insert(0, discussion);
        CurrentDiscussion = discussion;

        return discussion;
    }

    private void AddMessageToGroup(Message message)
    {
        string dateDisplay = FormatMessageDate(message.CreatedAt);
        var group = MessageGroups.FirstOrDefault(g => g.Date == dateDisplay);

        if (group == null)
        {
            group = new MessageGroup 
            { 
                Date = dateDisplay,
                DateObj = message.CreatedAt.Date
            };

            // Find the correct position to insert the new group (newest dates first)
            int insertIndex = 0;
            while (insertIndex < MessageGroups.Count && 
                   MessageGroups[insertIndex].DateObj > message.CreatedAt.Date)
            {
                insertIndex++;
            }

            MessageGroups.Insert(insertIndex, group);
        }

        group.Messages.Add(message);
    }

    private string FormatMessageDate(DateTime dateTime)
    {
        DateTime today = DateTime.Today;
        DateTime yesterday = today.AddDays(-1);

        if (dateTime.Date == today)
            return "Heute";
        else if (dateTime.Date == yesterday)
            return "Gestern";
        else
            return dateTime.ToString("dddd, d. MMMM yyyy", new CultureInfo("de-DE"));
    }


    public string CurrentMessage
    {
        get => _currentMessage;
        set
        {
            if (_currentMessage != value)
            {
                _currentMessage = value;
                OnPropertyChanged(nameof(CurrentMessage));
            }
        }
    }

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(isWelcomeTextNotVisible))]
    private bool _isWelcomeTextVisible = true;

    public bool isWelcomeTextNotVisible => !IsWelcomeTextVisible;

    private ObservableCollection<MessageGroup> _messageGroups = new();
    private ObservableCollection<Message> _messages = new();

    public ObservableCollection<MessageGroup> MessageGroups
    {
        get => _messageGroups;
        set
        {
            _messageGroups = value;
            OnPropertyChanged(nameof(MessageGroups));
        }
    }

    public ObservableCollection<Message> Messages
    {
        get => _messages;
        set
        {
            _messages = value;
            OnPropertyChanged(nameof(Messages));
        }
    }

    public ICommand NavigateToDiscussionsCommand => new Command(async () =>
    {
        await Shell.Current.GoToAsync("discussionspage");
    });

    public ICommand CreateNewDiscussionCommand => new Command(async () =>
    {
        await CreateNewDiscussionAsync("New Chat");
    });

    public ICommand RenameDiscussionCommand => new Command(async () =>
    {
        if (CurrentDiscussion == null) return;

        // Prompt user for new name
        string result = await Shell.Current.DisplayPromptAsync(
            "Rename Discussion", 
            "Enter a new name for this discussion:", 
            initialValue: CurrentDiscussion.Title,
            maxLength: 50,
            keyboard: Keyboard.Text);

        if (!string.IsNullOrWhiteSpace(result))
        {
            CurrentDiscussion.Title = result;
            await _databaseService.Discussions.UpdateAsync(CurrentDiscussion);
            OnPropertyChanged(nameof(CurrentDiscussion));

            // Also update the entry in the Discussions collection
            var discussionInList = Discussions.FirstOrDefault(d => d.DiscussionId == CurrentDiscussion.DiscussionId);
            if (discussionInList != null)
            {
                discussionInList.Title = result;
            }
        }
    });

    public ICommand DeleteDiscussionCommand => new Command(async () =>
    {
        if (CurrentDiscussion == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Delete Discussion", 
            $"Are you sure you want to delete '{CurrentDiscussion.Title}'?", "Yes", "No");

        if (confirm)
        {
            // Store the ID before deleting for comparison
            int deletedId = CurrentDiscussion.DiscussionId;

            // Delete all messages in the discussion first
            var messages = await _databaseService.Messages.GetByDiscussionIdAsync(CurrentDiscussion.DiscussionId);
            foreach (var message in messages)
            {
                await _databaseService.Messages.DeleteAsync(message);
            }

            // Then delete the discussion
            await _databaseService.Discussions.DeleteAsync(CurrentDiscussion);

            // Remove from the list
            var discussionToRemove = Discussions.FirstOrDefault(d => d.DiscussionId == deletedId);
            if (discussionToRemove != null)
            {
                Discussions.Remove(discussionToRemove);
            }

            // Navigate back to discussions list
            await Shell.Current.GoToAsync("discussionspage");

            // If we have other discussions, set the current one to the first available
            if (Discussions.Count > 0)
            {
                CurrentDiscussion = Discussions[0];
            }
            else
            {
                CurrentDiscussion = null;
            }
        }
    });

    public ICommand SendMessageCommand => new Command<string>(async (content) =>
    {
        IsWelcomeTextVisible = false;

        if (string.IsNullOrWhiteSpace(content))
            return;

        // Create a new discussion if needed
        if (CurrentDiscussion == null)
        {
            await CreateNewDiscussionAsync("New Chat");
        }

        // Add user message
        var userMessage = new Message
        {
            Content = content,
            Author = MessageAuthor.User,
            DiscussionId = CurrentDiscussion.DiscussionId
        };

        userMessage.MessageId = await _databaseService.Messages.CreateAsync(userMessage);
        Messages.Add(userMessage);
        AddMessageToGroup(userMessage);

        // Update discussion title if it's the first message
        if (Messages.Count == 1)
        {
            CurrentDiscussion.Title = content.Length > 30 ? content.Substring(0, 30) + "..." : content;
            await _databaseService.Discussions.UpdateAsync(CurrentDiscussion);
        }

        // Clear the input field
        CurrentMessage = string.Empty;
        await ProcessAiResponse();
    });

    private async Task ProcessAiResponse()
    {
        IsBusy = true;

        await _chatService.GetAiResponse(
            Messages.Select(m => new OllamaApiMessage
            {
                Role = m.Author == MessageAuthor.User ? "user" : "assistant",
                Content = m.Content
            }).ToList(),
            CurrentDiscussion?.Context
        );
    }
}