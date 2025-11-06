using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.Services.Callbacks;

namespace MindWeaveClient.ViewModel.Main
{
    public class FriendDtoDisplay : BaseViewModel
    {
        private bool isOnlineValue;
        public string Username { get; set; }
        public string AvatarPath { get; set; }
        public bool IsOnline
        {
            get => isOnlineValue;
            set { isOnlineValue = value; OnPropertyChanged(); }
        }

        public FriendDtoDisplay(FriendDto dto)
        {
            this.Username = dto.username;
            this.AvatarPath = dto.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            this.IsOnline = dto.isOnline;
        }

        public FriendDtoDisplay() { }
    }

    public class SocialViewModel : BaseViewModel
    {
        private SocialManagerClient proxy => SocialServiceClientManager.instance.proxy;
        private SocialCallbackHandler callbackHandler => SocialServiceClientManager.instance.callbackHandler;

        private string searchQueryValue;
        private bool isFriendsListCheckedValue = true;
        private bool isAddFriendCheckedValue;
        private bool isRequestsCheckedValue;
        private bool isBusyValue;

        public ObservableCollection<FriendDtoDisplay> FriendsList { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<PlayerSearchResultDto> SearchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> ReceivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

        public string SearchQuery { get => searchQueryValue; set { searchQueryValue = value; OnPropertyChanged(); } }
        public bool IsFriendsListChecked { get => isFriendsListCheckedValue; set { isFriendsListCheckedValue = value; OnPropertyChanged(); if (value) LoadFriendsListCommand.Execute(null); } }
        public bool IsAddFriendChecked { get => isAddFriendCheckedValue; set { isAddFriendCheckedValue = value; OnPropertyChanged(); if (value) { SearchResults.Clear(); SearchQuery = ""; } } }
        public bool IsRequestsChecked { get => isRequestsCheckedValue; set { isRequestsCheckedValue = value; OnPropertyChanged(); if (value) LoadRequestsCommand.Execute(null); } }
        public bool IsBusy { get => isBusyValue; set { isBusyValue = value; OnPropertyChanged(); ((RelayCommand)LoadFriendsListCommand).RaiseCanExecuteChanged(); ((RelayCommand)LoadRequestsCommand).RaiseCanExecuteChanged(); /* etc */ } }

        public ICommand LoadFriendsListCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackCommand { get; }

        private readonly Action navigateBackAction;
        private string currentUserUsername => SessionService.Username;

        public SocialViewModel(Action navigateBack)
        {
            navigateBackAction = navigateBack;

            LoadFriendsListCommand = new RelayCommand(async (param) => await executeLoadFriendsListAsync(), (param) => !IsBusy);
            LoadRequestsCommand = new RelayCommand(async (param) => await executeLoadRequestsAsync(), (param) => !IsBusy);
            SearchCommand = new RelayCommand(async (param) => await executeSearchAsync(), (param) => !IsBusy && !string.IsNullOrWhiteSpace(SearchQuery));
            SendRequestCommand = new RelayCommand(async (param) => await executeSendRequestAsync(param as PlayerSearchResultDto), (param) => !IsBusy && param is PlayerSearchResultDto);
            AcceptRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !IsBusy && param is FriendRequestInfoDto);
            DeclineRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !IsBusy && param is FriendRequestInfoDto);
            RemoveFriendCommand = new RelayCommand(async (param) => await executeRemoveFriendAsync(param as FriendDtoDisplay), (param) => !IsBusy && param is FriendDtoDisplay);
            BackCommand = new RelayCommand((param) => navigateBackAction?.Invoke());

            connectAndSubscribe();

            if (IsFriendsListChecked) LoadFriendsListCommand.Execute(null);
            else if (IsRequestsChecked) LoadRequestsCommand.Execute(null);
        }

        private void connectAndSubscribe()
        {
            if (SocialServiceClientManager.instance.EnsureConnected(currentUserUsername))
            {
                if (callbackHandler != null)
                {
                    callbackHandler.FriendRequestReceived -= handleFriendRequestReceived;
                    callbackHandler.FriendResponseReceived -= handleFriendResponseReceived;
                    callbackHandler.FriendStatusChanged -= handleFriendStatusChanged;

                    callbackHandler.FriendRequestReceived += handleFriendRequestReceived;
                    callbackHandler.FriendResponseReceived += handleFriendResponseReceived;
                    callbackHandler.FriendStatusChanged += handleFriendStatusChanged;
                    Console.WriteLine($"SocialViewModel: Subscribed to callback events for {currentUserUsername}.");
                }
                else { /* Error Handling */ }
            }
            else { /* Error Handling */ }
        }

        private async Task executeLoadFriendsListAsync()
        {
            if (!SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;
            setBusy(true);
            FriendsList.Clear();
            try
            {
                SocialManagerService.FriendDto[] friends = await proxy.getFriendsListAsync(currentUserUsername);
                if (friends != null)
                {
                    foreach (var friendDto in friends)
                    {
                        FriendsList.Add(new FriendDtoDisplay(friendDto));
                    }
                }
            }
            catch (Exception ex) { handleError("Error loading friends list", ex); }
            finally { setBusy(false); }
        }

        private async Task executeLoadRequestsAsync()
        {
            if (!SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;
            setBusy(true);
            ReceivedRequests.Clear();
            try
            {
                FriendRequestInfoDto[] requests = await proxy.getFriendRequestsAsync(currentUserUsername);
                if (requests != null)
                {
                    foreach (var req in requests) ReceivedRequests.Add(req);
                }
            }
            catch (Exception ex) { handleError("Error loading friend requests", ex); }
            finally { setBusy(false); }
        }

        private async Task executeSearchAsync()
        {
            if (!SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;
            setBusy(true);
            SearchResults.Clear();
            try
            {
                PlayerSearchResultDto[] results = await proxy.searchPlayersAsync(currentUserUsername, SearchQuery);
                if (results != null)
                {
                    foreach (var user in results) SearchResults.Add(user);
                }
            }
            catch (Exception ex) { handleError("Error searching players", ex); }
            finally { setBusy(false); }
        }

        private async Task executeSendRequestAsync(PlayerSearchResultDto targetUser)
        {
            if (targetUser == null || !SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.sendFriendRequestAsync(currentUserUsername, targetUser.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    SearchResults.Remove(targetUser); 
                }
            }
            catch (Exception ex) { handleError("Error sending friend request", ex); }
            finally { setBusy(false); }
        }

        private async Task executeRespondRequestAsync(FriendRequestInfoDto request, bool accept)
        {
            if (request == null || !SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.respondToFriendRequestAsync(currentUserUsername, request.requesterUsername, accept);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    ReceivedRequests.Remove(request);
                    if (accept && IsFriendsListChecked)
                    {
                        await executeLoadFriendsListAsync();
                    }
                }
            }
            catch (Exception ex) { handleError("Error responding to friend request", ex); }
            finally { setBusy(false); }
        }

        private async Task executeRemoveFriendAsync(FriendDtoDisplay friendToRemove)
        {
            if (friendToRemove == null || !SocialServiceClientManager.instance.EnsureConnected(currentUserUsername)) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to remove {friendToRemove.Username}?", "Remove Friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult != MessageBoxResult.Yes) return;

            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.removeFriendAsync(currentUserUsername, friendToRemove.Username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    FriendsList.Remove(friendToRemove);
                }
            }
            catch (Exception ex) { handleError("Error removing friend", ex); }
            finally { setBusy(false); }
        }

        // --- Handlers ---
        private void handleFriendRequestReceived(string fromUsername)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (IsRequestsChecked) { await executeLoadRequestsAsync(); }
            });
        }

        private void handleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (accepted && IsFriendsListChecked) { await executeLoadFriendsListAsync(); }
            });
        }

        private void handleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Console.WriteLine($"SocialViewModel: Received status change for {friendUsername} -> Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                var friend = FriendsList.FirstOrDefault(f => f.Username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    Console.WriteLine($"SocialViewModel: Found friend {friendUsername} in list. Updating IsOnline to {isOnline}.");
                    friend.IsOnline = isOnline;
                }
                else { Console.WriteLine($"SocialViewModel: Friend {friendUsername} not found in current FriendsList."); }
            });
        }

        private void setBusy(bool busy) { IsBusy = busy; Application.Current.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested()); }
        private void handleError(string message, Exception ex) { Console.WriteLine($"!!! {message}: {ex}"); MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        public void cleanup()
        {
            if (callbackHandler != null)
            {
                callbackHandler.FriendRequestReceived -= handleFriendRequestReceived;
                callbackHandler.FriendResponseReceived -= handleFriendResponseReceived;
                callbackHandler.FriendStatusChanged -= handleFriendStatusChanged;
                Console.WriteLine($"SocialViewModel: Unsubscribed from callback events for {currentUserUsername}.");
            }
        }

    }
}