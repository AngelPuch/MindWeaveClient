// MindWeaveClient/ViewModel/Main/SocialViewModel.cs
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel; // <-- AÑADIR para INotifyPropertyChanged
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace MindWeaveClient.ViewModel.Main
{
    // *** NUEVO: FriendDto ahora implementa INotifyPropertyChanged ***
    // Esto permite que la UI se actualice automáticamente cuando 'isOnline' cambie,
    // sin necesidad de remover/reinsertar en la lista.
    public class FriendDtoDisplay : BaseViewModel // Hereda de BaseViewModel para OnPropertyChanged
    {
        private bool isOnlineValue;
        public string username { get; set; }
        public string avatarPath { get; set; }
        public bool isOnline
        {
            get => isOnlineValue;
            set { isOnlineValue = value; OnPropertyChanged(); } // Notifica el cambio
        }

        // Constructor para mapear desde el DTO del servicio
        public FriendDtoDisplay(FriendDto dto)
        {
            this.username = dto.username;
            this.avatarPath = dto.avatarPath;
            this.isOnline = dto.isOnline; // Establecer valor inicial
        }
    }


    public class SocialViewModel : BaseViewModel
    {
        // Managers y Proxies (sin cambios)
        private SocialManagerClient _proxy => SocialServiceClientManager.Instance.Proxy;
        private SocialCallbackHandler _callbackHandler => SocialServiceClientManager.Instance.CallbackHandler;

        // Propiedades UI (FriendsList ahora usa FriendDtoDisplay)
        private string _searchQuery;
        private bool _isFriendsListChecked = true;
        private bool _isAddFriendChecked;
        private bool _isRequestsChecked;
        private bool _isBusy;

        // *** CAMBIO: Usar FriendDtoDisplay ***
        public ObservableCollection<FriendDtoDisplay> FriendsList { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<PlayerSearchResultDto> SearchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> ReceivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

        // Propiedades SearchQuery, Is*Checked, IsBusy (sin cambios)
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }
        public bool IsFriendsListChecked
        {
            get => _isFriendsListChecked;
            set { _isFriendsListChecked = value; OnPropertyChanged(); if (value) LoadFriendsListCommand.Execute(null); }
        }
        public bool IsAddFriendChecked
        {
            get => _isAddFriendChecked;
            set { _isAddFriendChecked = value; OnPropertyChanged(); if (value) { SearchResults.Clear(); SearchQuery = ""; } }
        }
        public bool IsRequestsChecked
        {
            get => _isRequestsChecked;
            set { _isRequestsChecked = value; OnPropertyChanged(); if (value) LoadRequestsCommand.Execute(null); }
        }
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // Comandos (RemoveFriendCommand ahora usa FriendDtoDisplay)
        public ICommand LoadFriendsListCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; } // Tipo de parámetro cambia a FriendDtoDisplay
        public ICommand BackCommand { get; }

        private readonly Action _navigateBackAction;
        private string CurrentUserUsername => SessionService.username;

        public SocialViewModel(Action navigateBack)
        {
            _navigateBackAction = navigateBack;

            // Definición de comandos (RemoveFriendCommand ajustado)
            LoadFriendsListCommand = new RelayCommand(async (param) => await ExecuteLoadFriendsListAsync(), (param) => !IsBusy);
            LoadRequestsCommand = new RelayCommand(async (param) => await ExecuteLoadRequestsAsync(), (param) => !IsBusy);
            SearchCommand = new RelayCommand(async (param) => await ExecuteSearchAsync(), (param) => !IsBusy && !string.IsNullOrWhiteSpace(SearchQuery));
            SendRequestCommand = new RelayCommand(async (param) => await ExecuteSendRequestAsync(param as PlayerSearchResultDto), (param) => !IsBusy && param is PlayerSearchResultDto);
            AcceptRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !IsBusy && param is FriendRequestInfoDto);
            DeclineRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !IsBusy && param is FriendRequestInfoDto);
            // *** CAMBIO: El parámetro ahora es FriendDtoDisplay ***
            RemoveFriendCommand = new RelayCommand(async (param) => await ExecuteRemoveFriendAsync(param as FriendDtoDisplay), (param) => !IsBusy && param is FriendDtoDisplay);
            BackCommand = new RelayCommand((param) => _navigateBackAction?.Invoke());

            // Conectar y suscribirse
            ConnectAndSubscribe();

            // Carga inicial
            if (IsFriendsListChecked) LoadFriendsListCommand.Execute(null);
            else if (IsRequestsChecked) LoadRequestsCommand.Execute(null);
        }

        // *** MÉTODO ConnectAndSubscribe MODIFICADO ***
        private void ConnectAndSubscribe()
        {
            // *** CAMBIO: Pasar username a EnsureConnected ***
            if (SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername))
            {
                if (_callbackHandler != null)
                {
                    // Desuscribirse primero
                    _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;
                    // Suscribirse
                    _callbackHandler.FriendRequestReceived += HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived += HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged += HandleFriendStatusChanged;
                    Console.WriteLine($"SocialViewModel: Subscribed to callback events for {CurrentUserUsername}.");
                }
                else
                {
                    Console.WriteLine($"SocialViewModel Error: CallbackHandler is null after EnsureConnected.");
                    MessageBox.Show("Callback handler not available.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                Console.WriteLine($"SocialViewModel Error: EnsureConnected failed for {CurrentUserUsername}.");
                MessageBox.Show("Could not connect to the social service.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // *** MÉTODO ExecuteLoadFriendsListAsync MODIFICADO ***
        private async Task ExecuteLoadFriendsListAsync()
        {
            // *** CAMBIO: Pasar username a EnsureConnected ***
            if (!SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;
            SetBusy(true);
            FriendsList.Clear();
            try
            {
                // La llamada al proxy no cambia
                FriendDto[] friends = await _proxy.getFriendsListAsync(CurrentUserUsername);
                if (friends != null)
                {
                    // *** CAMBIO: Mapear a FriendDtoDisplay ***
                    foreach (var friendDto in friends)
                    {
                        FriendsList.Add(new FriendDtoDisplay(friendDto));
                    }
                    Console.WriteLine($"Loaded {friends.Length} friends for {CurrentUserUsername}.");
                }
                else
                {
                    Console.WriteLine($"getFriendsListAsync returned null for {CurrentUserUsername}.");
                }
            }
            catch (Exception ex) { HandleError("Error loading friends list", ex); }
            finally { SetBusy(false); }
        }

        // ExecuteLoadRequestsAsync (sin cambios)
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


        // ExecuteSearchAsync (sin cambios)
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

        // ExecuteSendRequestAsync (sin cambios)
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
                    SearchResults.Remove(targetUser);
                }
            }
            catch (Exception ex) { HandleError("Error sending friend request", ex); }
            finally { SetBusy(false); }
        }

        // ExecuteRespondRequestAsync (Refresca lista de amigos al aceptar)
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
                    if (accept)
                    {
                        // *** CAMBIO: Refrescar lista de amigos ***
                        await ExecuteLoadFriendsListAsync();
                    }
                }
            }
            catch (Exception ex) { HandleError("Error responding to friend request", ex); }
            finally { SetBusy(false); }
        }

        // *** MÉTODO ExecuteRemoveFriendAsync MODIFICADO ***
        // Recibe FriendDtoDisplay
        private async Task ExecuteRemoveFriendAsync(FriendDtoDisplay friendToRemove) // Cambiado el tipo
        {
            if (friendToRemove == null || !SocialServiceClientManager.Instance.EnsureConnected(CurrentUserUsername)) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to remove {friendToRemove.username} from your friends list?", "Remove Friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult != MessageBoxResult.Yes) return;

            SetBusy(true);
            try
            {
                // La llamada al proxy usa el username
                OperationResultDto result = await _proxy.removeFriendAsync(CurrentUserUsername, friendToRemove.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    // *** CAMBIO: Remover el objeto FriendDtoDisplay de la lista ***
                    FriendsList.Remove(friendToRemove);
                }
            }
            catch (Exception ex) { HandleError("Error removing friend", ex); }
            finally { SetBusy(false); }
        }


        // --- Manejadores de Callbacks ---

        // HandleFriendRequestReceived (sin cambios)
        private void HandleFriendRequestReceived(string fromUsername)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (IsRequestsChecked)
                {
                    await ExecuteLoadRequestsAsync();
                }
                MessageBox.Show($"New friend request from: {fromUsername}", "Friend Request", MessageBoxButton.OK, MessageBoxImage.Information); // Considera una notificación menos intrusiva
            });
        }


        // HandleFriendResponseReceived (Refresca lista de amigos al aceptar)
        private void HandleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                string message = accepted ? $"{fromUsername} accepted your friend request!" : $"{fromUsername} declined your friend request.";
                MessageBox.Show(message, "Friend Request Response", MessageBoxButton.OK, MessageBoxImage.Information); // Considera notificación menos intrusiva

                if (accepted)
                {
                    // *** CAMBIO: Refrescar lista de amigos ***
                    if (IsFriendsListChecked)
                    {
                        await ExecuteLoadFriendsListAsync();
                    }
                }
                // Podrías tener una lista de "Sent Requests" y quitarla de ahí
            });
        }

        // *** MÉTODO HandleFriendStatusChanged MODIFICADO ***
        private void HandleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Console.WriteLine($"SocialViewModel: Received status change for {friendUsername} -> Online: {isOnline}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Buscar el amigo en la lista por username
                var friend = FriendsList.FirstOrDefault(f => f.username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    Console.WriteLine($"SocialViewModel: Found friend {friendUsername} in list. Updating IsOnline to {isOnline}.");
                    // *** CAMBIO: Simplemente actualizar la propiedad ***
                    // Como FriendDtoDisplay implementa INotifyPropertyChanged, la UI se actualizará sola.
                    friend.isOnline = isOnline;
                }
                else
                {
                    Console.WriteLine($"SocialViewModel: Friend {friendUsername} not found in current FriendsList.");
                    // Podría pasar si la lista no está actualizada o si el evento llega antes de cargar la lista.
                    // Considera recargar la lista si esto es un problema frecuente.
                    // if(IsFriendsListChecked) { LoadFriendsListCommand.Execute(null); }
                }
            });
        }


        // --- Helpers ---
        // SetBusy (sin cambios)
        private void SetBusy(bool busy) { /* ... */ IsBusy = busy; CommandManager.InvalidateRequerySuggested(); }
        // HandleError (sin cambios)
        private void HandleError(string message, Exception ex) { /* ... */ Console.WriteLine($"!!! {message}: {ex}"); MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

        // --- Limpieza ---
        public void Cleanup()
        {
            if (_callbackHandler != null)
            {
                // Desuscribirse
                _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;
                Console.WriteLine($"SocialViewModel: Unsubscribed from callback events for {CurrentUserUsername}.");
            }
            // NO llamamos a Disconnect aquí necesariamente, podría hacerse al cerrar sesión o la app.
            // SocialServiceClientManager.Instance.Disconnect();
        }
    }
}