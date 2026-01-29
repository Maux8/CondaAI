using Conda_AI.Model;
using Conda_AI.Service;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Conda_AI.ViewModel
{
    public partial class DiscussionViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly UserService _userService;
        private readonly ChatViewModel _chatViewModel;

        [ObservableProperty]
        private ObservableCollection<Discussion> _discussions = new();

        [ObservableProperty]
        private Discussion _selectedDiscussion;

        public DiscussionViewModel(DatabaseService databaseService, UserService userService, ChatViewModel chatViewModel)
        {
            _databaseService = databaseService;
            _userService = userService;
            _chatViewModel = chatViewModel;

            LoadDiscussionsAsync();
        }

        private async void LoadDiscussionsAsync()
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

        public ICommand CreateNewDiscussionCommand => new Command(async () =>
        {
            var discussion = new Discussion
            {
                Title = "New Chat",
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
                Context = $"Chat started by {_userService.GetUser()?.Name}"
            };

            discussion.DiscussionId = await _databaseService.Discussions.CreateAsync(discussion);
            Discussions.Insert(0, discussion);
            SelectedDiscussion = discussion;

            // Navigate to the chat page with this discussion
            await Shell.Current.GoToAsync($"chatpage?discussionId={discussion.DiscussionId}");
        });

        public ICommand SelectDiscussionCommand => new Command<Discussion>(async (discussion) =>
        {
            if (discussion == null) return;

            SelectedDiscussion = discussion;

            // Get the chat view model and set the discussion directly
            if (_chatViewModel != null)
            {
                _chatViewModel.CurrentDiscussion = discussion;
                // Navigate to the dedicated discussion chat page
                await Shell.Current.GoToAsync($"discussionchatpage?discussionId={discussion.DiscussionId}");
            }
            else
            {
                // Fallback to the query parameter approach
                await Shell.Current.GoToAsync($"discussionchatpage?discussionId={discussion.DiscussionId}");
            }
        });

        public ICommand RenameDiscussionCommand => new Command<Discussion>(async (discussion) =>
        {
            if (discussion == null) return;

            // Prompt user for new name
            string result = await Shell.Current.DisplayPromptAsync(
                "Rename Discussion",
                "Enter a new name for this discussion:",
                initialValue: discussion.Title,
                maxLength: 50,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(result))
            {
                discussion.Title = result;
                await _databaseService.Discussions.UpdateAsync(discussion);

                // Refresh the list
                var index = Discussions.IndexOf(discussion);
                if (index >= 0)
                {
                    Discussions.RemoveAt(index);
                    Discussions.Insert(index, discussion);
                }

                // Update the selected discussion if needed
                if (SelectedDiscussion == discussion)
                {
                    OnPropertyChanged(nameof(SelectedDiscussion));
                }

                // Also update in the ChatViewModel if needed
                if (_chatViewModel?.CurrentDiscussion?.DiscussionId == discussion.DiscussionId)
                {
                    _chatViewModel.CurrentDiscussion.Title = result;

                    // Fix: Use the public method `OnPropertyChanged` from `ChatViewModel` instead of attempting to access the protected member directly.
                    //_chatViewModel.OnPropertyChanged(nameof(ChatViewModel.CurrentDiscussion));
                }
            }
        });

        public ICommand DeleteDiscussionCommand => new Command<Discussion>(async (discussion) =>
        {
            if (discussion == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Delete Discussion", 
                $"Are you sure you want to delete '{discussion.Title}'?", "Yes", "No");

            if (confirm)
            {
                // Delete all messages in the discussion first
                var messages = await _databaseService.Messages.GetByDiscussionIdAsync(discussion.DiscussionId);
                foreach (var message in messages)
                {
                    await _databaseService.Messages.DeleteAsync(message);
                }

                // Then delete the discussion
                await _databaseService.Discussions.DeleteAsync(discussion);
                Discussions.Remove(discussion);

                // If we deleted the selected discussion, select another one
                if (SelectedDiscussion == discussion)
                {
                    SelectedDiscussion = Discussions.Count > 0 ? Discussions[0] : null;
                }
            }
        });
    }
}
