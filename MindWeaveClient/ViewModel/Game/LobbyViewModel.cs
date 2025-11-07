using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Services.Implementations;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
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
using System.Windows.Controls;
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

        public bool IsGuestUser => SessionService.IsGuest;
        private LobbyStateDto lobbyState;

        public string LobbyCode { get; private set; } = "Joining...";
        public string HostUsername { get; private set; } = "Loading...";
        public bool IsHost => lobbyState?.hostUsername == SessionService.Username;
        public LobbySettingsDto CurrentSettings { get; private set; }

        public ObservableCollection<string> Players { get; } = new ObservableCollection<string>();
        public ObservableCollection<FriendDtoDisplay> OnlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<ChatMessageDisplayViewModel> ChatMessages { get; } = new ObservableCollection<ChatMessageDisplayViewModel>();

        private string currentChatMessage = string.Empty;
        public string CurrentChatMessage
        {
            get => currentChatMessage;
            set { currentChatMessage = value; OnPropertyChanged(); ((RelayCommand)SendMessageCommand).RaiseCanExecuteChanged(); }
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
            this.currentMatchService = currentMatchService;

            LeaveLobbyCommand = new RelayCommand(executeLeaveLobby, param => !IsBusy);
            StartGameCommand = new RelayCommand(executeStartGame, param => IsHost && !IsBusy && !IsGuestUser);
            InviteFriendCommand = new RelayCommand(executeInviteFriend, canInviteFriend);
            KickPlayerCommand = new RelayCommand(executeKickPlayer, p => IsHost && !IsBusy && !IsGuestUser && p is string target && target != HostUsername);
            UploadImageCommand = new RelayCommand(p => dialogService.showInfo("La carga de imágenes personalizadas aún no está implementada.", "Info"), param => IsHost && !IsBusy && !IsGuestUser);
            ChangeSettingsCommand = new RelayCommand(p => dialogService.showInfo("El cambio de ajustes de partida aún no está implementado.", "Info"), param => IsHost && !IsBusy && !IsGuestUser);
            RefreshFriendsCommand = new RelayCommand(async p => await executeRefreshFriendsAsync(), p => IsHost && !IsBusy && !IsGuestUser);
            SendMessageCommand = new RelayCommand(executeSendMessage, canExecuteSendMessage);
            InviteGuestCommand = new RelayCommand(async p => await executeInviteGuestAsync(), p => IsHost && !IsBusy && !IsGuestUser);

            subscribeToAggregator();

            var initialState = currentLobbyService.getInitialState();
            if (initialState != null)
            {
                onLobbyStateUpdated(initialState);
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

        private void subscribeToAggregator()
        {
            GameEventAggregator.OnLobbyStateUpdated += onLobbyStateUpdated;
            GameEventAggregator.OnMatchFound += onMatchFound;
            GameEventAggregator.OnLobbyJoinFailed += onKickedOrFailed;
            GameEventAggregator.OnChatMessageReceived += onChatMessageReceived;
        }

        private void unsubscribeFromAggregator()
        {
            GameEventAggregator.OnLobbyStateUpdated -= onLobbyStateUpdated;
            GameEventAggregator.OnMatchFound -= onMatchFound;
            GameEventAggregator.OnLobbyJoinFailed -= onKickedOrFailed;
            GameEventAggregator.OnChatMessageReceived -= onChatMessageReceived;
        }

        private void onLobbyStateUpdated(LobbyStateDto newState)
        {
            bool isInitialJoin = string.IsNullOrEmpty(this.LobbyCode) || this.LobbyCode == "Joining...";

            SetBusy(false);

            this.lobbyState = newState;
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

            if (isInitialJoin && !string.IsNullOrEmpty(newState.lobbyId))
            {
                connectToChat(newState.lobbyId);
            }
        }

        private void onMatchFound(string matchId, List<string> playerUsernames)
        {
            if (playerUsernames != null && playerUsernames.Contains(SessionService.Username))
            {
                SetBusy(false);
                cleanupAndUnsubscribe();
                currentMatchService.setMatchId(matchId);

                navigationService.navigateTo<GamePage>();
            }
        }

        private void onKickedOrFailed(string reason)
        {
            dialogService.showError(string.Format(Lang.KickedMessage, reason), Lang.KickedTitle);
            cleanupAndUnsubscribe();
            windowNavigationService.openWindow<MainWindow>();
            windowNavigationService.closeWindowFromContext(this);
        }

        private void onChatMessageReceived(ChatMessageDto messageDto)
        {
            ChatMessages.Add(new ChatMessageDisplayViewModel(messageDto));
        }

        private async void executeLeaveLobby(object parameter)
        {
            SetBusy(true);
            try
            {
                await matchmakingService.leaveLobbyAsync(SessionService.Username, LobbyCode);
                cleanupAndUnsubscribe();
                windowNavigationService.openWindow<MainWindow>();
                windowNavigationService.closeWindowFromContext(this);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorLeavingLobby, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void executeStartGame(object parameter)
        {
            if (Players.Count != 4)
            {
                dialogService.showError(Lang.Need4PlayersError, Lang.ErrorTitle);
                return;
            }

            SetBusy(true);
            try
            {
                await matchmakingService.startGameAsync(SessionService.Username, LobbyCode);
            }
            catch (Exception ex)
            {
                handleError(Lang.GameStartError, ex);
                SetBusy(false);
            }
        }

        private async void executeKickPlayer(object parameter)
        {
            if (!(parameter is string playerToKick)) return;

            var confirm = dialogService.showConfirmation(
                string.Format(Lang.KickPlayerMessage, playerToKick),
                Lang.KickPlayerTitle
            );

            if (!confirm) return;

            try
            {
                await matchmakingService.kickPlayerAsync(SessionService.Username, playerToKick, LobbyCode);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorKickingPlayer, ex);
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

        private async void executeInviteFriend(object parameter)
        {
            if (!(parameter is FriendDtoDisplay friendToInvite)) return;

            try
            {
                await matchmakingService.inviteToLobbyAsync(SessionService.Username, friendToInvite.Username, LobbyCode);
                dialogService.showInfo(string.Format(Lang.InviteSentBody, friendToInvite.Username), Lang.InviteSentTitle);
            }
            catch (Exception ex)
            {
                handleError(Lang.SendInviteError, ex);
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
                handleError(Lang.LoadFriendsError, ex);
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
                    dialogService.showError(Lang.GlobalErrorInvalidEmailFormat, Lang.ErrorTitle);
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
                    dialogService.showInfo(string.Format(Lang.InviteSentBody, guestEmail), Lang.InviteSentTitle);
                }
                catch (Exception ex)
                {
                    handleError(Lang.GuestInviteError, ex);
                }
                finally
                {
                    SetBusy(false);
                }
            }
        }

        private void connectToChat(string lobbyIdToConnect)
        {
            if (!chatService.connect(SessionService.Username, lobbyIdToConnect))
            {
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
            }
        }

        private void disconnectFromChat()
        {
            chatService.disconnect();
        }

        private bool canExecuteSendMessage(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CurrentChatMessage) && chatService.isConnected();
        }

        private async void executeSendMessage(object parameter)
        {
            if (!canExecuteSendMessage(parameter)) return;

            string messageToSend = CurrentChatMessage;
            CurrentChatMessage = string.Empty;

            try
            {
                await chatService.sendLobbyMessageAsync(SessionService.Username, LobbyCode, messageToSend);
            }
            catch (Exception ex)
            {
                dialogService.showError(string.Format(Lang.SendChatError, ex.Message), Lang.ErrorTitle);

                disconnectFromChat();
                connectToChat(LobbyCode);
            }
        }

        private void cleanupAndUnsubscribe()
        {
            unsubscribeFromAggregator();
            disconnectFromChat();
        }

        private void handleError(string message, Exception ex)
        {
            dialogService.showError($"{message}: {ex.Message}", Lang.ErrorTitle);
        }

    }
}