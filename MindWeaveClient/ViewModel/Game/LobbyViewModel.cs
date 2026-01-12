using Microsoft.Extensions.DependencyInjection;
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
using MindWeaveClient.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.View.Authentication;

namespace MindWeaveClient.ViewModel.Game
{
    public class LobbyViewModel : BaseViewModel, IDisposable
    {
        private const int MAX_CHAT_MESSAGE_LENGTH = 200;
        private const int MAX_EMAIL_LENGTH = 45;

        private const string LOBBY_CODE_JOINING = "Joining...";
        private const string HOST_USERNAME_LOADING = "Loading...";

        private const string PUZZLE_IMAGE_BASE_PATH = "/Resources/Images/Puzzles/";
        private const string DEFAULT_PUZZLE_IMAGE_PATH = "/Resources/Images/Puzzles/puzzleDefault.png";

        private const string SYSTEM_MSG_PREFIX_WARN_STRIKE = "WARN_STRIKE:";
        private const string SYSTEM_MSG_LOBBY_CLOSED_HOST_BAN = "LOBBY_CLOSED_HOST_BAN";
        private const string SYSTEM_MSG_SENDER = "SYSTEM";

        private const string EMAIL_REGEX_PATTERN = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        private const char PATH_SEPARATOR = '/';
        private const char STRIKE_SEPARATOR = ':';

        private readonly IMatchmakingService matchmakingService;
        private readonly ISocialService socialService;
        private readonly IChatService chatService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly IServiceExceptionHandler exceptionHandler;

        private LobbyStateDto lobbyState;
        private bool isChatConnected;
        private string currentChatMessage = string.Empty;
        private bool isCleaningUp;
        private bool isDisposed;

        public static bool IsGuestUser => SessionService.IsGuest;
        public ImageSource PuzzleImage { get; private set; }
        public string LobbyCode { get; private set; } = LOBBY_CODE_JOINING;
        public string HostUsername { get; private set; } = HOST_USERNAME_LOADING;
        public bool IsHost => lobbyState?.HostUsername == SessionService.Username;
        public LobbySettingsDto CurrentSettings { get; private set; }

        public ObservableCollection<string> Players { get; } = new ObservableCollection<string>();
        public ObservableCollection<FriendDtoDisplay> OnlineFriends { get; } = new ObservableCollection<FriendDtoDisplay>();
        public ObservableCollection<ChatMessageDisplayViewModel> ChatMessages { get; } = new ObservableCollection<ChatMessageDisplayViewModel>();

        public string CurrentChatMessage
        {
            get => currentChatMessage;
            set
            {
                string processedValue = clampString(value, MAX_CHAT_MESSAGE_LENGTH);

                if (currentChatMessage != processedValue)
                {
                    currentChatMessage = processedValue;
                    OnPropertyChanged();
                }
            }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "Dependencies are injected via DI container - this is standard practice for ViewModels")]
        public LobbyViewModel(
            IMatchmakingService matchmakingService,
            ISocialService socialService,
            IChatService chatService,
            IDialogService dialogService,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ICurrentLobbyService currentLobbyService,
            IServiceExceptionHandler exceptionHandler)
        {
            this.matchmakingService = matchmakingService;
            this.socialService = socialService;
            this.chatService = chatService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.exceptionHandler = exceptionHandler;

            LeaveLobbyCommand = new RelayCommand(async p => await executeLeaveLobbyAsync(), p => !IsBusy);
            StartGameCommand = new RelayCommand(async p => await executeStartGameAsync(), p => IsHost && !IsBusy && !IsGuestUser);
            InviteFriendCommand = new RelayCommand(async p => await executeInviteFriendAsync(p), canInviteFriend);
            KickPlayerCommand = new RelayCommand(async p => await executeKickPlayerAsync(p), canKickPlayer);
            RefreshFriendsCommand = new RelayCommand(async p => await executeRefreshFriendsAsync(), p => IsHost && !IsBusy && !IsGuestUser);
            SendMessageCommand = new RelayCommand(async p => await executeSendMessageAsync(), canExecuteSendMessage);
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

        private static string clampString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private void subscribeToServiceEvents()
        {
            matchmakingService.OnLobbyStateUpdated += handleLobbyStateUpdated;
            matchmakingService.OnGameStarted += handleGameStarted;
            matchmakingService.OnLobbyCreationFailed += handleFatalError;
            matchmakingService.OnKicked += handleKicked;
            matchmakingService.OnLobbyActionFailed += handleActionFailed;
            matchmakingService.OnLobbyDestroyed += handleLobbyDestroyed;
            chatService.OnMessageReceived += onChatMessageReceived;
            chatService.OnSystemMessageReceived += handleSystemMessage;
        }

        private void unsubscribeFromServiceEvents()
        {
            matchmakingService.OnLobbyStateUpdated -= handleLobbyStateUpdated;
            matchmakingService.OnGameStarted -= handleGameStarted;
            matchmakingService.OnLobbyCreationFailed -= handleFatalError;
            matchmakingService.OnKicked -= handleKicked;
            matchmakingService.OnLobbyActionFailed -= handleActionFailed;
            matchmakingService.OnLobbyDestroyed -= handleLobbyDestroyed;
            chatService.OnMessageReceived -= onChatMessageReceived;
            chatService.OnSystemMessageReceived -= handleSystemMessage;
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
                exceptionHandler.handleException(ex, Lang.LoadFriendsOperation);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeLeaveLobbyAsync()
        {
            SetBusy(true);

            try
            {
                await cleanupAsync();
            }
            catch (EndpointNotFoundException)
            {
                // ignored
            }
            catch (CommunicationException)
            {
                // ignored
            }
            catch (TimeoutException)
            {
                // ignored
            }
            catch (SocketException)
            {
                // ignored
            }
            finally
            {
                var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
                if (gameWindow != null)
                {
                    gameWindow.IsExitConfirmed = true;
                }

                if (IsGuestUser)
                {
                    SessionService.clearSession();
                    windowNavigationService.openWindow<AuthenticationWindow>();
                }
                else
                {
                    windowNavigationService.openWindow<MainWindow>();
                }

                windowNavigationService.closeWindow<GameWindow>();

                SetBusy(false);
            }
        }

        private async Task executeStartGameAsync()
        {
            SetBusy(true);

            try
            {
                var currentMatchService = App.ServiceProvider.GetService<ICurrentMatchService>();

                if (string.IsNullOrEmpty(currentMatchService.LobbyId) && !string.IsNullOrEmpty(LobbyCode))
                {
                    currentMatchService.initializeMatch(
                        LobbyCode,
                        Players.ToList(),
                        CurrentSettings,
                        lobbyState?.PuzzleImagePath);
                }

                await matchmakingService.startGameAsync(SessionService.Username, LobbyCode);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.StartGameOperation);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private bool canKickPlayer(object parameter)
        {
            return IsHost && !IsBusy && !IsGuestUser &&
                   parameter is string target && target != HostUsername;
        }

        private async Task executeKickPlayerAsync(object parameter)
        {
            if (!(parameter is string playerToKick)) return;

            var confirm = dialogService.showConfirmation(
                string.Format(Lang.KickPlayerMessage, playerToKick),
                Lang.KickPlayerTitle);

            if (!confirm) return;

            SetBusy(true);

            try
            {
                await matchmakingService.kickPlayerAsync(SessionService.Username, playerToKick, LobbyCode);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.KickPlayerOperation);
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

        private async Task executeInviteFriendAsync(object parameter)
        {
            if (!(parameter is FriendDtoDisplay friendToInvite)) return;

            SetBusy(true);

            try
            {
                await matchmakingService.inviteToLobbyAsync(SessionService.Username, friendToInvite.Username, LobbyCode);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.InviteFriendOperation);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeInviteGuestAsync()
        {
            if (!dialogService.showGuestInputDialog(out string guestEmail)) return;

            string processedEmail = clampString(guestEmail?.Trim(), MAX_EMAIL_LENGTH);

            if (string.IsNullOrWhiteSpace(guestEmail) || !Regex.IsMatch(guestEmail, EMAIL_REGEX_PATTERN))
            {
                dialogService.showError(Lang.GlobalErrorInvalidEmailFormat, Lang.ErrorTitle);
                return;
            }

            SetBusy(true);

            var invitationData = new GuestInvitationDto
            {
                InviterUsername = SessionService.Username,
                GuestEmail = processedEmail,
                LobbyCode = LobbyCode
            };

            try
            {
                await matchmakingService.inviteGuestByEmailAsync(invitationData);
                dialogService.showInfo(
                    string.Format(Lang.InviteSentBody, guestEmail),
                    Lang.InviteSentTitle);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.InviteGuestOperation);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private bool canExecuteSendMessage(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CurrentChatMessage) && IsChatConnected;
        }

        private async Task executeSendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentChatMessage)) return;

            string messageToSend = CurrentChatMessage;
            CurrentChatMessage = string.Empty;

            try
            {
                await chatService.sendLobbyMessageAsync(SessionService.Username, LobbyCode, messageToSend);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.SendMessageOperation);
                await attemptChatReconnectAsync();
            }
        }

        private void handleActionFailed(string messageCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string localizedMessage = MessageCodeInterpreter.translate(messageCode, messageCode);
                dialogService.showWarning(localizedMessage, Lang.WarningTitle);
            });
        }

        private void handleGameStarted(PuzzleDefinitionDto puzzleDefinition, int matchDurationSeconds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                navigationService.navigateTo<GamePage>();
            });
        }

        private void handleFatalError(string reasonCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string localizedReason = MessageCodeInterpreter.translate(reasonCode, reasonCode);
                dialogService.showError(localizedReason, Lang.ErrorTitle);
                _ = forceExitLobbyAsync();
            });
        }

        private void handleKicked(string reasonCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string localizedReason = MessageCodeInterpreter.translate(reasonCode, reasonCode);
                dialogService.showInfo(localizedReason, Lang.KickedTitle);

                _ = handleKickedExitAsync();
            });
        }

        private void handleLobbyStateUpdated(LobbyStateDto newState)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                bool isInitialJoin = string.IsNullOrEmpty(LobbyCode) || LobbyCode == LOBBY_CODE_JOINING;

                SetBusy(false);

                lobbyState = newState;
                LobbyCode = newState.LobbyId;
                HostUsername = newState.HostUsername;
                CurrentSettings = newState.CurrentSettingsDto;

                updatePuzzleImage(newState);
                updatePlayersList(newState);
                initializeMatchService(newState);

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

        private void handleLobbyDestroyed(string reasonCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string localizedReason = MessageCodeInterpreter.translate(reasonCode, reasonCode);
                dialogService.showInfo(localizedReason, Lang.LobbyClosedTitle);

                var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
                if (gameWindow != null)
                {
                    gameWindow.IsExitConfirmed = true;
                }

                if (IsGuestUser)
                {
                    SessionService.clearSession();
                    windowNavigationService.openWindow<AuthenticationWindow>();
                }
                else
                {
                    windowNavigationService.openWindow<MainWindow>();
                }

                windowNavigationService.closeWindow<GameWindow>();

                Dispose();
            });
        }

        private void handleSystemMessage(string messageCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                
                string finalMessage = messageCode;

                if (messageCode.StartsWith(SYSTEM_MSG_PREFIX_WARN_STRIKE))
                {
                    string strikeCount = messageCode.Split(STRIKE_SEPARATOR)[1];
                    finalMessage = string.Format(Lang.SystemMessageBlocked, strikeCount);
                }
                else if (messageCode == SYSTEM_MSG_LOBBY_CLOSED_HOST_BAN)
                {
                    finalMessage = Lang.SystemHostExpelled;
                }
                else
                {
                    string translated = MessageCodeInterpreter.translate(messageCode, null);
                    if (translated != Lang.ErrorGeneric) 
                    {
                        finalMessage = translated;
                    }
                }

                var sysMsg = new ChatMessageDto
                {
                    SenderUsername = SYSTEM_MSG_SENDER,
                    Content = finalMessage,
                    Timestamp = DateTime.Now
                };

                ChatMessages.Add(new ChatMessageDisplayViewModel(sysMsg));
            });
        }

        private void onChatMessageReceived(ChatMessageDto messageDto)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatMessages.Add(new ChatMessageDisplayViewModel(messageDto));
            });
        }


        private void updatePlayersList(LobbyStateDto newState)
        {
            var playersToRemove = Players.Except(newState.Players).ToList();
            var playersToAdd = newState.Players.Except(Players).ToList();

            foreach (var p in playersToRemove) Players.Remove(p);
            foreach (var p in playersToAdd) Players.Add(p);
        }

        private static void initializeMatchService(LobbyStateDto newState)
        {
            var currentMatchService = App.ServiceProvider.GetService<ICurrentMatchService>();
            if (currentMatchService != null)
            {
                currentMatchService.initializeMatch(
                    newState.LobbyId,
                    newState.Players.ToList(),
                    newState.CurrentSettingsDto,
                    newState.PuzzleImagePath);
            }
        }

        private void updatePuzzleImage(LobbyStateDto newState)
        {
            if (newState.CurrentSettingsDto?.CustomPuzzleImage != null &&
                newState.CurrentSettingsDto.CustomPuzzleImage.Length > 0)
            {
                PuzzleImage = convertBytesToImageSource(newState.CurrentSettingsDto.CustomPuzzleImage);
            }
            else if (!string.IsNullOrEmpty(newState.PuzzleImagePath))
            {
                string rawPath = newState.PuzzleImagePath;
                string finalPath = rawPath.StartsWith(PATH_SEPARATOR.ToString())
                    ? rawPath
                    : PUZZLE_IMAGE_BASE_PATH + rawPath;

                try
                {
                    PuzzleImage = new BitmapImage(new Uri(finalPath, UriKind.Relative));
                }
                catch (Exception)
                {
                    PuzzleImage = getDefaultPuzzleImage();
                }
            }
            else
            {
                PuzzleImage = getDefaultPuzzleImage();
            }

            OnPropertyChanged(nameof(PuzzleImage));
        }

        private static ImageSource getDefaultPuzzleImage()
        {
            return new BitmapImage(new Uri(DEFAULT_PUZZLE_IMAGE_PATH, UriKind.Relative));
        }

        private static ImageSource convertBytesToImageSource(byte[] imageBytes)
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
            catch (Exception)
            {
                return getDefaultPuzzleImage();
            }
        }

        private async Task connectToChatAsync(string lobbyIdToConnect)
        {
            try
            {
                await chatService.connectAsync(SessionService.Username, lobbyIdToConnect);
                IsChatConnected = true;
            }
            catch (EndpointNotFoundException)
            {
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
                IsChatConnected = false;
            }
            catch (CommunicationException)
            {
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
                IsChatConnected = false;
            }
            catch (TimeoutException)
            {
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
                IsChatConnected = false;
            }
            catch (SocketException)
            {
                dialogService.showError(Lang.ChatConnectError, Lang.ErrorTitle);
                IsChatConnected = false;
            }
        }

        private async Task disconnectFromChatAsync()
        {
            if (!IsChatConnected) return;

            try
            {
                await chatService.disconnectAsync(SessionService.Username, LobbyCode);
            }
            finally
            {
                IsChatConnected = false;
            }
        }

        private async Task attemptChatReconnectAsync()
        {
            try
            {
                await disconnectFromChatAsync();
                await connectToChatAsync(LobbyCode);
            }
            catch (CommunicationException)
            {
                // ignored
            }
            catch (TimeoutException)
            {
                // ignored
            }
        }

        private async Task forceExitLobbyAsync()
        {
            unsubscribeFromServiceEvents();

            try
            {
                await disconnectFromChatAsync();
            }
            catch (Exception)
            {
                // ignored
            }

            matchmakingService.disconnect();

            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                gameWindow.IsExitConfirmed = true;
            }

            if (IsGuestUser)
            {
                SessionService.clearSession();
                windowNavigationService.openWindow<AuthenticationWindow>();
            }
            else
            {
                windowNavigationService.openWindow<MainWindow>();
            }

            windowNavigationService.closeWindow<GameWindow>();
        }


        private async Task handleKickedExitAsync()
        {
            unsubscribeFromServiceEvents();

            try
            {
                await disconnectFromChatAsync();
            }
            catch (Exception)
            {
                //ignored
            }

            matchmakingService.disconnect();

            var gameWindow = Application.Current.Windows.OfType<GameWindow>().FirstOrDefault();
            if (gameWindow != null)
            {
                gameWindow.IsExitConfirmed = true;
            }

            if (SessionService.IsGuest)
            {
                SessionService.clearSession();
                windowNavigationService.openWindow<AuthenticationWindow>();
            }
            else
            {
                windowNavigationService.openWindow<MainWindow>();
            }

            windowNavigationService.closeWindow<GameWindow>();
        }

        public async Task cleanupAsync()
        {
            if (isCleaningUp) return;
            isCleaningUp = true;

            unsubscribeFromServiceEvents();

            if (!string.IsNullOrEmpty(LobbyCode) && LobbyCode != LOBBY_CODE_JOINING)
            {
                try
                {
                    await matchmakingService.leaveLobbyAsync(SessionService.Username, LobbyCode);
                }
                catch (EndpointNotFoundException)
                {
                    // ignore
                }
                catch (CommunicationException)
                {
                    // ignore
                }
                catch (TimeoutException)
                {
                    // ignore
                }

                try
                {
                    await disconnectFromChatAsync();
                }
                catch (Exception)
                {
                    // ignore
                }
            }
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
                unsubscribeFromServiceEvents();

                Players.Clear();
                OnlineFriends.Clear();
                ChatMessages.Clear();
            }

            isDisposed = true;
        }

    }
}