// MindWeaveClient/ViewModel/Game/LobbyViewModel.cs
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs; // Para Lang
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService; // *** Necesario para SocialManagerClient y FriendDto ***
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Main; // *** Necesario para FriendDtoDisplay ***
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MindWeaveClient.ViewModel.Game
{
    public class LobbyViewModel : BaseViewModel
    {
        // --- Managers y Proxies ---
        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;
        private MatchmakingCallbackHandler matchmakingCallbackHandler => MatchmakingServiceClientManager.Instance.CallbackHandler;
        private SocialManagerClient socialProxy => SocialServiceClientManager.Instance.Proxy; // *** Proxy Social ***

        // --- Navegación ---
        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;

        // --- Propiedades del Lobby ---
        private string lobbyCodeValue;
        public string lobbyCode { get => lobbyCodeValue; set { lobbyCodeValue = value; OnPropertyChanged(); } }

        private string hostUsernameValue;
        public string hostUsername { get => hostUsernameValue; set { hostUsernameValue = value; OnPropertyChanged(); OnPropertyChanged(nameof(isHost)); } }

        public ObservableCollection<string> players { get; } = new ObservableCollection<string>();

        private LobbySettingsDto currentSettingsValue;
        public LobbySettingsDto currentSettings { get => currentSettingsValue; set { currentSettingsValue = value; OnPropertyChanged(); } }

        public bool isHost => hostUsernameValue == SessionService.username;

        private bool isBusyValue;
        public bool isBusy { get => isBusyValue; set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } } // Llama a helper

        // *** NUEVO: Lista de Amigos Online para Invitar ***
        public ObservableCollection<FriendDtoDisplay> onlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();


        // --- Comandos ---
        public ICommand leaveLobbyCommand { get; }
        public ICommand startGameCommand { get; }
        public ICommand inviteFriendCommand { get; } // *** Ahora con lógica ***
        public ICommand kickPlayerCommand { get; }
        public ICommand uploadImageCommand { get; }
        public ICommand changeSettingsCommand { get; }
        public ICommand refreshFriendsCommand { get; } // *** NUEVO: Para recargar amigos ***


        public LobbyViewModel(LobbyStateDto initialState, Action<Page> navigateToAction, Action navigateBackAction)
        {
            this.navigateTo = navigateToAction;
            this.navigateBack = navigateBackAction;

            // Definir Comandos
            leaveLobbyCommand = new RelayCommand(executeLeaveLobby, param => !isBusy);
            startGameCommand = new RelayCommand(executeStartGame, param => isHost && !isBusy /*&& players.Count >= 4*/ ); // Podrías requerir 4
            inviteFriendCommand = new RelayCommand(executeInviteFriend, param => isHost && !isBusy && param is FriendDtoDisplay friend && friend.isOnline && !players.Contains(friend.username)); // *** Condición CanExecute ***
            kickPlayerCommand = new RelayCommand(executeKickPlayer, p => isHost && !isBusy && p is string target && target != hostUsername); // *** Comando Kick ***
            uploadImageCommand = new RelayCommand(p => MessageBox.Show("Upload image TBD"), param => isHost && !isBusy);
            changeSettingsCommand = new RelayCommand(p => MessageBox.Show("Change settings TBD"), param => isHost && !isBusy);
            refreshFriendsCommand = new RelayCommand(async p => await loadOnlineFriendsAsync(), p => isHost && !isBusy); // *** Comando Refresh ***


            subscribeToCallbacks();

            if (initialState != null) { updateState(initialState); }
            else { lobbyCode = "Joining..."; hostUsername = "Loading..."; }

            // Cargar amigos si somos el host
            if (isHost) { refreshFriendsCommand.Execute(null); }
        }

        // --- Lógica de Comandos ---

        private void executeLeaveLobby(object parameter)
        {
            // (Sin cambios)
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;
            isBusy = true;
            try
            {
                matchmakingProxy.leaveLobby(SessionService.username, lobbyCode);
                navigateBack?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show($"Error leaving lobby: {ex.Message}", "Error"); } // TODO: Lang
            finally { isBusy = false; }
        }

        private void executeStartGame(object parameter)
        {
            // *** VALIDACIÓN: Asegurar 4 jugadores ***
            if (!isHost || players.Count < 4) // Cambiar a == 4 si es estricto
            {
                MessageBox.Show("Need exactly 4 players to start.", "Cannot Start Game", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                return;
            }
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            isBusy = true;
            try
            {
                matchmakingProxy.startGame(SessionService.username, lobbyCode);
                // La navegación ocurre en el callback handleMatchFound
            }
            catch (Exception ex) { MessageBox.Show($"Error starting game: {ex.Message}", "Error"); isBusy = false; } // TODO: Lang
            // isBusy se pondrá en false en el callback o si hay error
        }

        // *** NUEVO: Lógica para Kick Player ***
        private void executeKickPlayer(object parameter)
        {
            if (!(parameter is string playerToKick) || !isHost || playerToKick == hostUsername) return;
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            // Confirmación (Opcional pero recomendado)
            var confirmResult = MessageBox.Show($"Are you sure you want to kick {playerToKick}?", "Kick Player", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmResult != MessageBoxResult.Yes) return;

            try
            {
                // Llamada OneWay al servidor
                matchmakingProxy.kickPlayer(SessionService.username, playerToKick, lobbyCode);
                // La UI se actualizará cuando llegue el callback updateLobbyState
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error kicking player: {ex.Message}", "Error"); // TODO: Lang
            }
        }

        // *** NUEVO: Lógica para Invitar Amigo ***
        private async Task loadOnlineFriendsAsync()
        {
            if (!isHost || !SocialServiceClientManager.Instance.EnsureConnected(SessionService.username)) return;

            SetBusy(true); // Usar el helper SetBusy
            onlineFriends.Clear();
            try
            {
                // Usar el proxy social para obtener la lista de amigos
                SocialManagerService.FriendDto[] friends = await socialProxy.getFriendsListAsync(SessionService.username); // Namespace completo
                if (friends != null)
                {
                    foreach (var friendDto in friends.Where(f => f.isOnline)) // Filtrar solo online
                    {
                        // Crear FriendDtoDisplay (asegúrate que sea accesible)
                        onlineFriends.Add(new FriendDtoDisplay(friendDto));
                    }
                    Console.WriteLine($"Loaded {onlineFriends.Count} online friends for lobby invite list.");
                }
            }
            catch (Exception ex) { HandleError("Error loading online friends", ex); } // Usar helper HandleError
            finally { SetBusy(false); } // Usar helper SetBusy
        }

        private void executeInviteFriend(object parameter)
        {
            if (!(parameter is FriendDtoDisplay friendToInvite)) return;
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            // No necesitamos isBusy aquí porque es OneWay
            try
            {
                Console.WriteLine($"Sending invite to {friendToInvite.username} for lobby {lobbyCode}");
                matchmakingProxy.inviteToLobby(SessionService.username, friendToInvite.username, lobbyCode);
                // Opcional: Mostrar un mensaje temporal "Invitación enviada"
                MessageBox.Show($"Invitation sent to {friendToInvite.username}.", "Invitation Sent", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
            }
            catch (Exception ex)
            {
                HandleError("Error sending lobby invite", ex);
            }
        }


        // --- Suscripción y Manejo de Callbacks ---
        // (handleLobbyStateUpdate, handleMatchFound, handleKickedFromLobby sin cambios)
        private void subscribeToCallbacks()
        {
            if (matchmakingCallbackHandler != null)
            {
                matchmakingCallbackHandler.LobbyStateUpdated -= handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound -= handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby -= handleKickedFromLobby;
                // No necesitamos InviteReceived aquí, se maneja globalmente

                matchmakingCallbackHandler.LobbyStateUpdated += handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound += handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby += handleKickedFromLobby;
            }
        }

        private void unsubscribeFromCallbacks()
        {
            if (matchmakingCallbackHandler != null)
            {
                matchmakingCallbackHandler.LobbyStateUpdated -= handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound -= handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby -= handleKickedFromLobby;
            }
        }

        private void handleLobbyStateUpdate(LobbyStateDto newState)
        {
            if (newState != null && (string.IsNullOrEmpty(this.lobbyCode) || this.lobbyCode == "Joining..." || newState.lobbyId == this.lobbyCode))
            {
                Application.Current.Dispatcher.Invoke(() => updateState(newState));
            }
        }

        private void handleMatchFound(string matchId, List<string> playerUsernames)
        {
            if (playerUsernames != null && playerUsernames.Contains(SessionService.username))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    isBusy = false; // Detener estado ocupado si startGame lo activó
                    MessageBox.Show($"Match found! Starting game (ID: {matchId})...", "Match Start", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
                    unsubscribeFromCallbacks();

                    var gameWindow = new GameWindow();
                    // gameWindow.DataContext = new GameViewModel(matchId, playerUsernames); // Pasar datos necesarios
                    gameWindow.Show();

                    // Cerrar ventana actual o navegar
                    var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive); // O busca por tipo específico si es necesario
                    currentWindow?.Close();
                });
            }
        }

        private void handleKickedFromLobby(string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"You have been kicked. Reason: {reason}", "Kicked", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                unsubscribeFromCallbacks();
                navigateBack?.Invoke();
            });
        }

        // --- Métodos Auxiliares ---
        private void updateState(LobbyStateDto state)
        {
            // (Sin cambios)
            lobbyCode = state.lobbyId;
            hostUsername = state.hostUsername;
            currentSettings = state.currentSettingsDto;

            var playersToRemove = players.Except(state.players).ToList();
            var playersToAdd = state.players.Except(players).ToList();

            foreach (var p in playersToRemove) players.Remove(p);
            foreach (var p in playersToAdd) players.Add(p);

            OnPropertyChanged(nameof(isHost)); // Notificar cambio por si acaso
            RaiseCanExecuteChangedOnCommands(); // Actualizar CanExecute
        }

        // *** NUEVO: Helper para actualizar CanExecute de comandos ***
        private void RaiseCanExecuteChangedOnCommands()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                ((RelayCommand)leaveLobbyCommand).RaiseCanExecuteChanged();
                ((RelayCommand)startGameCommand).RaiseCanExecuteChanged();
                ((RelayCommand)inviteFriendCommand).RaiseCanExecuteChanged();
                ((RelayCommand)kickPlayerCommand).RaiseCanExecuteChanged();
                ((RelayCommand)uploadImageCommand).RaiseCanExecuteChanged();
                ((RelayCommand)changeSettingsCommand).RaiseCanExecuteChanged();
                ((RelayCommand)refreshFriendsCommand).RaiseCanExecuteChanged();
            });
        }
        // *** NUEVO: Helpers SetBusy y HandleError ***
        private void SetBusy(bool busy)
        {
            isBusy = busy;
            // No es necesario llamar a RaiseCanExecuteChangedOnCommands aquí
            // porque el setter de IsBusy ya lo hace.
        }

        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"!!! {message}: {ex}");
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
        }


        // --- Limpieza ---
        public void cleanup()
        {
            // (Sin cambios)
            unsubscribeFromCallbacks();
        }
    }
}