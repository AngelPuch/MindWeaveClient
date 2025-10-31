using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class FriendDtoDisplay : BaseViewModel
    {
        private bool isOnlineValue;
        public string username { get; set; }
        public string avatarPath { get; set; }
        public bool isOnline
        {
            get => isOnlineValue;
            set { isOnlineValue = value; OnPropertyChanged(); }
        }

        public FriendDtoDisplay(FriendDto dto)
        {
            this.username = dto.username;
            this.avatarPath = dto.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            this.isOnline = dto.isOnline;
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

        public ObservableCollection<FriendDtoDisplay> friendsList { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<PlayerSearchResultDto> searchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> receivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

        public string searchQuery { get => searchQueryValue; set { searchQueryValue = value; OnPropertyChanged(); } }
        public bool isFriendsListChecked { get => isFriendsListCheckedValue; set { isFriendsListCheckedValue = value; OnPropertyChanged(); if (value) loadFriendsListCommand.Execute(null); } }
        public bool isAddFriendChecked { get => isAddFriendCheckedValue; set { isAddFriendCheckedValue = value; OnPropertyChanged(); if (value) { searchResults.Clear(); searchQuery = ""; } } }
        public bool isRequestsChecked { get => isRequestsCheckedValue; set { isRequestsCheckedValue = value; OnPropertyChanged(); if (value) loadRequestsCommand.Execute(null); } }
        public bool isBusy { get => isBusyValue; set { isBusyValue = value; OnPropertyChanged(); ((RelayCommand)loadFriendsListCommand).RaiseCanExecuteChanged(); ((RelayCommand)loadRequestsCommand).RaiseCanExecuteChanged(); /* etc */ } }

        public ICommand loadFriendsListCommand { get; }
        public ICommand loadRequestsCommand { get; }
        public ICommand searchCommand { get; }
        public ICommand sendRequestCommand { get; }
        public ICommand acceptRequestCommand { get; }
        public ICommand declineRequestCommand { get; }
        public ICommand removeFriendCommand { get; }
        public ICommand backCommand { get; }

        private readonly Action navigateBackAction;
        private string currentUserUsername => SessionService.username;

        public SocialViewModel(Action navigateBack)
        {
            navigateBackAction = navigateBack;

            loadFriendsListCommand = new RelayCommand(async (param) => await executeLoadFriendsListAsync(), (param) => !isBusy);
            loadRequestsCommand = new RelayCommand(async (param) => await executeLoadRequestsAsync(), (param) => !isBusy);
            searchCommand = new RelayCommand(async (param) => await executeSearchAsync(), (param) => !isBusy && !string.IsNullOrWhiteSpace(searchQuery));
            sendRequestCommand = new RelayCommand(async (param) => await executeSendRequestAsync(param as PlayerSearchResultDto), (param) => !isBusy && param is PlayerSearchResultDto);
            acceptRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !isBusy && param is FriendRequestInfoDto);
            declineRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !isBusy && param is FriendRequestInfoDto);
            removeFriendCommand = new RelayCommand(async (param) => await executeRemoveFriendAsync(param as FriendDtoDisplay), (param) => !isBusy && param is FriendDtoDisplay);
            backCommand = new RelayCommand((param) => navigateBackAction?.Invoke());

            connectAndSubscribe();

            if (isFriendsListChecked) loadFriendsListCommand.Execute(null);
            else if (isRequestsChecked) loadRequestsCommand.Execute(null);
        }

        private void connectAndSubscribe()
        {
            if (SocialServiceClientManager.instance.ensureConnected(currentUserUsername))
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
            if (!SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;
            setBusy(true);
            friendsList.Clear();
            try
            {
                SocialManagerService.FriendDto[] friends = await proxy.getFriendsListAsync(currentUserUsername);
                if (friends != null)
                {
                    foreach (var friendDto in friends)
                    {
                        friendsList.Add(new FriendDtoDisplay(friendDto));
                    }
                }
            }
            catch (Exception ex) { handleError("Error loading friends list", ex); }
            finally { setBusy(false); }
        }

        private async Task executeLoadRequestsAsync()
        {
            if (!SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;
            setBusy(true);
            receivedRequests.Clear();
            try
            {
                FriendRequestInfoDto[] requests = await proxy.getFriendRequestsAsync(currentUserUsername);
                if (requests != null)
                {
                    foreach (var req in requests) receivedRequests.Add(req);
                }
            }
            catch (Exception ex) { handleError("Error loading friend requests", ex); }
            finally { setBusy(false); }
        }

        private async Task executeSearchAsync()
        {
            if (!SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;
            setBusy(true);
            searchResults.Clear();
            try
            {
                PlayerSearchResultDto[] results = await proxy.searchPlayersAsync(currentUserUsername, searchQuery);
                if (results != null)
                {
                    foreach (var user in results) searchResults.Add(user);
                }
            }
            catch (Exception ex) { handleError("Error searching players", ex); }
            finally { setBusy(false); }
        }

        private async Task executeSendRequestAsync(PlayerSearchResultDto targetUser)
        {
            if (targetUser == null || !SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.sendFriendRequestAsync(currentUserUsername, targetUser.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    searchResults.Remove(targetUser); 
                }
            }
            catch (Exception ex) { handleError("Error sending friend request", ex); }
            finally { setBusy(false); }
        }

        private async Task executeRespondRequestAsync(FriendRequestInfoDto request, bool accept)
        {
            if (request == null || !SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.respondToFriendRequestAsync(currentUserUsername, request.requesterUsername, accept);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    receivedRequests.Remove(request);
                    if (accept && isFriendsListChecked)
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
            if (friendToRemove == null || !SocialServiceClientManager.instance.ensureConnected(currentUserUsername)) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to remove {friendToRemove.username}?", "Remove Friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult != MessageBoxResult.Yes) return;

            setBusy(true);
            try
            {
                OperationResultDto result = await proxy.removeFriendAsync(currentUserUsername, friendToRemove.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    friendsList.Remove(friendToRemove);
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
                if (isRequestsChecked) { await executeLoadRequestsAsync(); }
            });
        }

        private void handleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (accepted && isFriendsListChecked) { await executeLoadFriendsListAsync(); }
            });
        }

        private void handleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Console.WriteLine($"SocialViewModel: Received status change for {friendUsername} -> Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                var friend = friendsList.FirstOrDefault(f => f.username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    Console.WriteLine($"SocialViewModel: Found friend {friendUsername} in list. Updating IsOnline to {isOnline}.");
                    friend.isOnline = isOnline;
                }
                else { Console.WriteLine($"SocialViewModel: Friend {friendUsername} not found in current FriendsList."); }
            });
        }

        private void setBusy(bool busy) { isBusy = busy; Application.Current.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested()); }
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