using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs; 
using MindWeaveClient.Services;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.View.Dialogs;
using MindWeaveClient.View.Game;
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
 public bool IsGuestUser => SessionService.IsGuest;
 private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.instance.proxy;
 private MatchmakingCallbackHandler matchmakingCallbackHandler => MatchmakingServiceClientManager.instance.callbackHandler;
 private SocialManagerClient socialProxy => SocialServiceClientManager.instance.proxy;
 private ChatManagerClient chatProxy => ChatServiceClientManager.instance.proxy;
 private ChatCallbackHandler chatCallbackHandler => ChatServiceClientManager.instance.callbackHandler;


 private readonly Action<Page> navigateTo;
 private readonly Action navigateBack;

 private string lobbyCodeValue;
 public string LobbyCode { get => lobbyCodeValue; set { lobbyCodeValue = value; OnPropertyChanged(); } }
 private string hostUsernameValue;
 public string HostUsername { get => hostUsernameValue; set { hostUsernameValue = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsHost)); } }
 public ObservableCollection<string> Players { get; } = new ObservableCollection<string>();
 private LobbySettingsDto currentSettingsValue;
 public LobbySettingsDto CurrentSettings { get => currentSettingsValue; set { currentSettingsValue = value; OnPropertyChanged(); } }
 public bool IsHost => hostUsernameValue == SessionService.Username;
 private bool isBusyValue;
 public bool IsBusy { get => isBusyValue; set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } }
 public ObservableCollection<FriendDtoDisplay> OnlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();

 public ObservableCollection<ChatMessageDisplayViewModel> ChatMessages { get; } = new ObservableCollection<ChatMessageDisplayViewModel>();
 private string currentChatMessageValue = string.Empty;
 public string CurrentChatMessage
 {
 get => currentChatMessageValue;
 set { currentChatMessageValue = value; OnPropertyChanged(); ((RelayCommand)SendMessageCommand).RaiseCanExecuteChanged(); }
 }
 public ICommand LeaveLobbyCommand { get; }
 public ICommand StartGameCommand { get; }
 public ICommand InviteFriendCommand { get; }
 public ICommand KickPlayerCommand { get; }
 public ICommand UploadImageCommand { get; }
 public ICommand ChangeSettingsCommand { get; }
 public ICommand RefreshFriendsCommand { get; }
 public ICommand SendMessageCommand { get; }
 public ICommand InviteGuestCommand { get; }

 public LobbyViewModel(LobbyStateDto initialState, Action<Page> navigateToAction, Action navigateBackAction)
 {
 this.navigateTo = navigateToAction;
 this.navigateBack = navigateBackAction;

 LeaveLobbyCommand = new RelayCommand(executeLeaveLobby, param => !IsBusy);
 StartGameCommand = new RelayCommand(executeStartGame, param => IsHost && !IsBusy && !IsGuestUser);
 InviteFriendCommand = new RelayCommand(executeInviteFriend, param => IsHost && !IsBusy && !IsGuestUser && param is FriendDtoDisplay friend && friend.IsOnline && !Players.Contains(friend.Username));
 KickPlayerCommand = new RelayCommand(executeKickPlayer, p => IsHost && !IsBusy && !IsGuestUser && p is string target && target != HostUsername);
 UploadImageCommand = new RelayCommand(p => MessageBox.Show("Upload image TBD"), param => IsHost && !IsBusy && !IsGuestUser);
 ChangeSettingsCommand = new RelayCommand(p => MessageBox.Show("Change settings TBD"), param => IsHost && !IsBusy && !IsGuestUser);
 RefreshFriendsCommand = new RelayCommand(async p => await loadOnlineFriendsAsync(), p => IsHost && !IsBusy && !IsGuestUser);
 SendMessageCommand = new RelayCommand(executeSendMessage, canExecuteSendMessage);
 InviteGuestCommand = new RelayCommand(async p => await executeInviteGuestAsync(), p => IsHost && !IsBusy && !IsGuestUser);

 subscribeToCallbacks();

 if (initialState != null) {
 connectToChat(initialState.lobbyId);
 updateState(initialState);
 }
 else { LobbyCode = "Joining..."; HostUsername = "Loading..."; }

 if (IsHost && !IsGuestUser) { RefreshFriendsCommand.Execute(null); }
 OnPropertyChanged(nameof(IsGuestUser));
 RaiseCanExecuteChangedOnCommands();
 Debug.WriteLine($"LobbyViewModel initialized. Initial State Null: {initialState == null}");

 }

 private async Task executeInviteGuestAsync()
 {
 if (!IsHost || IsGuestUser || IsBusy) return;

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

 if (!MatchmakingServiceClientManager.instance.EnsureConnected())
 {
 MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
 return;
 }

 setBusy(true);

 var invitationData = new GuestInvitationDto
 {
 inviterUsername = SessionService.Username,
 guestEmail = guestEmail.Trim(),
 lobbyCode = this.LobbyCode
 };

 try
 {
 await matchmakingProxy.inviteGuestByEmailAsync(invitationData);
 MessageBox.Show($"Invitation sent to {guestEmail}.", Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
 }
 catch (Exception ex)
 {
 handleError(Lang.ErrorSendingGuestInvitation, ex);
 MatchmakingServiceClientManager.instance.Disconnect();
 }
 finally
 {
 setBusy(false);
 }
 }

 private void connectToChat(string lobbyIdToConnect)
 {
 Debug.WriteLine($"Attempting to connect to chat for lobby {lobbyIdToConnect}...");
 if (ChatServiceClientManager.instance.Connect(SessionService.Username, lobbyIdToConnect))
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
 ChatServiceClientManager.instance.Disconnect();
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
 ChatMessages.Add(new ChatMessageDisplayViewModel(messageDto));
 Debug.WriteLine($"Added message from {messageDto.senderUsername} to UI collection.");
 });
 }


 private bool canExecuteSendMessage(object parameter)
 {
 return !string.IsNullOrWhiteSpace(CurrentChatMessage) && ChatServiceClientManager.instance.IsConnected();
 }

 private void executeSendMessage(object parameter)
 {
 if (!canExecuteSendMessage(parameter)) return;

 string messageToSend = CurrentChatMessage;
 CurrentChatMessage = string.Empty;

 try
 {
 chatProxy.sendLobbyMessageAsync(SessionService.Username, LobbyCode, messageToSend);
 Debug.WriteLine($"Sent message: '{messageToSend}'");
 }
 catch (Exception ex)
 {
 Debug.WriteLine($"Error sending chat message: {ex.Message}");
 MessageBox.Show($"Failed to send message: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);

 if (chatProxy.State == CommunicationState.Faulted)
 {
 disconnectFromChat();
 connectToChat(LobbyCode);
 }
 }
 }




 private void executeLeaveLobby(object parameter)
 {
 if (!MatchmakingServiceClientManager.instance.EnsureConnected()) return;
 IsBusy = true;
 try
 {
 matchmakingProxy.leaveLobby(SessionService.Username, LobbyCode);
 navigateBack?.Invoke();
 }
 catch (Exception ex) { MessageBox.Show($"Error leaving lobby: {ex.Message}", "Error"); }
 finally { IsBusy = false; }
 }

 private async void executeStartGame(object parameter)
 {
 if (!IsHost || Players.Count <4) 
 {
 MessageBox.Show("Need exactly4 players to start.", "Cannot Start Game", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
 return;
 }
 if (!MatchmakingServiceClientManager.instance.EnsureConnected()) return;

 IsBusy = true;
 try
 {
 matchmakingProxy.startGame(SessionService.Username, LobbyCode);
 }
 catch (Exception ex) { MessageBox.Show($"Error starting game: {ex.Message}", "Error"); IsBusy = false; } // TODO: Lang
 }

 private void executeKickPlayer(object parameter)
 {
 if (!(parameter is string playerToKick) || !IsHost || playerToKick == HostUsername) return;
 if (!MatchmakingServiceClientManager.instance.EnsureConnected()) return;

 var confirmResult = MessageBox.Show($"Are you sure you want to kick {playerToKick}?", "Kick Player", MessageBoxButton.YesNo, MessageBoxImage.Warning);
 if (confirmResult != MessageBoxResult.Yes) return;

 try
 {
 matchmakingProxy.kickPlayer(SessionService.Username, playerToKick, LobbyCode);
 }
 catch (Exception ex)
 {
 MessageBox.Show($"Error kicking player: {ex.Message}", "Error"); // TODO: Lang
 }
 }

 private async Task loadOnlineFriendsAsync()
 {
 if (!IsHost || !SocialServiceClientManager.instance.EnsureConnected(SessionService.Username)) return;

 setBusy(true);
 OnlineFriends.Clear();
 try
 {
 SocialManagerService.FriendDto[] friends = await socialProxy.getFriendsListAsync(SessionService.Username);
 if (friends != null)
 {
 foreach (var friendDto in friends.Where(f => f.isOnline))
 {
 OnlineFriends.Add(new FriendDtoDisplay(friendDto));
 }
 Console.WriteLine($"Loaded {OnlineFriends.Count} online friends for lobby invite list.");
 }
 }
 catch (Exception ex) { handleError("Error loading online friends", ex); }
 finally { setBusy(false); }
 }

 private void executeInviteFriend(object parameter)
 {
 if (!(parameter is FriendDtoDisplay friendToInvite)) return;
 if (!MatchmakingServiceClientManager.instance.EnsureConnected()) return;

 try
 {
 Console.WriteLine($"Sending invite to {friendToInvite.Username} for lobby {LobbyCode}");
 matchmakingProxy.inviteToLobby(SessionService.Username, friendToInvite.Username, LobbyCode);
 MessageBox.Show($"Invitation sent to {friendToInvite.Username}.", "Invitation Sent", MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang
 }
 catch (Exception ex)
 {
 handleError("Error sending lobby invite", ex);
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
 if (newState != null && (string.IsNullOrEmpty(this.LobbyCode) || this.LobbyCode == "Joining..." || newState.lobbyId == this.LobbyCode))
 {
 bool isInitialJoin = string.IsNullOrEmpty(this.LobbyCode) || this.LobbyCode == "Joining...";
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
 if (playerUsernames != null && playerUsernames.Contains(SessionService.Username))
 {
 Application.Current.Dispatcher.Invoke(() =>
 {
 IsBusy = false;
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
 LobbyCode = state.lobbyId;
 HostUsername = state.hostUsername;
 CurrentSettings = state.currentSettingsDto;

 var playersToRemove = Players.Except(state.players).ToList();
 var playersToAdd = state.players.Except(Players).ToList();

 foreach (var p in playersToRemove) Players.Remove(p);
 foreach (var p in playersToAdd) Players.Add(p);

 OnPropertyChanged(nameof(IsHost));
 OnPropertyChanged(nameof(IsGuestUser));
 RaiseCanExecuteChangedOnCommands();
 }

 private void RaiseCanExecuteChangedOnCommands()
 {
 Application.Current.Dispatcher?.Invoke(() =>
 {
 ((RelayCommand)LeaveLobbyCommand).RaiseCanExecuteChanged();
 ((RelayCommand)StartGameCommand).RaiseCanExecuteChanged();
 ((RelayCommand)InviteFriendCommand).RaiseCanExecuteChanged();
 ((RelayCommand)KickPlayerCommand).RaiseCanExecuteChanged();
 ((RelayCommand)UploadImageCommand).RaiseCanExecuteChanged();
 ((RelayCommand)ChangeSettingsCommand).RaiseCanExecuteChanged();
 ((RelayCommand)RefreshFriendsCommand).RaiseCanExecuteChanged();
 ((RelayCommand)InviteGuestCommand).RaiseCanExecuteChanged();
 });
 }
 private void setBusy(bool busy)
 {
 IsBusy = busy;
 }

 private void handleError(string message, Exception ex)
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