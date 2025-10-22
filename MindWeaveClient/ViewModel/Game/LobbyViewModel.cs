// MindWeaveClient/ViewModel/Game/LobbyViewModel.cs
using MindWeaveClient.MatchmakingService; // Para LobbyStateDto, LobbySettingsDto
using MindWeaveClient.Services; // Para SessionService y los managers de servicio
using System;
using System.Collections.ObjectModel; // Para ObservableCollection
using System.Linq;
using System.Windows; // Para MessageBox y Dispatcher
using System.Windows.Input; // Para ICommand
using MindWeaveClient.View.Game; // Para GameWindow
using System.Windows.Controls; // Para Page
using System.Collections.Generic; // <--- AÑADIR ESTE
using MindWeaveClient.View.Main;


namespace MindWeaveClient.ViewModel.Game // Podrías crear un subnamespace 'Game' si prefieres
{
    public class LobbyViewModel : BaseViewModel
    {
        // --- Managers y Proxies ---
        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;
        private MatchmakingCallbackHandler matchmakingCallbackHandler => MatchmakingServiceClientManager.Instance.CallbackHandler;

        // --- Navegación ---
        private readonly Action<Page> navigateTo; // Para navegar a otras páginas (ej. Juego)
        private readonly Action navigateBack; // Para volver al menú principal

        // --- Propiedades del Lobby (bindeables a la UI) ---
        private string lobbyCodeValue;
        public string lobbyCode
        {
            get => lobbyCodeValue;
            set { lobbyCodeValue = value; OnPropertyChanged(); }
        }

        private string hostUsernameValue;
        public string hostUsername
        {
            get => hostUsernameValue;
            set { hostUsernameValue = value; OnPropertyChanged(); OnPropertyChanged(nameof(isHost)); /* Actualiza si el usuario actual es host */ }
        }

        // Usamos ObservableCollection para que la lista se actualice automáticamente en la UI
        public ObservableCollection<string> players { get; } = new ObservableCollection<string>();

        private LobbySettingsDto currentSettingsValue;
        public LobbySettingsDto currentSettings
        {
            get => currentSettingsValue;
            set { currentSettingsValue = value; OnPropertyChanged(); /* Podrías necesitar actualizar la UI de settings */ }
        }

        // Propiedad para saber si el usuario actual es el host
        public bool isHost => hostUsernameValue == SessionService.username;

        // Propiedad para estado ocupado
        private bool isBusyValue;
        public bool isBusy
        {
            get => isBusyValue;
            set { isBusyValue = value; OnPropertyChanged(); /* Actualiza CanExecute de comandos si es necesario */ }
        }

        // --- Comandos ---
        public ICommand leaveLobbyCommand { get; }
        public ICommand startGameCommand { get; }
        public ICommand inviteFriendCommand { get; } // A implementar
        public ICommand kickPlayerCommand { get; } // A implementar
        public ICommand uploadImageCommand { get; } // A implementar (requiere lógica de host)
        public ICommand changeSettingsCommand { get; } // A implementar (requiere lógica de host)

        // --- Constructor ---
        public LobbyViewModel(LobbyStateDto initialState, Action<Page> navigateToAction, Action navigateBackAction)
        {
            if (initialState == null)
            {
                // Manejar error o volver atrás si el estado inicial es nulo
                MessageBox.Show("Error: Invalid lobby state received.", "Lobby Error", MessageBoxButton.OK, MessageBoxImage.Error);
                navigateBackAction?.Invoke();
                return; // Salir del constructor
            }

            this.navigateTo = navigateToAction;
            this.navigateBack = navigateBackAction;

            // Inicializar propiedades con el estado recibido
            updateState(initialState); // Usamos un método para reutilizar la lógica

            // Definir Comandos
            leaveLobbyCommand = new RelayCommand(executeLeaveLobby, param => !isBusy);
            startGameCommand = new RelayCommand(executeStartGame, param => isHost && !isBusy /*&& players.Count > 1*/ );
            inviteFriendCommand = new RelayCommand(p => MessageBox.Show("Invite friend TBD"), param => isHost && !isBusy);
            // kickPlayerCommand ya estaba bien, pero lo revisamos para consistencia:
            kickPlayerCommand = new RelayCommand(p => MessageBox.Show("Kick player TBD"), p => isHost && !isBusy && p is string target && target != hostUsername);
            uploadImageCommand = new RelayCommand(p => MessageBox.Show("Upload image TBD"), param => isHost && !isBusy);
            changeSettingsCommand = new RelayCommand(p => MessageBox.Show("Change settings TBD"), param => isHost && !isBusy);

            // Suscribirse a los callbacks del servicio de Matchmaking
            subscribeToCallbacks();
        }

        // --- Lógica de Comandos ---

        private void executeLeaveLobby(object parameter)
        {
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            isBusy = true; // Opcional, si la operación es rápida
            try
            {
                // Llama al método OneWay del servidor
                matchmakingProxy.leaveLobby(SessionService.username, lobbyCode);
                // Como es OneWay, no esperamos respuesta. Simplemente navegamos atrás.
                navigateBack?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leaving lobby: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
                // Considerar qué hacer aquí, ¿intentar desconectar?
            }
            finally
            {
                isBusy = false;
            }
        }

        private void executeStartGame(object parameter)
        {
            if (!isHost || !MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            isBusy = true; // Opcional, si la operación es rápida
            try
            {
                // Llama al método OneWay del servidor
                matchmakingProxy.startGame(SessionService.username, lobbyCode);
                // El servidor enviará un callback 'matchFound' a todos los jugadores (incluido el host)
                // La navegación a la pantalla de juego se manejará en el callback 'HandleMatchFound'
                MessageBox.Show("Start game signal sent...", "Starting", MessageBoxButton.OK, MessageBoxImage.Information); // Mensaje temporal
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
            }
            finally
            {
                isBusy = false;
            }
        }


        // --- Suscripción y Manejo de Callbacks ---

        private void subscribeToCallbacks()
        {
            if (matchmakingCallbackHandler != null)
            {
                // Desuscribirse primero para evitar duplicados si se entra/sale rápido
                matchmakingCallbackHandler.LobbyStateUpdated -= handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound -= handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby -= handleKickedFromLobby;
                // Añadir otros callbacks (LobbyCreationFailed, InviteReceived si son relevantes aquí)

                // Suscribirse
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
                // Quitar otros callbacks
            }
        }

        // Método llamado por el CallbackHandler cuando llega un nuevo estado
        private void handleLobbyStateUpdate(LobbyStateDto newState)
        {
            // Asegurarse de que la actualización es para *este* lobby
            if (newState != null && newState.lobbyId == this.lobbyCode)
            {
                // Actualizar en el hilo de la UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    updateState(newState);
                });
            }
        }

        // Método llamado por el CallbackHandler cuando el servidor inicia la partida
        private void handleMatchFound(string matchId, List<string> playerUsernames)
        {
            // Verificar si este cliente está en la lista de jugadores de la partida encontrada
            if (playerUsernames != null && playerUsernames.Contains(SessionService.username))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Match found! Starting game (ID: {matchId})...", "Match Start", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
                    unsubscribeFromCallbacks(); // Dejar de escuchar eventos del lobby
                    // Navegar a la pantalla de juego
                    // TODO: Crear GameWindowViewModel y pasarle matchId, players, etc.
                    var gameWindow = new GameWindow();
                    // gameWindow.DataContext = new GameViewModel(matchId, playerUsernames); // Ejemplo
                    gameWindow.Show();

                    // Cerrar la ventana principal (o solo navegar el frame si MainWindow contiene el frame)
                    // Esta lógica depende de tu estructura de ventanas/navegación
                    var currentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(); // Asume que estás en MainWindow
                    currentWindow?.Close(); // Cierra MainWindow si la partida empieza en una nueva ventana

                    // Si usas navegación dentro de MainWindow:
                    // navigateTo(new GamePage(matchId, playerUsernames)); // Navegar dentro del Frame
                });
            }
        }

        // Método llamado por el CallbackHandler si este usuario es expulsado
        private void handleKickedFromLobby(string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show($"You have been kicked from the lobby. Reason: {reason}", "Kicked", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                unsubscribeFromCallbacks();
                navigateBack?.Invoke(); // Volver al menú principal
            });
        }

        // --- Métodos Auxiliares ---

        // Actualiza las propiedades del ViewModel con un nuevo estado
        private void updateState(LobbyStateDto state)
        {
            lobbyCode = state.lobbyId;
            hostUsername = state.hostUsername;
            currentSettings = state.currentSettingsDto; // Actualiza settings también

            // Actualizar la lista de jugadores de forma eficiente
            // (Evita recrear la colección, modifica la existente para mantener bindings)
            var playersToRemove = players.Except(state.players).ToList();
            var playersToAdd = state.players.Except(players).ToList();

            foreach (var p in playersToRemove) players.Remove(p);
            foreach (var p in playersToAdd) players.Add(p);

            // Re-evaluar CanExecute de los comandos que dependen del estado
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        // --- Limpieza ---
        // Llamar a este método cuando la página del Lobby se cierre o navegue fuera
        public void cleanup()
        {
            unsubscribeFromCallbacks();
            // No necesariamente desconectamos el servicio aquí,
            // podría seguir usándose (ej. para recibir invitaciones en el menú principal)
            // MatchmakingServiceClientManager.Instance.Disconnect();
        }
    }
}