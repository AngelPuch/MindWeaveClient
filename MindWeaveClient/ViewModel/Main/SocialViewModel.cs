using MindWeaveClient.Properties.Langs; // Para Lang
using MindWeaveClient.Services; // Para SocialServiceClientManager y SocialCallbackHandler
using MindWeaveClient.SocialManagerService; // Para DTOs (FriendDto, etc.) y el Proxy (SocialManagerClient)
using System;
using System.Collections.ObjectModel; // Para ObservableCollection
using System.Linq;
using System.Threading.Tasks;
using System.Windows; // Para MessageBox y Dispatcher
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class SocialViewModel : BaseViewModel
    {
        private SocialManagerClient _proxy => SocialServiceClientManager.Instance.Proxy;
        private SocialCallbackHandler _callbackHandler => SocialServiceClientManager.Instance.CallbackHandler;

        private string _searchQuery;
        private bool _isFriendsListChecked = true;
        private bool _isAddFriendChecked;
        private bool _isRequestsChecked;
        private bool _isBusy; // Para indicar operaciones en curso

        public ObservableCollection<FriendDto> FriendsList { get; } = new ObservableCollection<FriendDto>();
        public ObservableCollection<PlayerSearchResultDto> SearchResults { get; } = new ObservableCollection<PlayerSearchResultDto>();
        public ObservableCollection<FriendRequestInfoDto> ReceivedRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();
        // Podrías añadir SentRequests si el servicio lo devuelve, aunque en la interfaz ISocialManager no estaba getSentRequests
        // public ObservableCollection<FriendRequestInfoDto> SentRequests { get; } = new ObservableCollection<FriendRequestInfoDto>();

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
            set { _isAddFriendChecked = value; OnPropertyChanged(); if (value) SearchResults.Clear(); SearchQuery = ""; } // Limpiar al cambiar a esta pestaña
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
        public string playerAvatarPath { get; private set; }

        // --- Comandos ---
        public ICommand LoadFriendsListCommand { get; }
        public ICommand LoadRequestsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand DeclineRequestCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand BackCommand { get; } // Para navegación

        private readonly Action _navigateBackAction; // Acción para volver atrás

        // TODO: Reemplazar "CurrentUserUsername" con la forma real de obtener el nombre de usuario logueado
        private string CurrentUserUsername => SessionService.username; // ¡¡¡CAMBIAR ESTO!!!

        public SocialViewModel(Action navigateBack)
        {
            _navigateBackAction = navigateBack;

            LoadFriendsListCommand = new RelayCommand(async (param) => await ExecuteLoadFriendsListAsync(), (param) => !IsBusy);
            LoadRequestsCommand = new RelayCommand(async (param) => await ExecuteLoadRequestsAsync(), (param) => !IsBusy);
            SearchCommand = new RelayCommand(async (param) => await ExecuteSearchAsync(), (param) => !IsBusy && !string.IsNullOrWhiteSpace(SearchQuery));
            SendRequestCommand = new RelayCommand(async (param) => await ExecuteSendRequestAsync(param as PlayerSearchResultDto), (param) => !IsBusy && param is PlayerSearchResultDto);
            AcceptRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, true), (param) => !IsBusy && param is FriendRequestInfoDto);
            DeclineRequestCommand = new RelayCommand(async (param) => await ExecuteRespondRequestAsync(param as FriendRequestInfoDto, false), (param) => !IsBusy && param is FriendRequestInfoDto);
            RemoveFriendCommand = new RelayCommand(async (param) => await ExecuteRemoveFriendAsync(param as FriendDto), (param) => !IsBusy && param is FriendDto);
            BackCommand = new RelayCommand((param) => _navigateBackAction?.Invoke());
            // Conectar al servicio y suscribirse a eventos de callback
            ConnectAndSubscribe();

            // Cargar datos iniciales
            if (IsFriendsListChecked) LoadFriendsListCommand.Execute(null);
            else if (IsRequestsChecked) LoadRequestsCommand.Execute(null);
        }

        private void ConnectAndSubscribe()
        {
            if (SocialServiceClientManager.Instance.EnsureConnected())
            {
                if (_callbackHandler != null)
                {
                    // Desuscribirse primero por si acaso
                    _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;

                    // Suscribirse
                    _callbackHandler.FriendRequestReceived += HandleFriendRequestReceived;
                    _callbackHandler.FriendResponseReceived += HandleFriendResponseReceived;
                    _callbackHandler.FriendStatusChanged += HandleFriendStatusChanged;
                }
            }
            else
            {
                MessageBox.Show("Could not connect to the social service.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Podrías intentar reconectar o deshabilitar funcionalidad
            }
        }

        private async Task ExecuteLoadFriendsListAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected()) return;
            SetBusy(true);
            FriendsList.Clear();
            try
            {
                var friends = await _proxy.getFriendsListAsync(CurrentUserUsername);
                if (friends != null)
                {
                    foreach (var friend in friends) FriendsList.Add(friend);
                }
            }
            catch (Exception ex)
            {
                HandleError("Error loading friends list", ex);
            }
            finally { SetBusy(false); }
        }

        private async Task ExecuteLoadRequestsAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected()) return;
            SetBusy(true);
            ReceivedRequests.Clear();
            // SentRequests.Clear(); // Si tuvieras lista de enviadas
            try
            {
                var requests = await _proxy.getFriendRequestsAsync(CurrentUserUsername);
                if (requests != null)
                {
                    foreach (var req in requests) ReceivedRequests.Add(req);
                }
                // Cargar enviadas si existiera el método en el servicio
            }
            catch (Exception ex)
            {
                HandleError("Error loading friend requests", ex);
            }
            finally { SetBusy(false); }
        }

        private async Task ExecuteSearchAsync()
        {
            if (!SocialServiceClientManager.Instance.EnsureConnected()) return;
            SetBusy(true);
            SearchResults.Clear();
            try
            {
                var results = await _proxy.searchPlayersAsync(CurrentUserUsername, SearchQuery);
                if (results != null)
                {
                    foreach (var user in results) SearchResults.Add(user);
                }
            }
            catch (Exception ex)
            {
                HandleError("Error searching players", ex);
            }
            finally { SetBusy(false); }
        }

        private async Task ExecuteSendRequestAsync(PlayerSearchResultDto targetUser)
        {
            if (targetUser == null || !SocialServiceClientManager.Instance.EnsureConnected()) return;
            SetBusy(true);
            try
            {
                OperationResultDto result = await _proxy.sendFriendRequestAsync(CurrentUserUsername, targetUser.username);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    // Opcional: Podrías quitar al usuario de los resultados de búsqueda o marcarlo como "pendiente"
                    SearchResults.Remove(targetUser); // Ejemplo simple: quitarlo
                }
            }
            catch (Exception ex)
            {
                HandleError("Error sending friend request", ex);
            }
            finally { SetBusy(false); }
        }

        private async Task ExecuteRespondRequestAsync(FriendRequestInfoDto request, bool accept)
        {
            if (request == null || !SocialServiceClientManager.Instance.EnsureConnected()) return;
            SetBusy(true);
            try
            {
                OperationResultDto result = await _proxy.respondToFriendRequestAsync(CurrentUserUsername, request.requesterUsername, accept);
                MessageBox.Show(result.message, result.success ? Lang.InfoMsgTitleSuccess : "Error", MessageBoxButton.OK, result.success ? MessageBoxImage.Information : MessageBoxImage.Warning);
                if (result.success)
                {
                    // Quitar de la lista de recibidas y, si se aceptó, actualizar lista de amigos
                    ReceivedRequests.Remove(request);
                    if (accept)
                    {
                        // Podrías recargar la lista de amigos o añadirlo directamente si tienes los datos
                        await ExecuteLoadFriendsListAsync(); // Recargar la lista
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("Error responding to friend request", ex);
            }
            finally { SetBusy(false); }
        }

        private async Task ExecuteRemoveFriendAsync(FriendDto friendToRemove)
        {
            if (friendToRemove == null || !SocialServiceClientManager.Instance.EnsureConnected()) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to remove {friendToRemove.username} from your friends list?", "Remove Friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
            catch (Exception ex)
            {
                HandleError("Error removing friend", ex);
            }
            finally { SetBusy(false); }
        }


        // --- Manejadores de Callbacks ---

        private void HandleFriendRequestReceived(string fromUsername)
        {
            // Ejecutar en hilo de UI
            Application.Current.Dispatcher.Invoke(async () =>
            {
                // Podrías añadir directamente a ReceivedRequests si tienes la fecha,
                // o simplemente marcar que hay que recargar la lista.
                // Recargar es más simple:
                if (IsRequestsChecked)
                {
                    await ExecuteLoadRequestsAsync();
                }
                // Opcional: Mostrar una notificación más persistente en la UI
            });
        }

        private void HandleFriendResponseReceived(string fromUsername, bool accepted)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                // Quitar de la lista de solicitudes enviadas (si la tuvieras)
                // Actualizar la lista de amigos si fue aceptado
                if (accepted)
                {
                    if (IsFriendsListChecked)
                    {
                        await ExecuteLoadFriendsListAsync();
                    }
                }
                // Opcional: Actualizar UI de solicitudes enviadas
            });
        }

        private void HandleFriendStatusChanged(string friendUsername, bool isOnline)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var friend = FriendsList.FirstOrDefault(f => f.username.Equals(friendUsername, StringComparison.OrdinalIgnoreCase));
                if (friend != null)
                {
                    friend.isOnline = isOnline;
                    // Forzar actualización de la UI si FriendDto no implementa INotifyPropertyChanged
                    // (ObservableCollection maneja adición/remoción, pero no cambios en propiedades de items)
                    // Una forma simple es remover y re-añadir, o implementar INotifyPropertyChanged en FriendDto
                    int index = FriendsList.IndexOf(friend);
                    if (index != -1)
                    {
                        FriendsList.RemoveAt(index);
                        FriendsList.Insert(index, friend); // Esto refresca la UI
                    }
                }
            });
        }


        // --- Helpers ---
        private void SetBusy(bool busy)
        {
            IsBusy = busy;
            // Forzar actualización del estado CanExecute de los comandos
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void HandleError(string message, Exception ex)
        {
            // TODO: Log the full exception ex
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // --- Limpieza ---
        // Llamar a esto cuando la ventana/página se cierre
        public void Cleanup()
        {
            if (_callbackHandler != null)
            {
                // Desuscribirse de eventos
                _callbackHandler.FriendRequestReceived -= HandleFriendRequestReceived;
                _callbackHandler.FriendResponseReceived -= HandleFriendResponseReceived;
                _callbackHandler.FriendStatusChanged -= HandleFriendStatusChanged;
            }
            // SocialServiceClientManager.Instance.Disconnect(); // Desconectar si es apropiado (ej. al cerrar la app)
        }
    }
}