using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.ChatManagerService;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.SocialManagerService;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Main;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel.Game
{
    public class LobbyViewModel : BaseViewModel, IDisposable
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly ISocialService socialService;
        private readonly IChatService chatService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;

        private LobbyStateDto lobbyState;
        private bool isChatConnected;
        private string currentChatMessage = string.Empty;
        private bool isCleaningUp;
        private bool isDisposed = false;

        public static bool IsGuestUser => SessionService.IsGuest;
        public ImageSource PuzzleImage { get; private set; }
        public string LobbyCode { get; private set; } = "Joining...";
        public string HostUsername { get; private set; } = "Loading...";
        public bool IsHost => lobbyState?.HostUsername == SessionService.Username;
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
            ICurrentLobbyService currentLobbyService)
        {
            this.matchmakingService = matchmakingService;
            this.socialService = socialService;
            this.chatService = chatService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;

            LeaveLobbyCommand = new RelayCommand(async p => await executeLeaveLobby(), p => !IsBusy);
            StartGameCommand = new RelayCommand(async p => await executeStartGame(), p => IsHost && !IsBusy && !IsGuestUser);
            InviteFriendCommand = new RelayCommand(async p => await executeInviteFriend(p), canInviteFriend);
            KickPlayerCommand = new RelayCommand(async p => await executeKickPlayer(p), p => IsHost && !IsBusy && !IsGuestUser && p is string target && target != HostUsername);
            RefreshFriendsCommand = new RelayCommand(async p => await executeRefreshFriendsAsync(), p => IsHost && !IsBusy && !IsGuestUser);
            SendMessageCommand = new RelayCommand(async p => await executeSendMessage(), canExecuteSendMessage);
            InviteGuestCommand = new RelayCommand(async p => await executeInviteGuestAsync(), p => IsHost && !IsBusy && !IsGuestUser);

            subscribeToServiceEvents();

            var initialState = currentLobbyService.getInitialState();
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
        }

        private void subscribeToServiceEvents()
        {
            matchmakingService.OnLobbyStateUpdated += handleLobbyStateUpdated;
            MatchmakingCallbackHandler.OnGameStartedNavigation += handleGameStarted;
            matchmakingService.OnLobbyCreationFailed += handleKickedOrFailed;
            matchmakingService.OnKicked += handleKickedOrFailed;
            chatService.OnMessageReceived += onChatMessageReceived;
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
                    foreach (var friendDto in friends.Where(f => f.IsOnline))
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
        private void handleGameStarted()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                navigationService.navigateTo<GamePage>();
            });
        }
        
        private void handleLobbyStateUpdated(LobbyStateDto newState)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                bool isInitialJoin = string.IsNullOrEmpty(this.LobbyCode) || this.LobbyCode == "Joining...";

                SetBusy(false);

                lobbyState = newState;
                LobbyCode = newState.LobbyId;
                HostUsername = newState.HostUsername;
                CurrentSettings = newState.CurrentSettingsDto;


                updatePuzzleImage(newState);
                var playersToRemove = Players.Except(newState.Players).ToList();
                var playersToAdd = newState.Players.Except(Players).ToList();
                foreach (var p in playersToRemove) Players.Remove(p);
                foreach (var p in playersToAdd) Players.Add(p);

                OnPropertyChanged(nameof(LobbyCode));
                OnPropertyChanged(nameof(HostUsername));
                OnPropertyChanged(nameof(CurrentSettings));
                OnPropertyChanged(nameof(IsHost));
                OnPropertyChanged(nameof(IsGuestUser));

                CommandManager.InvalidateRequerySuggested();

                if (isInitialJoin && !string.IsNullOrEmpty(newState.LobbyId))
                {
                    await connectToChatAsync(newState.LobbyId);
                }
            });
        }

        private void updatePuzzleImage(LobbyStateDto newState)
        {
            if (newState.CurrentSettingsDto?.CustomPuzzleImage != null && newState.CurrentSettingsDto.CustomPuzzleImage.Length > 0)
            {
                PuzzleImage = convertBytesToImageSource(newState.CurrentSettingsDto.CustomPuzzleImage);
            }
            else if (!string.IsNullOrEmpty(newState.PuzzleImagePath))
            {
                string rawPath = newState.PuzzleImagePath;
                string finalPath = rawPath.StartsWith("/") ? rawPath : $"/Resources/Images/Puzzles/{rawPath}";
                try
                {
                    PuzzleImage = new BitmapImage(new Uri(finalPath, UriKind.Relative));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Failed to load pre-loaded puzzle image: " + ex.Message);
                    PuzzleImage = new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative)); // Fallback
                }
            }
            else
            {
                PuzzleImage = new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative));
            }
            OnPropertyChanged(nameof(PuzzleImage));
        }

        private ImageSource convertBytesToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;
            try
            {
                var bitmapImage = new BitmapImage();
                using (var memStream = new MemoryStream(imageBytes))
                {
                    memStream.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.UriSource = null;
                    bitmapImage.StreamSource = memStream;
                    bitmapImage.EndInit();
                }
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to convert byte array to ImageSource: " + ex.Message);
                return new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative)); // Fallback
            }
        }

        private void handleKickedOrFailed(string reason)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                dialogService.showError(string.Format(Lang.KickedMessage, reason), Lang.KickedTitle);

                try
                {
                    await disconnectFromChatAsync();
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning($"Error desconectando chat al ser expulsado: {ex.Message}");
                }

                Dispose();
                matchmakingService.disconnect();

                windowNavigationService.openWindow<MainWindow>();
                windowNavigationService.closeWindow<GameWindow>();

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
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
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
                await cleanup();
            }
            catch (EndpointNotFoundException)
            {
            }
            catch (TimeoutException)
            {
            }
            catch (CommunicationException)
            {
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorLeavingLobby, ex);
            }
            finally
            {
                windowNavigationService.closeWindow<GameWindow>();
                windowNavigationService.openWindow<MainWindow>();

                SetBusy(false);
            }
        }

        private async Task executeStartGame()
        {
            /*
            if (Players.Count != 4) 
            { 
                dialogService.showError(Lang.Need4PlayersError, Lang.ErrorTitle); 
                return;
            }
            */

            SetBusy(true);
            try
            {
                var currentMatchService = App.ServiceProvider.GetService<ICurrentMatchService>();

                Debug.WriteLine($"[LobbyViewModel] Before start - CurrentMatchService.LobbyId: {currentMatchService.LobbyId ?? "NULL"}");
                Debug.WriteLine($"[LobbyViewModel] Before start - This.LobbyCode: {this.LobbyCode ?? "NULL"}");

                if (string.IsNullOrEmpty(currentMatchService.LobbyId) && !string.IsNullOrEmpty(this.LobbyCode))
                {
                    Debug.WriteLine($"[LobbyViewModel] CurrentMatchService LobbyId is NULL, setting it to: {this.LobbyCode}");
                    currentMatchService.initializeMatch(
                        this.LobbyCode,
                        this.Players.ToList(),
                        this.CurrentSettings,
                        lobbyState?.PuzzleImagePath
                    );
                }
                else
                {
                    Debug.WriteLine($"[LobbyViewModel] CurrentMatchService already has LobbyId: {currentMatchService.LobbyId}");
                }

                await matchmakingService.startGameAsync(SessionService.Username, LobbyCode);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.GameStartError, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeKickPlayer(object parameter)
        {
            if (!(parameter is string playerToKick)) return;

            var confirm = dialogService.showConfirmation(
                string.Format(Lang.KickPlayerMessage, playerToKick),
                Lang.KickPlayerTitle
            );

            if (!confirm) return;
            SetBusy(true);

            try
            {
                await matchmakingService.kickPlayerAsync(SessionService.Username, playerToKick, LobbyCode);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorKickingPlayer, ex);
            }
            finally
            {
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
                    string.Format(Lang.InviteSentBody, friendToInvite.Username),
                    Lang.InviteSentTitle);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.SendInviteError, ex);
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
                    InviterUsername = SessionService.Username,
                    GuestEmail = guestEmail.Trim(),
                    LobbyCode = this.LobbyCode
                };

                try
                {
                    await matchmakingService.inviteGuestByEmailAsync(invitationData);
                    dialogService.showInfo(
                        string.Format(Lang.InviteSentBody, guestEmail),
                        Lang.InviteSentTitle);
                }
                catch (EndpointNotFoundException ex)
                {
                    handleError(Lang.ErrorMsgServerOffline, ex);
                }
                catch (TimeoutException ex)
                {
                    handleError(Lang.ErrorMsgServerOffline, ex);
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

        private bool canExecuteSendMessage(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CurrentChatMessage) && IsChatConnected;
        }

        private async Task executeSendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentChatMessage)) return;

            string messageToSend = CurrentChatMessage;
            CurrentChatMessage = string.Empty;

            try
            {
                await chatService.sendLobbyMessageAsync(SessionService.Username, LobbyCode, messageToSend);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                await attemptChatReconnect();
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                await attemptChatReconnect();
            }
            catch (InvalidOperationException ex)
            {
                handleError(Lang.ChatConnectError, ex);
                await attemptChatReconnect();
            }
            catch (Exception ex)
            {
                handleError(Lang.SendChatError, ex);
                await attemptChatReconnect();
            }

        }

        private async Task attemptChatReconnect()
        {
            try
            {
                await disconnectFromChatAsync();
                await connectToChatAsync(LobbyCode);
            }
            catch (CommunicationException)
            {

            }
            catch (TimeoutException)
            {

            }
        }

        public async Task cleanup()
        {
            if (isCleaningUp) return;
            isCleaningUp = true;

            if (!string.IsNullOrEmpty(LobbyCode) && LobbyCode != "Joining...")
            {
                try
                {
                    await matchmakingService.leaveLobbyAsync(SessionService.Username, LobbyCode);
                }
                catch (EndpointNotFoundException)
                {

                }
                catch (TimeoutException)
                {

                }
                catch (CommunicationException)
                {

                }

                try
                {
                    await disconnectFromChatAsync();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error disconnecting chat: {ex.Message}");
                }
            }
            matchmakingService.OnLobbyStateUpdated -= handleLobbyStateUpdated;
            matchmakingService.OnLobbyCreationFailed -= handleKickedOrFailed;
            matchmakingService.OnKicked -= handleKickedOrFailed;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                chatService.OnMessageReceived -= onChatMessageReceived;
                Players.Clear();
                OnlineFriends.Clear();
                ChatMessages.Clear();
            }

            isDisposed = true;
        }


        private void handleError(string message, Exception ex)
        {
            dialogService.showError($"{message}: {ex.Message}", Lang.ErrorTitle);
        }
    }
}