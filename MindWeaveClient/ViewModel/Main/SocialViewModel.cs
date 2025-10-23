// MindWeaveClient/ViewModel/Main/SocialViewModel.cs
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService; // Necesario para FriendDto
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    // *** ESTA CLASE SE REUTILIZARÁ EN LOBBYVIEWMODEL ***
    // Asegúrate de que sea 'public'
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
            this.avatarPath = dto.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png"; // Default si es null
            this.isOnline = dto.isOnline;
        }

        // Constructor vacío o alternativo si es necesario
        public FriendDtoDisplay() { }
    }

    // El resto de SocialViewModel.cs sigue igual...
    public class SocialViewModel : BaseViewModel
    {
        // ... (código existente de SocialViewModel sin cambios) ...
        // Managers y Proxies
        private SocialManagerClient _proxy => SocialServiceClientManager.Instance.Proxy;
        private SocialCallbackHandler _callbackHandler => SocialServiceClientManager.Instance.CallbackHandler;

        // Propiedades UI
        private string _searchQuery;
        private bool _isFriendsListChecked = true;
        private bool _isAddFriendChecked;
        private bool _isRequestsChecked;
        private bool _isBusy;

        public ObservableCollection<FriendDtoDisplay> FriendsList { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<PlayerSearchResultDto> SearchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> ReceivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

        public string SearchQuery { get => _searchQuery; set { _searchQuery = value; OnPropertyChanged(); } }
        public bool IsFriendsListChecked { get => _isFriendsListChecked; set { _isFriendsListChecked = value; OnPropertyChanged(); if (value) LoadFriendsListCommand.Execute(null); } }
        public bool IsAddFriendChecked { get => _isAddFriendChecked; set { _isAddFriendChecked = value; OnPropertyChanged(); if (value) { SearchResults.Clear(); SearchQuery = ""; } } }
        public bool IsRequestsChecked { get => _isRequestsChecked; set { _isRequestsChecked = value; OnPropertyChanged(); if (value) LoadRequestsCommand.Execute(null); } }
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); ((RelayCommand)LoadFriendsListCommand).RaiseCanExecuteChanged(); ((RelayCommand)LoadRequestsCommand).RaiseCanExecuteChanged(); /* etc */ } }

        // Comandos
        public ICommand LoadFriendsListCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackCommand { get; }

        private readonly Action _navigateBackAction;
        private string CurrentUserUsername => SessionService.username;

        public SocialViewModel(Action navigateBack)
        {
            _navigateBackAction = navigateBack;

            LoadFriendsListCommand = new RelayCommand(async (param) => await ExecuteLoadFriendsListAsync(), (param) => !IsBusy);
            LoadRequestsCommand = new RelayCommand(async (param) => await ExecuteLoadRequestsAsync(), (param) => !IsBusy);
            SearchCommand = new RelayCommand(async (param) => await ExecuteSearchAsync(), (param) => !IsBusy && !string.IsNullOrWhiteSpace(SearchQuery));
            SendRequestCommand = new RelayCommand(async (param) => await ExecuteSendRequestAsync(param as PlayerSearchResultDto), (param) => !IsBusy && param is PlayerSearchResultDto);
            AcceptRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !IsBusy && param is FriendRequestInfoDto);
            DeclineRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !IsBusy && param is FriendRequestInfoDto);
            RemoveFriendCommand = new RelayCommand(async (param) => await ExecuteRemoveFriendAsync(param as FriendDtoDisplay), (param) => !IsBusy && param is FriendDtoDisplay);
            BackCommand = new RelayCommand((param) => _navigateBackAction?.Invoke());

            ConnectAndSubscribe();

            if (IsFriendsListChecked) LoadFriendsListCommand.Execute(null);
            else if (IsRequestsChecked) LoadRequestsCommand.Execute(null);
        }

        private void ConnectAndSubscribe()
        {
            if (SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername))
            {
                if (_callbackHandler != null)
                {
                    _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;

                    _callbackHandler.FriendRequestReceived += HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived += HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged += HandleFriendStatusChanged;
                    Console.WriteLine($"SocialViewModel: Subscribed to callback events for {CurrentUserUsername}.");
                }
                else { /* Error Handling */ }
            }
            else { /* Error Handling */ }
        }

        private async Task ExecuteLoadFriendsListAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            FriendsList.Clear();
            try
            {
                SocialManagerService.FriendDto[] friends = await _proxy.getFriendsListAsync(CurrentUserUsername); // Usar el namespace completo
                if (friends != null)
                {
                    foreach (var friendDto in friends)
                    {
                        FriendsList.Add(new FriendDtoDisplay(friendDto));
                    }
                }
            }
            catch (Exception ex) { HandleError("Error loading friends list", ex); }
            finally { SetBusy(false); }
        }

        private async Task ExecuteLoadRequestsAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            ReceivedRequests.Clear();
            try
            {
                FriendRequestInfoDto[] requests = await _proxy.getFriendRequestsAsync(CurrentUserUsername);
                if (requests != null)
                {
                    foreach (var req in requests) ReceivedRequests.Add(req);
                }
            }
            catch (Exception ex) { HandleError("Error loading friend requests", ex); }
            finally { SetBusy(false); }
        }

        private async Task ExecuteSearchAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            SearchResults.Clear();
            try
            {
                PlayerSearchResultDto[] results = await _proxy.searchPlayersAsync(CurrentUserUsername, SearchQuery);
                if (results != null)
                {
                    foreach (var user in results) SearchResults.Add(user);
                }
            }
            catch (Exception ex) { HandleError("Error searching players", ex); }
            finally { SetBusy(false); }
        }

        private async Task ExecuteSendRequestAsync(PlayerSearchResultDto targetUser)
        {
            if (targetUser == null || !SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            try
            {
                OperationResultDto result = await _proxy.sendFriendRequestAsync(CurrentUserUsername, targetUser.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    SearchResults.Remove(targetUser); // Quitar de resultados si fue exitoso
                }
            }
            catch (Exception ex) { HandleError("Error sending friend request", ex); }
            finally { SetBusy(false); }
        }

        private async Task ExecuteRespondRequestAsync(FriendRequestInfoDto request, bool accept)
        {
            if (request == null || !SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            try
            {
                OperationResultDto result = await _proxy.respondToFriendRequestAsync(CurrentUserUsername, request.requesterUsername, accept);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    ReceivedRequests.Remove(request);
                    if (accept && IsFriendsListChecked) // Si aceptamos y estamos viendo la lista, recargarla
                    {
                        await ExecuteLoadFriendsListAsync();
                    }
                }
            }
            catch (Exception ex) { HandleError("Error responding to friend request", ex); }
            finally { SetBusy(false); }
        }

        private async Task ExecuteRemoveFriendAsync(FriendDtoDisplay friendToRemove)
        {
            if (friendToRemove == null || !SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to remove {friendToRemove.username}?", "Remove Friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult != MessageBoxResult.Yes) return;

            SetBusy(true);
            try
            {
                OperationResultDto result = await _proxy.removeFriendAsync(CurrentUserUsername, friendToRemove.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    FriendsList.Remove(friendToRemove);
                }
            }
            catch (Exception ex) { HandleError("Error removing friend", ex); }
            finally { SetBusy(false); }
        }

        // --- Handlers ---
        private void HandleFriendRequestReceived(string fromUsername)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                // Podríamos añadir un indicador visual en lugar de recargar toda la lista
                if (IsRequestsChecked) { await ExecuteLoadRequestsAsync(); }
                // Mostrar notificación (esto se manejará globalmente ahora)
            });
        }

        private void HandleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                // Mostrar notificación (esto se manejará globalmente ahora)
                if (accepted && IsFriendsListChecked) { await ExecuteLoadFriendsListAsync(); }
                // Actualizar lista de solicitudes enviadas si la tuvieras
            });
        }

        private void HandleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Console.WriteLine($"SocialViewModel: Received status change for {friendUsername} -> Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                var friend = FriendsList.FirstOrDefault(f => f.username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    Console.WriteLine($"SocialViewModel: Found friend {friendUsername} in list. Updating IsOnline to {isOnline}.");
                    friend.isOnline = isOnline; // Actualiza la propiedad, la UI reacciona
                }
                else { Console.WriteLine($"SocialViewModel: Friend {friendUsername} not found in current FriendsList."); }
            });
        }

        // --- Helpers ---
        private void SetBusy(bool busy) { IsBusy = busy; Application.Current.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested()); }
        private void HandleError(string message, Exception ex) { Console.WriteLine($"!!! {message}: {ex}"); MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        public void Cleanup()
        {
            if (_callbackHandler != null)
            {
                _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;
                Console.WriteLine($"SocialViewModel: Unsubscribed from callback events for {CurrentUserUsername}.");
            }
        }

    }
}