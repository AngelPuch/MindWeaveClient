using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs; 
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.View.Dialogs;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace MindWeaveClient.ViewModel.Game
{

    
    public class LobbyViewModel : BaseViewModel
    {
        public bool isGuestUser => SessionService.isGuest;
        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;
        private MatchmakingCallbackHandler matchmakingCallbackHandler => MatchmakingServiceClientManager.Instance.CallbackHandler;
        private SocialManagerClient socialProxy => SocialServiceClientManager.Instance.Proxy;
        private ChatManagerClient chatProxy => ChatServiceClientManager.Instance.Proxy;
        private ChatCallbackHandler chatCallbackHandler => ChatServiceClientManager.Instance.CallbackHandler;


        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;

        private string lobbyCodeValue;
        public string lobbyCode { get => lobbyCodeValue; set { lobbyCodeValue = value; OnPropertyChanged(); } }
        private string hostUsernameValue;
        public string hostUsername { get => hostUsernameValue; set { hostUsernameValue = value; OnPropertyChanged(); OnPropertyChanged(nameof(isHost)); } }
        public ObservableCollection<string> players { get; } = new ObservableCollection<string>();
        private LobbySettingsDto currentSettingsValue;
        public LobbySettingsDto currentSettings { get => currentSettingsValue; set { currentSettingsValue = value; OnPropertyChanged(); } }
        public bool isHost => hostUsernameValue == SessionService.username;
        private bool isBusyValue;
        public bool isBusy { get => isBusyValue; set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } }
        public ObservableCollection<FriendDtoDisplay> onlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();

        public ObservableCollection<ChatMessageDisplayViewModel> chatMessages { get; } = new ObservableCollection<ChatMessageDisplayViewModel>();
        private string currentChatMessageValue = string.Empty;
        public string currentChatMessage
        {
            get => currentChatMessageValue;
            set { currentChatMessageValue = value; OnPropertyChanged(); ((RelayCommand)sendMessageCommand).RaiseCanExecuteChanged(); }
        }
        public ICommand leaveLobbyCommand { get; }
        public ICommand startGameCommand { get; }
        public ICommand inviteFriendCommand { get; }
        public ICommand kickPlayerCommand { get; }
        public ICommand uploadImageCommand { get; }
        public ICommand changeSettingsCommand { get; }
        public ICommand refreshFriendsCommand { get; }
        public ICommand sendMessageCommand { get; }
        public ICommand inviteGuestCommand { get; }

        public LobbyViewModel(LobbyStateDto initialState, Action<Page> navigateToAction, Action navigateBackAction)
        {
            this.navigateTo = navigateToAction;
            this.navigateBack = navigateBackAction;

            leaveLobbyCommand = new RelayCommand(executeLeaveLobby, param => !isBusy);
            startGameCommand = new RelayCommand(executeStartGame, param => isHost && !isBusy && !isGuestUser);
            inviteFriendCommand = new RelayCommand(executeInviteFriend, param => isHost && !isBusy && !isGuestUser && param is FriendDtoDisplay friend && friend.isOnline && !players.Contains(friend.username));
            kickPlayerCommand = new RelayCommand(executeKickPlayer, p => isHost && !isBusy && !isGuestUser && p is string target && target != hostUsername);
            uploadImageCommand = new RelayCommand(p => MessageBox.Show("Upload image TBD"), param => isHost && !isBusy && !isGuestUser);
            changeSettingsCommand = new RelayCommand(p => MessageBox.Show("Change settings TBD"), param => isHost && !isBusy && !isGuestUser);
            refreshFriendsCommand = new RelayCommand(async p => await loadOnlineFriendsAsync(), p => isHost && !isBusy && !isGuestUser);
            sendMessageCommand = new RelayCommand(executeSendMessage, canExecuteSendMessage);
            inviteGuestCommand = new RelayCommand(async p => await executeInviteGuestAsync(), p => isHost && !isBusy && !isGuestUser);

            subscribeToCallbacks();

            if (initialState != null) {
                connectToChat(initialState.lobbyId);
                updateState(initialState);
            }
            else { lobbyCode = "Joining..."; hostUsername = "Loading..."; }

            if (isHost && !isGuestUser) { refreshFriendsCommand.Execute(null); }
            OnPropertyChanged(nameof(isGuestUser));
            RaiseCanExecuteChangedOnCommands();
            Debug.WriteLine($"LobbyViewModel initialized. Initial State Null: {initialState == null}");

        }

        private async Task executeInviteGuestAsync()
        {
            if (!isHost || isGuestUser || isBusy) return;

            var dialog = new GuestInputDialog();
            dialog.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

            bool? dialogResult = dialog.ShowDialog();

            if (dialogResult != true)
            {
                return;
            }

            string guestEmail = dialog.GuestEmail; 

            if (string.IsNullOrWhiteSpace(guestEmail))
            {
                return;
            }

            if (!Regex.IsMatch(guestEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show(Lang.GlobalErrorInvalidEmailFormat, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!MatchmakingServiceClientManager.Instance.EnsureConnected())
            {
                MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBusy(true);

            var invitationData = new GuestInvitationDto
            {
                inviterUsername = SessionService.username,
                guestEmail = guestEmail.Trim(),
                lobbyCode = this.lobbyCode
            };

            try
            {
                await matchmakingProxy.inviteGuestByEmailAsync(invitationData);
                MessageBox.Show($"Invitation sent to {guestEmail}.", Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError(Lang.ErrorSendingGuestInvitation, ex);
                MatchmakingServiceClientManager.Instance.Disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void connectToChat(string lobbyIdToConnect)
        {
            Debug.WriteLine($"Attempting to connect to chat for lobby {lobbyIdToConnect}...");
            if (ChatServiceClientManager.Instance.Connect(SessionService.username, lobbyIdToConnect))
            {
                Debug.WriteLine($"Chat connected successfully for lobby {lobbyIdToConnect}. Subscribing...");
                subscribeToChatCallbacks(); // Subscribe specifically to chat messages
            }
            else
            {
                Debug.WriteLine($"Failed to connect to chat service for lobby {lobbyIdToConnect}.");
                MessageBox.Show("Could not connect to lobby chat.", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void disconnectFromChat()
        {
            Debug.WriteLine("Disconnecting from chat service...");
            unsubscribeFromChatCallbacks();
            ChatServiceClientManager.Instance.Disconnect();
        }

        private void subscribeToChatCallbacks()
        {
            if (chatCallbackHandler != null)
            {
                chatCallbackHandler.LobbyMessageReceived -= handleLobbyMessageReceived; // Prevent duplicates
                chatCallbackHandler.LobbyMessageReceived += handleLobbyMessageReceived;
                Debug.WriteLine("Subscribed to LobbyMessageReceived.");
            }
            else
            {
                Debug.WriteLine("ERROR: Cannot subscribe to chat callbacks, handler is null.");
            }
        }

        private void unsubscribeFromChatCallbacks()
        {
            if (chatCallbackHandler != null)
            {
                chatCallbackHandler.LobbyMessageReceived -= handleLobbyMessageReceived;
                Debug.WriteLine("Unsubscribed from LobbyMessageReceived.");
            }
        }

        private void handleLobbyMessageReceived(ChatMessageDto messageDto)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                chatMessages.Add(new ChatMessageDisplayViewModel(messageDto));
                Debug.WriteLine($"Added message from {messageDto.senderUsername} to UI collection.");
            });
        }


        private bool canExecuteSendMessage(object parameter)
        {
            return !string.IsNullOrWhiteSpace(currentChatMessage) && ChatServiceClientManager.Instance.IsConnected();
        }

        private void executeSendMessage(object parameter)
        {
            if (!canExecuteSendMessage(parameter)) return;

            string messageToSend = currentChatMessage;
            currentChatMessage = string.Empty;

            try
            {
                chatProxy.sendLobbyMessageAsync(SessionService.username, lobbyCode, messageToSend);
                Debug.WriteLine($"Sent message: '{messageToSend}'");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending chat message: {ex.Message}");
                MessageBox.Show($"Failed to send message: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                // Consider attempting to reconnect chat if channel faulted
                if (chatProxy.State == CommunicationState.Faulted)
                {
                    disconnectFromChat();
                    connectToChat(lobbyCode);
                }
            }
        }




        private void executeLeaveLobby(object parameter)
        {
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;
            isBusy = true;
            try
            {
                matchmakingProxy.leaveLobby(SessionService.username, lobbyCode);
                navigateBack?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show($"Error leaving lobby: {ex.Message}", "Error"); }
            finally { isBusy = false; }
        }

        private async void executeStartGame(object parameter)
        {
            if (!isHost || players.Count < 4) 
            {
                MessageBox.Show("Need exactly 4 players to start.", "Cannot Start Game", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                return;
            }
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            isBusy = true;
            try
            {
                matchmakingProxy.startGame(SessionService.username, lobbyCode);
            }
            catch (Exception ex) { MessageBox.Show($"Error starting game: {ex.Message}", "Error"); isBusy = false; } // TODO: Lang
        }

        private void executeKickPlayer(object parameter)
        {
            if (!(parameter is string playerToKick) || !isHost || playerToKick == hostUsername) return;
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            var confirmResult = MessageBox.Show($"Are you sure you want to kick {playerToKick}?", "Kick Player", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmResult != MessageBoxResult.Yes) return;

            try
            {
                matchmakingProxy.kickPlayer(SessionService.username, playerToKick, lobbyCode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error kicking player: {ex.Message}", "Error"); // TODO: Lang
            }
        }

        private async Task loadOnlineFriendsAsync()
        {
            if (!isHost || !SocialServiceClientManager.Instance.EnsureConnected(SessionService.username)) return;

            SetBusy(true);
            onlineFriends.Clear();
            try
            {
                SocialManagerService.FriendDto[] friends = await socialProxy.getFriendsListAsync(SessionService.username);
                if (friends != null)
                {
                    foreach (var friendDto in friends.Where(f => f.isOnline))
                    {
                        onlineFriends.Add(new FriendDtoDisplay(friendDto));
                    }
                    Console.WriteLine($"Loaded {onlineFriends.Count} online friends for lobby invite list.");
                }
            }
            catch (Exception ex) { HandleError("Error loading online friends", ex); }
            finally { SetBusy(false); }
        }

        private void executeInviteFriend(object parameter)
        {
            if (!(parameter is FriendDtoDisplay friendToInvite)) return;
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) return;

            try
            {
                Console.WriteLine($"Sending invite to {friendToInvite.username} for lobby {lobbyCode}");
                matchmakingProxy.inviteToLobby(SessionService.username, friendToInvite.username, lobbyCode);
                MessageBox.Show($"Invitation sent to {friendToInvite.username}.", "Invitation Sent", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
            }
            catch (Exception ex)
            {
                HandleError("Error sending lobby invite", ex);
            }
        }
        private void subscribeToCallbacks()
        {
            if (matchmakingCallbackHandler != null)
            {
                matchmakingCallbackHandler.LobbyStateUpdated -= handleLobbyStateUpdate; 
                matchmakingCallbackHandler.MatchFound -= handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby -= handleKickedFromLobby;

                matchmakingCallbackHandler.LobbyStateUpdated += handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound += handleMatchFound;
                matchmakingCallbackHandler.KickedFromLobby += handleKickedFromLobby;
                Debug.WriteLine("Subscribed to Matchmaking callbacks.");
            }
            else { Debug.WriteLine("ERROR: Matchmaking callback handler is null."); }
        }

        private void unsubscribeFromCallbacks() 
        {
            if (matchmakingCallbackHandler != null)
            {
                matchmakingCallbackHandler.LobbyStateUpdated -= handleLobbyStateUpdate;
                matchmakingCallbackHandler.MatchFound -= handleMatchFound;           
                matchmakingCallbackHandler.KickedFromLobby -= handleKickedFromLobby; 
                Debug.WriteLine("Unsubscribed from Matchmaking callbacks.");
            }
            unsubscribeFromChatCallbacks();
        }

        private void handleLobbyStateUpdate(LobbyStateDto newState)
        {
            if (newState != null && (string.IsNullOrEmpty(this.lobbyCode) || this.lobbyCode == "Joining..." || newState.lobbyId == this.lobbyCode))
            {
                bool isInitialJoin = string.IsNullOrEmpty(this.lobbyCode) || this.lobbyCode == "Joining...";
                Application.Current.Dispatcher.Invoke(() => updateState(newState));

                if (isInitialJoin && !string.IsNullOrEmpty(newState.lobbyId))
                {
                    Debug.WriteLine($"Initial lobby state received ({newState.lobbyId}). Connecting to chat.");
                    connectToChat(newState.lobbyId);
                }
            }
        }

        private void handleMatchFound(string matchId, List<string> playerUsernames)
        {
            if (playerUsernames != null && playerUsernames.Contains(SessionService.username))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    isBusy = false;
                    MessageBox.Show($"Match found! Starting game (ID: {matchId})...", "Match Start", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
                    unsubscribeFromCallbacks();

                    var gameWindow = new GameWindow();
                    gameWindow.Show();

                    var currentWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
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

            OnPropertyChanged(nameof(isHost));
            OnPropertyChanged(nameof(isGuestUser));
            RaiseCanExecuteChangedOnCommands();
        }

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
                ((RelayCommand)inviteGuestCommand).RaiseCanExecuteChanged();
            });
        }
        private void SetBusy(bool busy)
        {
            isBusy = busy;
        }

        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"!!! {message}: {ex}");
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
        }

        public void cleanup()
        {
            Debug.WriteLine("LobbyViewModel Cleanup called.");
            unsubscribeFromCallbacks();
            disconnectFromChat();
        }
    }
}