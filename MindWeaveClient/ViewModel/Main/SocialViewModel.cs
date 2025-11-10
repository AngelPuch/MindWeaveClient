using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{

    public class FriendDtoDisplay : BaseViewModel
    {
        private const string DEFAULT_AVATAR_PATH = "/Resources/Images/Avatar/default_avatar.png";
        private bool _isOnlineValue;

        /// <summary>
        /// Gets the friend's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the path to the friend's avatar.
        /// </summary>
        public string AvatarPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the friend is online.
        /// </summary>
        public bool IsOnline
        {
            get => _isOnlineValue;
            set { _isOnlineValue = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendDtoDisplay"/> class from a DTO.
        /// </summary>
        public FriendDtoDisplay(FriendDto dto)
        {
            this.Username = dto.username;
            this.AvatarPath = dto.avatarPath ?? DEFAULT_AVATAR_PATH;
            this.IsOnline = dto.isOnline;
        }

        public FriendDtoDisplay() { }
    }
    public class SocialViewModel : BaseViewModel
    {
        // Services
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly ISocialService _socialService;

        // Backing Fields
        private string _searchQueryValue;
        private bool _isFriendsListCheckedValue = true;
        private bool _isAddFriendCheckedValue;
        private bool _isRequestsCheckedValue;
        private bool _isBusyValue;
        private readonly string _currentUserUsername;

        // Public Properties
        public ObservableCollection<FriendDtoDisplay> FriendsList { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<PlayerSearchResultDto> SearchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> ReceivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

        public string SearchQuery
        {
            get => _searchQueryValue;
            set
            {
                _searchQueryValue = value;
                OnPropertyChanged();
                raiseCanExecuteChanged();
            }
        }

        public bool IsFriendsListChecked
        {
            get => _isFriendsListCheckedValue;
            set
            {
                _isFriendsListCheckedValue = value;
                OnPropertyChanged();
                if (value) LoadFriendsListCommand.Execute(null);
            }
        }

        public bool IsAddFriendChecked
        {
            get => _isAddFriendCheckedValue;
            set
            {
                _isAddFriendCheckedValue = value;
                OnPropertyChanged();
                if (value) { SearchResults.Clear(); SearchQuery = ""; }
            }
        }

        public bool IsRequestsChecked
        {
            get => _isRequestsCheckedValue;
            set
            {
                _isRequestsCheckedValue = value;
                OnPropertyChanged();
                if (value) LoadRequestsCommand.Execute(null);
            }
        }

        public bool IsBusy
        {
            get => _isBusyValue;
            private set
            {
                _isBusyValue = value;
                OnPropertyChanged();
                raiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand LoadFriendsListCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackCommand { get; }

        public SocialViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            ISocialService socialService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _socialService = socialService;
            _currentUserUsername = SessionService.Username;

            LoadFriendsListCommand = new RelayCommand(async (param) => await executeLoadFriendsListAsync(), (param) => !IsBusy);
            LoadRequestsCommand = new RelayCommand(async (param) => await executeLoadRequestsAsync(), (param) => !IsBusy);
            SearchCommand = new RelayCommand(async (param) => await executeSearchAsync(), (param) => !IsBusy && !string.IsNullOrWhiteSpace(SearchQuery));
            SendRequestCommand = new RelayCommand(async (param) => await executeSendRequestAsync(param as PlayerSearchResultDto), (param) => !IsBusy && param is PlayerSearchResultDto);
            AcceptRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !IsBusy && param is FriendRequestInfoDto);
            DeclineRequestCommand = new RelayCommand(async (param) => await executeRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !IsBusy && param is FriendRequestInfoDto);
            RemoveFriendCommand = new RelayCommand(async (param) => await executeRemoveFriendAsync(param as FriendDtoDisplay), (param) => !IsBusy && param is FriendDtoDisplay);
            BackCommand = new RelayCommand((param) => _navigationService.goBack());

            connectAndSubscribe();

            if (IsFriendsListChecked) LoadFriendsListCommand.Execute(null);
            else if (IsRequestsChecked) LoadRequestsCommand.Execute(null);
        }

        private async void connectAndSubscribe()
        {
            try
            {
                await _socialService.connectAsync(_currentUserUsername);

                _socialService.FriendRequestReceived += handleFriendRequestReceived;
                _socialService.FriendResponseReceived += handleFriendResponseReceived;
                _socialService.FriendStatusChanged += handleFriendStatusChanged;
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                _navigationService.goBack();
            }
        }

        private async Task executeLoadFriendsListAsync()
        {
            setBusy(true);
            FriendsList.Clear();
            try
            {
                FriendDto[] friends = await _socialService.getFriendsListAsync(_currentUserUsername);
                if (friends != null)
                {
                    foreach (var friendDto in friends)
                    {
                        FriendsList.Add(new FriendDtoDisplay(friendDto));
                    }
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorLoadingFriends, ex); }
            finally { setBusy(false); }
        }

        private async Task executeLoadRequestsAsync()
        {
            setBusy(true);
            ReceivedRequests.Clear();
            try
            {
                FriendRequestInfoDto[] requests = await _socialService.getFriendRequestsAsync(_currentUserUsername);
                if (requests != null)
                {
                    foreach (var req in requests) ReceivedRequests.Add(req);
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorLoadingRequests, ex); }
            finally { setBusy(false); }
        }

        private async Task executeSearchAsync()
        {
            setBusy(true);
            SearchResults.Clear();
            try
            {
                PlayerSearchResultDto[] results = await _socialService.searchPlayersAsync(_currentUserUsername, SearchQuery);
                if (results != null)
                {
                    foreach (var user in results) SearchResults.Add(user);
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorSearchingPlayer, ex); }
            finally { setBusy(false); }
        }

        private async Task executeSendRequestAsync(PlayerSearchResultDto targetUser)
        {
            if (targetUser == null) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await _socialService.sendFriendRequestAsync(_currentUserUsername, targetUser.username);

                if (result.success)
                {
                    _dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    SearchResults.Remove(targetUser);
                }
                else
                {
                    _dialogService.showWarning(result.message, Lang.WarningTitle);
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorSendingRequest, ex); }
            finally { setBusy(false); }
        }

        private async Task executeRespondRequestAsync(FriendRequestInfoDto request, bool accept)
        {
            if (request == null) return;
            setBusy(true);
            try
            {
                OperationResultDto result = await _socialService.respondToFriendRequestAsync(_currentUserUsername, request.requesterUsername, accept);

                if (result.success)
                {
                    _dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    ReceivedRequests.Remove(request);
                    if (accept && IsFriendsListChecked)
                    {
                        await executeLoadFriendsListAsync();
                    }
                }
                else
                {
                    _dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorRespondingRequest, ex); }
            finally { setBusy(false); }
        }

        private async Task executeRemoveFriendAsync(FriendDtoDisplay friendToRemove)
        {
            if (friendToRemove == null) return;

            bool confirmResult = _dialogService.showConfirmation(
                string.Format(Lang.SocialConfirmRemove, friendToRemove.Username),
                Lang.SocialConfirmRemoveTitle);

            if (!confirmResult) return;

            setBusy(true);
            try
            {
                OperationResultDto result = await _socialService.removeFriendAsync(_currentUserUsername, friendToRemove.Username);

                if (result.success)
                {
                    _dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    FriendsList.Remove(friendToRemove);
                }
                else
                {
                    _dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex) { handleError(Lang.ErrorRemovingFriend, ex); }
            finally { setBusy(false); }
        }

        // --- Callback Handlers ---

        private void handleFriendRequestReceived(string fromUsername)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (IsRequestsChecked) { await executeLoadRequestsAsync(); }
                _dialogService.showInfo(string.Format(Lang.SocialInfoNewRequest, fromUsername), Lang.SocialInfoNewRequestTitle);
            });
        }

        private void handleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (accepted)
                {
                    if (IsFriendsListChecked) { await executeLoadFriendsListAsync(); }
                    _dialogService.showInfo(string.Format(Lang.SocialInfoRequestAccepted, fromUsername), Lang.SocialInfoNewFriendTitle);
                }
                else
                {
                    _dialogService.showInfo(string.Format(Lang.SocialInfoRequestDeclined, fromUsername), Lang.SocialInfoRequestDeclinedTitle);
                }
            });
        }

        private void handleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var friend = FriendsList.FirstOrDefault(f => f.Username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    friend.IsOnline = isOnline;
                }
            });
        }

        // --- Helpers ---

        private void setBusy(bool busy)
        {
            IsBusy = busy;
            raiseCanExecuteChanged();
        }

        private void raiseCanExecuteChanged()
        {
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void handleError(string message, Exception ex)
        {
            Trace.TraceError($"Error in {nameof(SocialViewModel)}: {message} | Exception: {ex}");
            _dialogService.showError(message, Lang.ErrorTitle);
        }

        public async void cleanup()
        {
            _socialService.FriendRequestReceived -= handleFriendRequestReceived;
            _socialService.FriendResponseReceived -= handleFriendResponseReceived;
            _socialService.FriendStatusChanged -= handleFriendStatusChanged;

            try
            {
                await _socialService.disconnectAsync(_currentUserUsername);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Failed to disconnect social service cleanly: {ex.Message}");
            }
        }
    }
}