using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Game
{
    public class LobbyViewModel : BaseViewModel
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly ISocialService socialService;
        private readonly IChatService chatService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentMatchService currentMatchService;
        private readonly ICurrentLobbyService currentLobbyService;

        private LobbyStateDto lobbyState;
        private bool isChatConnected;
        private string currentChatMessage = string.Empty;
        private bool isCleaningUp = false;

        public bool IsGuestUser => SessionService.IsGuest;
        public string LobbyCode { get; private set; } = "Joining...";
        public string HostUsername { get; private set; } = "Loading...";
        public bool IsHost => lobbyState?.hostUsername == SessionService.Username;
        public LobbySettingsDto CurrentSettings { get; private set; }
        public ObservableCollection<string> Players { get; } = new ObservableCollection<string>();
        public ObservableCollection<FriendDtoDisplay> OnlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<ChatMessageDisplayViewModel> ChatMessages { get; } = new ObservableCollection<ChatMessageDisplayViewModel>();

        public string CurrentChatMessage
        {
            get => currentChatMessage;
            set { currentChatMessage = value; OnPropertyChanged(); }
        }

        public bool IsChatConnected
        {
            get => isChatConnected;
            set { isChatConnected = value; OnPropertyChanged(); }
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

        public LobbyViewModel(
            IMatchmakingService matchmakingService,
            ISocialService socialService,
            IChatService chatService,
            IDialogService dialogService,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ICurrentLobbyService currentLobbyService,
            ICurrentMatchService currentMatchService)
        {
            this.matchmakingService = matchmakingService;
            this.socialService = socialService;
            this.chatService = chatService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.currentLobbyService = currentLobbyService;
            this.currentMatchService = currentMatchService;

            LeaveLobbyCommand = new RelayCommand(async p => await executeLeaveLobby(), p => !IsBusy);
            StartGameCommand = new RelayCommand(async p => await executeStartGame(), p => IsHost && !IsBusy && !IsGuestUser);
            InviteFriendCommand = new RelayCommand(async p => await executeInviteFriend(p), canInviteFriend);
            KickPlayerCommand = new RelayCommand(async p => await executeKickPlayer(p), p => IsHost && !IsBusy && !IsGuestUser && p is string target && target != HostUsername);
            UploadImageCommand = new RelayCommand(p => dialogService.showInfo("La carga de imágenes personalizadas aún no está implementada.", "Info"), p => IsHost && !IsBusy && !IsGuestUser);
            ChangeSettingsCommand = new RelayCommand(p => dialogService.showInfo("El cambio de ajustes de partida aún no está implementado.", "Info"), p => IsHost && !IsBusy && !IsGuestUser);
            RefreshFriendsCommand = new RelayCommand(async p => await executeRefreshFriendsAsync(), p => IsHost && !IsBusy && !IsGuestUser);
            SendMessageCommand = new RelayCommand(async p => await executeSendMessage(), canExecuteSendMessage);
            InviteGuestCommand = new RelayCommand(async p => await executeInviteGuestAsync(), p => IsHost && !IsBusy && !IsGuestUser);

            subscribeToServiceEvents();

            var initialState = this.currentLobbyService.getInitialState();
            if (initialState != null)
            {
                handleLobbyStateUpdated(initialState);
            }
            else
            {
                SetBusy(true);
            }

            if (IsHost && !IsGuestUser)
            {
                RefreshFriendsCommand.Execute(null);
            }

            OnPropertyChanged(nameof(IsGuestUser));
        }

        private void subscribeToServiceEvents()
        {
            matchmakingService.OnLobbyStateUpdated += handleLobbyStateUpdated;
            matchmakingService.OnMatchFound += handleMatchFound;
            matchmakingService.OnLobbyCreationFailed += handleKickedOrFailed;
            matchmakingService.OnKicked += handleKickedOrFailed;
            chatService.OnMessageReceived += onChatMessageReceived;
        }

        private async Task cleanupAndUnsubscribeAsync()
        {
            if (isCleaningUp) return;
            isCleaningUp = true;

            try
            {
                matchmakingService.OnLobbyStateUpdated -= handleLobbyStateUpdated;
                matchmakingService.OnMatchFound -= handleMatchFound;
                matchmakingService.OnLobbyCreationFailed -= handleKickedOrFailed;
                matchmakingService.OnKicked -= handleKickedOrFailed;
                chatService.OnMessageReceived -= onChatMessageReceived;

                await disconnectFromChatAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error during cleanup: {ex.Message}");
            }
        }

        private void handleLobbyStateUpdated(LobbyStateDto newState)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                bool isInitialJoin = string.IsNullOrEmpty(this.LobbyCode) || this.LobbyCode == "Joining...";

                SetBusy(false);

                lobbyState = newState;
                LobbyCode = newState.lobbyId;
                HostUsername = newState.hostUsername;
                CurrentSettings = newState.currentSettingsDto;

                var playersToRemove = Players.Except(newState.players).ToList();
                var playersToAdd = newState.players.Except(Players).ToList();
                foreach (var p in playersToRemove) Players.Remove(p);
                foreach (var p in playersToAdd) Players.Add(p);

                OnPropertyChanged(nameof(LobbyCode));
                OnPropertyChanged(nameof(HostUsername));
                OnPropertyChanged(nameof(CurrentSettings));
                OnPropertyChanged(nameof(IsHost));
                OnPropertyChanged(nameof(IsGuestUser));

                CommandManager.InvalidateRequerySuggested();

                if (isInitialJoin && !string.IsNullOrEmpty(newState.lobbyId))
                {
                    await connectToChatAsync(newState.lobbyId);
                }
            });
        }

        private void handleMatchFound(string matchId, List<string> playerUsernames)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (playerUsernames != null && playerUsernames.Contains(SessionService.Username))
                {
                    SetBusy(false);
                    await cleanupAndUnsubscribeAsync();
                    currentMatchService.setMatchId(matchId);
                    navigationService.navigateTo<GamePage>();
                }
            });
        }

        private void handleKickedOrFailed(string reason)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                dialogService.showError(string.Format(Lang.KickedMessage ?? "You were kicked: {0}", reason), Lang.KickedTitle ?? "Kicked");

                await cleanupAndUnsubscribeAsync();
                matchmakingService.disconnect();

                // Cerrar GameWindow y volver a MainWindow
                var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
                if (gameWindow != null)
                {
                    windowNavigationService.closeWindowFromContext(gameWindow);
                }

                windowNavigationService.openWindow<MainWindow>();
            });
        }

        private void onChatMessageReceived(ChatMessageDto messageDto)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatMessages.Add(new ChatMessageDisplayViewModel(messageDto));
            });
        }

        private async Task connectToChatAsync(string lobbyIdToConnect)
        {
            try
            {
                await chatService.connectAsync(SessionService.Username, lobbyIdToConnect);
                IsChatConnected = true;
            }
            catch (Exception)
            {
                dialogService.showError(Lang.ChatConnectError ?? "Failed to connect to chat", Lang.ErrorTitle);
                IsChatConnected = false;
            }
        }

        private async Task disconnectFromChatAsync()
        {
            if (IsChatConnected)
            {
                try
                {
                    await chatService.disconnectAsync(SessionService.Username, LobbyCode);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Failed to disconnect chat cleanly: {ex.Message}");
                }
                finally
                {
                    IsChatConnected = false;
                }
            }
        }

        private async Task executeLeaveLobby()
        {
            SetBusy(true);
            try
            {
                await matchmakingService.leaveLobbyAsync(SessionService.Username, LobbyCode);
                await cleanupAndUnsubscribeAsync();

                // Cerrar GameWindow y abrir MainWindow
                var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
                if (gameWindow != null)
                {
                    windowNavigationService.closeWindowFromContext(gameWindow);
                }

                windowNavigationService.openWindow<MainWindow>();
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorLeavingLobby ?? "Error leaving lobby", ex);
                SetBusy(false);
            }
        }

        private async Task executeStartGame()
        {
            if (Players.Count != 4)
            {
                dialogService.showError(Lang.Need4PlayersError ?? "Need 4 players", Lang.ErrorTitle);
                return;
            }

            SetBusy(true);
            try
            {
                await matchmakingService.startGameAsync(SessionService.Username, LobbyCode);
            }
            catch (Exception ex)
            {
                handleError(Lang.GameStartError ?? "Error starting game", ex);
                SetBusy(false);
            }
        }

        private async Task executeKickPlayer(object parameter)
        {
            if (!(parameter is string playerToKick)) return;

            var confirm = dialogService.showConfirmation(
                string.Format(Lang.KickPlayerMessage ?? "Kick {0}?", playerToKick),
                Lang.KickPlayerTitle ?? "Kick Player"
            );

            if (!confirm) return;
            SetBusy(true);

            try
            {
                await matchmakingService.kickPlayerAsync(SessionService.Username, playerToKick, LobbyCode);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorKickingPlayer ?? "Error kicking player", ex);
                SetBusy(false);
            }
        }

        private bool canInviteFriend(object parameter)
        {
            if (parameter is FriendDtoDisplay friend)
            {
                return IsHost && !IsBusy && !IsGuestUser &&
                       friend.IsOnline && !Players.Contains(friend.Username);
            }
            return false;
        }

        private async Task executeInviteFriend(object parameter)
        {
            if (!(parameter is FriendDtoDisplay friendToInvite)) return;

            SetBusy(true);
            try
            {
                await matchmakingService.inviteToLobbyAsync(SessionService.Username, friendToInvite.Username, LobbyCode);
                dialogService.showInfo(
                    string.Format(Lang.InviteSentBody ?? "Invite sent to {0}", friendToInvite.Username),
                    Lang.InviteSentTitle ?? "Invite Sent");
            }
            catch (Exception ex)
            {
                handleError(Lang.SendInviteError ?? "Error sending invite", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeRefreshFriendsAsync()
        {
            if (!IsHost || IsGuestUser) return;

            SetBusy(true);
            try
            {
                OnlineFriends.Clear();
                FriendDto[] friends = await socialService.getFriendsListAsync(SessionService.Username);

                if (friends != null)
                {
                    foreach (var friendDto in friends.Where(f => f.isOnline))
                    {
                        OnlineFriends.Add(new FriendDtoDisplay(friendDto));
                    }
                }
            }
            catch (Exception ex)
            {
                handleError(Lang.LoadFriendsError ?? "Error loading friends", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeInviteGuestAsync()
        {
            if (dialogService.showGuestInputDialog(out string guestEmail))
            {
                if (string.IsNullOrWhiteSpace(guestEmail) || !Regex.IsMatch(guestEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    dialogService.showError(Lang.GlobalErrorInvalidEmailFormat ?? "Invalid email", Lang.ErrorTitle);
                    return;
                }

                SetBusy(true);
                var invitationData = new GuestInvitationDto
                {
                    inviterUsername = SessionService.Username,
                    guestEmail = guestEmail.Trim(),
                    lobbyCode = this.LobbyCode
                };

                try
                {
                    await matchmakingService.inviteGuestByEmailAsync(invitationData);
                    dialogService.showInfo(
                        string.Format(Lang.InviteSentBody ?? "Invite sent to {0}", guestEmail),
                        Lang.InviteSentTitle ?? "Invite Sent");
                }
                catch (Exception ex)
                {
                    handleError(Lang.GuestInviteError ?? "Error inviting guest", ex);
                }
                finally
                {
                    SetBusy(false);
                }
            }
        }

        private bool canExecuteSendMessage(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CurrentChatMessage) && IsChatConnected;
        }

        private async Task executeSendMessage()
        {
            string messageToSend = CurrentChatMessage;
            CurrentChatMessage = string.Empty;

            try
            {
                await chatService.sendLobbyMessageAsync(SessionService.Username, LobbyCode, messageToSend);
            }
            catch (Exception ex)
            {
                dialogService.showError(
                    string.Format(Lang.SendChatError ?? "Error sending message: {0}", ex.Message),
                    Lang.ErrorTitle);

                await disconnectFromChatAsync();
                await connectToChatAsync(LobbyCode);
            }
        }

        private void handleError(string message, Exception ex)
        {
            dialogService.showError($"{message}: {ex.Message}", Lang.ErrorTitle);
        }

        // Método público para cleanup cuando la página/ventana se cierra
        public async void cleanup()
        {
            if (isCleaningUp) return;

            Trace.TraceInformation("LobbyViewModel cleanup called");

            // Si aún estamos en el lobby, salir apropiadamente
            if (!string.IsNullOrEmpty(LobbyCode) && LobbyCode != "Joining...")
            {
                try
                {
                    await matchmakingService.leaveLobbyAsync(SessionService.Username, LobbyCode);
                    Trace.TraceInformation($"Left lobby {LobbyCode} during cleanup");
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Error leaving lobby during cleanup: {ex.Message}");
                }
            }

            await cleanupAndUnsubscribeAsync();
        }
    }
}