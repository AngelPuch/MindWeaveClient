using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.ViewModel.Puzzle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MindWeaveClient.ViewModel.Game
{
    public class PlayerScoreViewModel : BaseViewModel
    {
        public string Username { get; set; }

        private int score;
        public int Score
        {
            get => score;
            set { score = value; OnPropertyChanged(); }
        }
    }

    public class GameViewModel : BaseViewModel, IDisposable
    {
        private const double REMOTE_SNAP_THRESHOLD = 20.0;
        private const int DEFAULT_MATCH_DURATION_SECONDS = 300;

        private const double VISUAL_TIMER_INTERVAL_SECONDS = 0.5;
        private const double TIMER_ROUND_ADJUSTMENT_SECONDS = 0.9;
        private const double NOTIFICATION_DISPLAY_SECONDS = 2.0;

        private const int ZINDEX_PLACED_PIECE = -1;
        private const int ZINDEX_DRAGGING_PIECE = 100;
        private const int ZINDEX_DEFAULT_PIECE = 1;

        private const string TIMER_FORMAT = @"mm\:ss";
        private const string TIMER_ZERO_DISPLAY = "00:00";

        private const string PENALTY_REASON_HOARDING = "HOARDING";
        private const string PENALTY_REASON_MISS = "MISS";
        private const string PENALTY_REASON_WRONG_SPOT = "WRONG_SPOT";

        private const string BONUS_FIRST_BLOOD = "FIRST_BLOOD";
        private const string BONUS_STREAK = "STREAK";
        private const string BONUS_FRENZY = "FRENZY";
        private const string BONUS_LAST_HIT = "LAST_HIT";
        private const char BONUS_SEPARATOR = ',';

        private const string BONUS_MSG_FIRST_BLOOD = "🩸 FIRST BLOOD! (+25)";
        private const string BONUS_MSG_STREAK = "STREAK! (+10)";
        private const string BONUS_MSG_FRENZY = "FRENZY! (+40)";
        private const string BONUS_MSG_LAST_HIT = "LAST HIT! (+50)";

        private const string PENALTY_MSG_HOARDING_FORMAT = "Don't hoard! -{0}";
        private const string PENALTY_MSG_MISS_FORMAT = "Miss! -{0}";
        private const string PENALTY_MSG_WRONG_SPOT_FORMAT = "Wrong Piece! -{0}";
        private const string PENALTY_MSG_DEFAULT_FORMAT = "-{0}";

        private const string SOUND_SNAP = "snap_sound.mp3";
        private const string SOUND_ERROR = "error_sound.mp3";
        private const string SOUND_BONUS = "bonus.mp3";

        private const string END_REASON_TIMEOUT = "TimeOut";
        private const string END_REASON_FORFEIT = "Forfeit";

        private const string COLOR_BONUS_NOTIFICATION = "#F1C40F";
        private const string COLOR_PLAYER_BLUE = "#3498DB";
        private const string COLOR_PLAYER_RED = "#E74C3C";
        private const string COLOR_PLAYER_GREEN = "#2ECC71";
        private const string COLOR_PLAYER_YELLOW = "#F1C40F";


        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly IAudioService audioService;
        private readonly IServiceExceptionHandler exceptionHandler;

        private static Guid activeGameInstanceId;
        private readonly Guid myInstanceId;

        private readonly Dictionary<string, SolidColorBrush> playerColorsMap;
        private readonly SolidColorBrush[] definedColors;

        private DispatcherTimer visualTimer;
        private DateTime matchEndTime;
        private TimeSpan timeRemaining;
        private string timeDisplay;
        private bool isDisposed;
        private bool puzzleLoaded;

        private int puzzleWidth;
        private int puzzleHeight;
        private string bonusNotification;
        private bool isBonusVisible;
        private Brush notificationColor;
        private bool isHelpPopupVisible;
        private ImageSource targetPuzzleImage;
        
        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public ObservableCollection<PuzzleSlotViewModel> PuzzleSlots { get; }
        public ObservableCollection<PlayerScoreViewModel> PlayerScores { get; }
        public PlayerScoreViewModel MyPlayer { get; private set; }

        public string TimeDisplay
        {
            get => timeDisplay;
            set { timeDisplay = value; OnPropertyChanged(); }
        }

        public int PuzzleWidth
        {
            get => puzzleWidth;
            set { puzzleWidth = value; OnPropertyChanged(); }
        }

        public int PuzzleHeight
        {
            get => puzzleHeight;
            set { puzzleHeight = value; OnPropertyChanged(); }
        }

        public string BonusNotification
        {
            get => bonusNotification;
            set { bonusNotification = value; OnPropertyChanged(); }
        }

        public bool IsBonusVisible
        {
            get => isBonusVisible;
            set { isBonusVisible = value; OnPropertyChanged(); }
        }

        public Brush NotificationColor
        {
            get => notificationColor;
            set { notificationColor = value; OnPropertyChanged(); }
        }

        public bool IsHelpPopupVisible
        {
            get => isHelpPopupVisible;
            set { isHelpPopupVisible = value; OnPropertyChanged(); }
        }

        public ImageSource TargetPuzzleImage
        {
            get => targetPuzzleImage;
            set { targetPuzzleImage = value; OnPropertyChanged(); }
        }

        public RelayCommand ToggleHelpPopupCommand { get; }

        public event Action ForceReleaseLocalDrag;


        public GameViewModel(
            ICurrentMatchService currentMatchService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            IAudioService audioService,
            IServiceExceptionHandler exceptionHandler)
        {
            myInstanceId = Guid.NewGuid();
            activeGameInstanceId = myInstanceId;

            this.currentMatchService = currentMatchService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.audioService = audioService;
            this.exceptionHandler = exceptionHandler;

            definedColors = new[]
            {
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(COLOR_PLAYER_BLUE)),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(COLOR_PLAYER_RED)),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(COLOR_PLAYER_GREEN)),
                new SolidColorBrush((Color)ColorConverter.ConvertFromString(COLOR_PLAYER_YELLOW))
            };

            PiecesCollection = new ObservableCollection<PuzzlePieceViewModel>();
            PuzzleSlots = new ObservableCollection<PuzzleSlotViewModel>();
            PlayerScores = new ObservableCollection<PlayerScoreViewModel>();
            playerColorsMap = new Dictionary<string, SolidColorBrush>();

            ToggleHelpPopupCommand = new RelayCommand(executeToggleHelpPopup);

            initializePlayerScores();
            subscribeToEvents();
            initializeGameTimer();
            tryLoadExistingPuzzle();
        }

        public async Task startDraggingPiece(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsPlaced || piece.IsHeldByOther)
            {
                return;
            }

            try
            {
                await matchmakingService.requestPieceDragAsync(currentMatchService.LobbyId, piece.PieceId);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.DragPieceOperation);
            }
        }

        public async Task movePiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null || piece.IsHeldByOther) return;

            try
            {
                await matchmakingService.requestPieceMoveAsync(currentMatchService.LobbyId, piece.PieceId, newX, newY);
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        public async Task dropPiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null || piece.IsHeldByOther) return;

            if (string.IsNullOrEmpty(currentMatchService.LobbyId))
            {
                return;
            }

            try
            {
                await matchmakingService.requestPieceDropAsync(currentMatchService.LobbyId, piece.PieceId, newX, newY);
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.DropPieceOperation);
                resetPiecePosition(piece);
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void subscribeToEvents()
        {
            currentMatchService.PuzzleReady += onPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler += onServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler += onServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler += onServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler += onServerPieceDragReleased;
            MatchmakingCallbackHandler.PlayerPenaltyHandler += onServerPlayerPenalty;
            MatchmakingCallbackHandler.GameEndedStatic += onGameEnded;
            MatchmakingCallbackHandler.OnPlayerLeftEvent += handlePlayerLeft;
        }

        private void unsubscribeFromEvents()
        {
            if (currentMatchService != null)
            {
                currentMatchService.PuzzleReady -= onPuzzleReady;
            }

            MatchmakingCallbackHandler.PieceDragStartedHandler -= onServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler -= onServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler -= onServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler -= onServerPieceDragReleased;
            MatchmakingCallbackHandler.PlayerPenaltyHandler -= onServerPlayerPenalty;
            MatchmakingCallbackHandler.GameEndedStatic -= onGameEnded;
            MatchmakingCallbackHandler.OnPlayerLeftEvent -= handlePlayerLeft;
        }

        private void initializeGameTimer()
        {
            int duration = MatchmakingCallbackHandler.LastMatchDuration > 0
                ? MatchmakingCallbackHandler.LastMatchDuration
                : DEFAULT_MATCH_DURATION_SECONDS;

            matchEndTime = DateTime.UtcNow.AddSeconds(duration);
            updateRemainingTime();

            visualTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(VISUAL_TIMER_INTERVAL_SECONDS)
            };
            visualTimer.Tick += localTimerTick;
            visualTimer.Start();
        }

        private void initializePlayerScores()
        {
            PlayerScores.Clear();
            playerColorsMap.Clear();
            MyPlayer = null;

            if (currentMatchService.Players == null || !currentMatchService.Players.Any()) return;

            int colorIndex = 0;
            foreach (var username in currentMatchService.Players)
            {
                var newPlayerVm = new PlayerScoreViewModel
                {
                    Username = username,
                    Score = 0
                };

                PlayerScores.Add(newPlayerVm);

                playerColorsMap[username] = definedColors[colorIndex % definedColors.Length];

                if (username == SessionService.Username)
                {
                    MyPlayer = newPlayerVm;
                }

                colorIndex++;
            }

            OnPropertyChanged(nameof(MyPlayer));
        }

        private void tryLoadExistingPuzzle()
        {
            var puzzleDto = currentMatchService.getCurrentPuzzle();
            if (puzzleDto != null && !puzzleLoaded)
            {
                loadPuzzle(puzzleDto);
            }
        }
        
        private void localTimerTick(object sender, EventArgs e)
        {
            updateRemainingTime();
        }

        private void updateRemainingTime()
        {
            var remaining = matchEndTime - DateTime.UtcNow;

            if (remaining.TotalSeconds > 0)
            {
                timeRemaining = remaining;
                updateTimerDisplay();
            }
            else
            {
                timeRemaining = TimeSpan.Zero;
                visualTimer?.Stop();
                TimeDisplay = TIMER_ZERO_DISPLAY;
            }
        }

        private void updateTimerDisplay()
        {
            TimeDisplay = timeRemaining.Add(TimeSpan.FromSeconds(TIMER_ROUND_ADJUSTMENT_SECONDS)).ToString(TIMER_FORMAT);
        }

        private void onPuzzleReady()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (puzzleLoaded) return;
                var puzzleDto = currentMatchService.getCurrentPuzzle();
                if (puzzleDto != null) loadPuzzle(puzzleDto);
            });
        }

        private void loadPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzleDto)
        {
            if (puzzleLoaded) return;

            SetBusy(true);
            try
            {
                PuzzleWidth = puzzleDto.PuzzleWidth;
                PuzzleHeight = puzzleDto.PuzzleHeight;

                BitmapSource fullImage = convertBytesToBitmapSource(puzzleDto.FullImageBytes);
                if (fullImage == null) return;

                TargetPuzzleImage = fullImage;

                PiecesCollection.Clear();
                PuzzleSlots.Clear();

                if (puzzleDto.Pieces == null || puzzleDto.Pieces.Length == 0)
                {
                    dialogService.showError(Lang.ErrorNoPiecesInPuzzle, Lang.ErrorCriticalTitle);
                    return;
                }

                foreach (var pieceDef in puzzleDto.Pieces)
                {
                    var pieceViewModel = new PuzzlePieceViewModel(fullImage, pieceDef);
                    PiecesCollection.Add(pieceViewModel);

                    var slotViewModel = new PuzzleSlotViewModel(pieceDef.CorrectX, pieceDef.CorrectY, pieceDef.Width, pieceDef.Height);
                    PuzzleSlots.Add(slotViewModel);
                }

                foreach (var piece in PiecesCollection)
                {
                    piece.PieceGroup = new List<PuzzlePieceViewModel> { piece };
                }

                puzzleLoaded = true;
            }
            catch (Exception ex)
            {
                dialogService.showError(string.Format(Lang.ErrorProcessingPuzzle, ex.Message), Lang.ErrorTitle);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void handlePlayerLeft(string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var player = PlayerScores.FirstOrDefault(p => p.Username == username);
                if (player != null)
                {
                    PlayerScores.Remove(player);
                }
            });
        }

        private void onGameEnded(MatchEndResultDto result)
        {
            if (myInstanceId != activeGameInstanceId)
            {
                Dispose();
                return;
            }

            visualTimer?.Stop();

            Application.Current.Dispatcher.Invoke(() =>
            {
                string title;
                string message;

                switch (result.Reason)
                {
                    case END_REASON_TIMEOUT:
                        title = Lang.GameEndTimeoutTitle;
                        message = Lang.GameEndTimeoutMessage;
                        break;

                    case END_REASON_FORFEIT:
                        title = Lang.GameEndForfeitTitle;
                        message = Lang.GameEndForfeitMessage;
                        break;

                    default:
                        title = Lang.GameEndCompletedTitle;
                        message = Lang.GameEndCompletedMessage;
                        break;
                }

                dialogService.showInfo(string.Format(Lang.GameEndLoadingResults, message), title);

                Dispose();
                navigationService.navigateTo<PostMatchResultsPage>();
            });
        }

        private void onServerPieceDragStarted(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                bool amIDragging = username == SessionService.Username;

                if (amIDragging)
                {
                    piece.IsHeldByOther = false;
                    piece.BorderColor = Brushes.Transparent;
                }
                else
                {
                    piece.IsHeldByOther = true;
                    if (playerColorsMap.ContainsKey(username))
                    {
                        piece.BorderColor = playerColorsMap[username];
                    }
                }

                piece.ZIndex = ZINDEX_DRAGGING_PIECE;
            });
        }

        private void onServerPieceMoved(int pieceId, double newX, double newY, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                bool amIMoving = username == SessionService.Username;

                if (!amIMoving)
                {
                    piece.X = newX;
                    piece.Y = newY;
                    piece.IsHeldByOther = true;

                    if (playerColorsMap.ContainsKey(username))
                    {
                        piece.BorderColor = playerColorsMap[username];
                    }
                    piece.ZIndex = ZINDEX_DRAGGING_PIECE;
                }
            });
        }

        private void onServerPieceDragReleased(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                piece.IsHeldByOther = false;
                piece.BorderColor = Brushes.Transparent;
                piece.ZIndex = ZINDEX_DEFAULT_PIECE;

                attemptRemoteMerge(piece);

                if (username == SessionService.Username)
                {
                    ForceReleaseLocalDrag?.Invoke();
                }
            });
        }

        private void onServerPiecePlaced(int pieceId, double correctX, double correctY, string scoringUsername, int newScore, string bonusType)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);

                if (piece != null)
                {
                    piece.X = correctX;
                    piece.Y = correctY;
                    piece.IsPlaced = true;
                    piece.IsHeldByOther = false;
                    piece.BorderColor = Brushes.Transparent;
                    piece.ZIndex = ZINDEX_PLACED_PIECE;
                    audioService.playSoundEffect(SOUND_SNAP);
                }

                var player = PlayerScores.FirstOrDefault(p => p.Username == scoringUsername);
                if (player != null)
                {
                    player.Score = newScore;
                }

                if (!string.IsNullOrEmpty(bonusType))
                {
                    handleBonusEffects(scoringUsername, bonusType);
                }
            });
        }

        private void onServerPlayerPenalty(string username, int pointsLost, int newScore, string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var player = PlayerScores.FirstOrDefault(p => p.Username == username);
                if (player != null)
                {
                    player.Score = newScore;
                }

                if (username == SessionService.Username)
                {
                    string msg = getPenaltyMessage(reason, pointsLost);

                    audioService.playSoundEffect(SOUND_ERROR);
                    NotificationColor = Brushes.Red;
                    showBonusNotification(msg);
                }
            });
        }

        private void attemptRemoteMerge(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsPlaced) return;

            foreach (var potentialNeighbor in PiecesCollection)
            {
                if (potentialNeighbor == piece || potentialNeighbor.IsPlaced || potentialNeighbor.PieceGroup == piece.PieceGroup)
                    continue;

                bool isNeighbor = false;
                Point expectedPos = new Point();

                if (piece.RightNeighborId == potentialNeighbor.PieceId)
                {
                    isNeighbor = true;
                    expectedPos = new Point(piece.X + piece.Width, piece.Y);
                }
                else if (piece.LeftNeighborId == potentialNeighbor.PieceId)
                {
                    isNeighbor = true;
                    expectedPos = new Point(piece.X - potentialNeighbor.Width, piece.Y);
                }
                else if (piece.BottomNeighborId == potentialNeighbor.PieceId)
                {
                    isNeighbor = true;
                    expectedPos = new Point(piece.X, piece.Y + piece.Height);
                }
                else if (piece.TopNeighborId == potentialNeighbor.PieceId)
                {
                    isNeighbor = true;
                    expectedPos = new Point(piece.X, piece.Y - potentialNeighbor.Height);
                }

                if (isNeighbor)
                {
                    double distSquared = Math.Pow(potentialNeighbor.X - expectedPos.X, 2) +
                                         Math.Pow(potentialNeighbor.Y - expectedPos.Y, 2);

                    if (distSquared < REMOTE_SNAP_THRESHOLD * REMOTE_SNAP_THRESHOLD)
                    {
                        potentialNeighbor.X = expectedPos.X;
                        potentialNeighbor.Y = expectedPos.Y;
                        mergeGroups(piece, potentialNeighbor);
                    }
                }
            }
        }

        private static void mergeGroups(PuzzlePieceViewModel p1, PuzzlePieceViewModel p2)
        {
            var group1 = p1.PieceGroup;
            var group2 = p2.PieceGroup;

            if (group1 == group2) return;

            foreach (var p in group2)
            {
                if (!group1.Contains(p))
                {
                    group1.Add(p);
                    p.PieceGroup = group1;
                }
            }
            group2.Clear();
        }

        private static void resetPiecePosition(PuzzlePieceViewModel piece)
        {
            if (piece != null)
            {
                piece.X = piece.OriginalX;
                piece.Y = piece.OriginalY;
            }
        }

        private void handleBonusEffects(string username, string bonusType)
        {
            if (username != SessionService.Username) return;

            string message = string.Empty;
            string soundFile = string.Empty;

            var bonuses = bonusType.Split(BONUS_SEPARATOR);

            foreach (var bonus in bonuses)
            {
                switch (bonus)
                {
                    case BONUS_FIRST_BLOOD:
                        message = BONUS_MSG_FIRST_BLOOD;
                        soundFile = SOUND_BONUS;
                        break;
                    case BONUS_STREAK:
                        message = BONUS_MSG_STREAK;
                        soundFile = SOUND_BONUS;
                        break;
                    case BONUS_FRENZY:
                        message = BONUS_MSG_FRENZY;
                        soundFile = SOUND_BONUS;
                        break;
                    case BONUS_LAST_HIT:
                        message = BONUS_MSG_LAST_HIT;
                        soundFile = SOUND_BONUS;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                NotificationColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(COLOR_BONUS_NOTIFICATION));
                showBonusNotification(message);

                if (!string.IsNullOrEmpty(soundFile))
                {
                    audioService.playSoundEffect(soundFile);
                }
            }
        }

        private string getPenaltyMessage(string reason, int pointsLost)
        {
            switch (reason)
            {
                case PENALTY_REASON_HOARDING:
                    return string.Format(PENALTY_MSG_HOARDING_FORMAT, pointsLost);
                case PENALTY_REASON_MISS:
                    return string.Format(PENALTY_MSG_MISS_FORMAT, pointsLost);
                case PENALTY_REASON_WRONG_SPOT:
                    return string.Format(PENALTY_MSG_WRONG_SPOT_FORMAT, pointsLost);
                default:
                    return string.Format(PENALTY_MSG_DEFAULT_FORMAT, pointsLost);
            }
        }

        private void showBonusNotification(string message)
        {
            BonusNotification = message;
            IsBonusVisible = true;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(NOTIFICATION_DISPLAY_SECONDS)
            };
            timer.Tick += (s, e) =>
            {
                IsBonusVisible = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void executeToggleHelpPopup(object parameter)
        {
            IsHelpPopupVisible = !IsHelpPopupVisible;
        }

        private static BitmapSource convertBytesToBitmapSource(byte[] imageBytes)
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
            catch
            {
                return null;
            }
        }


        protected virtual void dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                visualTimer?.Stop();
                unsubscribeFromEvents();
            }

            isDisposed = true;
        }

    }
}