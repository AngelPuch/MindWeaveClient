using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.ViewModel.Puzzle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly IAudioService audioService;

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public ObservableCollection<PuzzleSlotViewModel> PuzzleSlots { get; }
        public ObservableCollection<PlayerScoreViewModel> PlayerScores { get; }
        public PlayerScoreViewModel MyPlayer { get; private set; }
        private readonly Dictionary<string, SolidColorBrush> playerColorsMap;

        private DispatcherTimer visualTimer;
        private TimeSpan timeRemaining;
        private string timeDisplay;

        public string TimeDisplay
        {
            get { return timeDisplay; }
            set
            {
                timeDisplay = value;
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

        private bool isDisposed = false;
        private readonly SolidColorBrush[] definedColors = new SolidColorBrush[]
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")), // Azul
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")), // Rojo
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")), // Verde
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"))  // Amarillo
        };

        private int puzzleWidth;
        public int PuzzleWidth { get => puzzleWidth; set { puzzleWidth = value; OnPropertyChanged(); } }
        private int puzzleHeight;
        public int PuzzleHeight { get => puzzleHeight; set { puzzleHeight = value; OnPropertyChanged(); } }
        private string bonusNotification;
        public string BonusNotification { get => bonusNotification; set { bonusNotification = value; OnPropertyChanged(); } }
        private bool isBonusVisible;
        public bool IsBonusVisible { get => isBonusVisible; set { isBonusVisible = value; OnPropertyChanged(); } }
        private Brush notificationColor;
        public Brush NotificationColor { get => notificationColor; set { notificationColor = value; OnPropertyChanged(); } }
        private bool isHelpPopupVisible;
        public bool IsHelpPopupVisible
        {
            get { return isHelpPopupVisible; }
            set
            {
                isHelpPopupVisible = value;
                OnPropertyChanged();
            }
        }

        private ImageSource targetPuzzleImage;
        public ImageSource TargetPuzzleImage
        {
            get { return targetPuzzleImage; }
            set
            {
                targetPuzzleImage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ToggleHelpPopupCommand { get; set; }
        private bool puzzleLoaded;

        public GameViewModel(
            ICurrentMatchService currentMatchService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            IAudioService audioService)
        {
            this.currentMatchService = currentMatchService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.audioService = audioService;

            PiecesCollection = new ObservableCollection<PuzzlePieceViewModel>();
            PuzzleSlots = new ObservableCollection<PuzzleSlotViewModel>();
            PlayerScores = new ObservableCollection<PlayerScoreViewModel>();
            playerColorsMap = new Dictionary<string, SolidColorBrush>();
            ToggleHelpPopupCommand = new RelayCommand(ExecuteToggleHelpPopup);

            initializePlayerScores();

            currentMatchService.PuzzleReady += OnPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler += OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler += OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler += OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler += OnServerPieceDragReleased;
            MatchmakingCallbackHandler.PlayerPenaltyHandler += OnServerPlayerPenalty;
            MatchmakingCallbackHandler.GameEndedStatic += OnGameEnded;

            initializeGameTimer();

            tryLoadExistingPuzzle();
        }

        private void ExecuteToggleHelpPopup(object parameter)
        {
            IsHelpPopupVisible = !IsHelpPopupVisible;
        }
        private void initializeGameTimer()
        {
            int duration = MatchmakingCallbackHandler.LastMatchDuration > 0
                           ? MatchmakingCallbackHandler.LastMatchDuration
                           : 300; 

            timeRemaining = TimeSpan.FromSeconds(duration);
            updateTimerDisplay();

            visualTimer = new DispatcherTimer();
            visualTimer.Interval = TimeSpan.FromSeconds(1);
            visualTimer.Tick += localTimerTick;
            visualTimer.Start();
        }

        private void localTimerTick(object sender, EventArgs e)
        {
            if (timeRemaining.TotalSeconds > 0)
            {
                timeRemaining = timeRemaining.Add(TimeSpan.FromSeconds(-1));
                updateTimerDisplay();
            }
            else
            {
                visualTimer.Stop();
                TimeDisplay = "00:00";
            }
        }

        private void updateTimerDisplay()
        {
            TimeDisplay = timeRemaining.ToString(@"mm\:ss");
        }

        private void OnGameEnded(MatchEndResultDto result)
        {
            visualTimer?.Stop();

            Application.Current.Dispatcher.Invoke(() =>
            {
                string reason = result.Reason;
                string title = reason == "TimeOut" ? "¡Tiempo Agotado!" : "¡Puzzle Completado!";

                string message = reason == "TimeOut"
                    ? "Se acabó el tiempo de la partida."
                    : "El rompecabezas ha sido resuelto.";

                dialogService.showInfo($"{message} Cargando resultados...", title);

                navigationService.navigateTo<PostMatchResultsPage>();
            });
        }


        private void tryLoadExistingPuzzle()
        {
            var puzzleDto = currentMatchService.getCurrentPuzzle();
            if (puzzleDto != null && !puzzleLoaded)
            {
                loadPuzzle(puzzleDto);
            }
        }

        private void OnPuzzleReady()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (puzzleLoaded) return;
                var puzzleDto = currentMatchService.getCurrentPuzzle();
                if (puzzleDto != null) loadPuzzle(puzzleDto);
            });
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

                if (colorIndex < definedColors.Length)
                    playerColorsMap[username] = definedColors[colorIndex];
                else
                    playerColorsMap[username] = definedColors[colorIndex % definedColors.Length];

                if (username == SessionService.Username)
                {
                    MyPlayer = newPlayerVm;
                }

                colorIndex++;
            }
            OnPropertyChanged(nameof(MyPlayer));
        }

        private void loadPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzleDto)
        {
            if (puzzleLoaded) return;

            SetBusy(true);
            try
            {
                PuzzleWidth = puzzleDto.PuzzleWidth;
                PuzzleHeight = puzzleDto.PuzzleHeight;
                OnPropertyChanged(nameof(PuzzleWidth));
                OnPropertyChanged(nameof(PuzzleHeight));

                BitmapSource fullImage = convertBytesToBitmapSource(puzzleDto.FullImageBytes);
                if (fullImage == null) return;
                TargetPuzzleImage = fullImage;

                PiecesCollection.Clear();
                PuzzleSlots.Clear();

                if (puzzleDto.Pieces == null || puzzleDto.Pieces.Length == 0)
                {
                    dialogService.showError("Error: No hay piezas en la definición del puzzle.", "Error Crítico");
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
                dialogService.showError($"Error al procesar puzzle: {ex.Message}", "Error");
            }
            finally
            {
                SetBusy(false);
            }
        }


        public async Task startDraggingPiece(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsPlaced || piece.IsHeldByOther)
            {
                Debug.WriteLine($"[CLIENT DRAG BLOCKED] Piece {piece?.PieceId}: IsPlaced={piece?.IsPlaced}, IsHeldByOther={piece?.IsHeldByOther}");
                return;
            }

            try
            {
                await matchmakingService.requestPieceDragAsync(currentMatchService.LobbyId, piece.PieceId);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al arrastrar pieza: {ex.Message}", "Error");
            }
        }

        public async Task movePiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null || piece.IsHeldByOther) return;

            try
            {
                await matchmakingService.requestPieceMoveAsync(currentMatchService.LobbyId, piece.PieceId, newX, newY);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error sending move request");
            }
        }

        public async Task dropPiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null || piece.IsHeldByOther) return;
            if (string.IsNullOrEmpty(currentMatchService.LobbyId))
            {
                System.Diagnostics.Debug.WriteLine("[CLIENT ERROR] Intento de DropPiece con LobbyId NULO.");
                return;
            }

            try
            {
                await matchmakingService.requestPieceDropAsync(currentMatchService.LobbyId, piece.PieceId, newX, newY);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al soltar pieza: {ex.Message}", "Error");
                piece.X = piece.OriginalX;
                piece.Y = piece.OriginalY;

            }
        }

        public async Task releasePiece(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsHeldByOther) return;
            try
            {
                await matchmakingService.requestPieceReleaseAsync(currentMatchService.LobbyId, piece.PieceId);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al liberar pieza: {ex.Message}", "Error");

            }
        }


        private void OnServerPieceDragStarted(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;
                bool amIDragging = (username == SessionService.Username);
                Debug.WriteLine($"[CLIENT DRAG STARTED] Piece {pieceId} by {username} (amI: {amIDragging})");

                if (amIDragging)
                {
                    piece.IsHeldByOther = false;
                    piece.BorderColor = Brushes.Transparent; // Sin borde para mí
                }
                else
                {
                    piece.IsHeldByOther = true;
                    if (playerColorsMap.ContainsKey(username))
                    {
                        piece.BorderColor = playerColorsMap[username];
                    }
                }

                piece.ZIndex = 100;

            });
        }


        private void OnServerPieceMoved(int pieceId, double newX, double newY, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;
                bool amIMoving = (username == SessionService.Username);
                Debug.WriteLine($"[CLIENT PIECE MOVED] Piece {pieceId} to ({newX}, {newY}) by {username} (amI: {amIMoving})");

                if (!amIMoving)
                {
                    piece.X = newX;
                    piece.Y = newY;
                    piece.IsHeldByOther = true;

                    if (playerColorsMap.ContainsKey(username))
                    {
                        piece.BorderColor = playerColorsMap[username];
                    }
                    piece.ZIndex = 100;

                }
            });
        }

        private void OnServerPieceDragReleased(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                piece.IsHeldByOther = false;
                piece.BorderColor = Brushes.Transparent;
                piece.ZIndex = 1;
            });
        }


        private void OnServerPiecePlaced(int pieceId, double correctX, double correctY, string scoringUsername, int newScore, string bonusType)
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
                    piece.ZIndex = -1;
                    audioService.playSoundEffect("snap_sound.mp3");
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

                Debug.WriteLine($"[PIECE PLACED] Piece {pieceId} at ({correctX}, {correctY}) by {scoringUsername}, new score: {newScore}");
            });
        }

        private void OnServerPlayerPenalty(string username, int pointsLost, int newScore, string reason)
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
                    string msg = "";
                    switch (reason)
                    {
                        case "HOARDING": msg = $"Don't hoard! -{pointsLost}"; break;
                        case "MISS": msg = $"Miss! -{pointsLost}"; break;
                        case "WRONG_SPOT": msg = $"Wrong Piece! -{pointsLost}"; break;
                        default: msg = $"-{pointsLost}"; break;
                    }

                    audioService.playSoundEffect("error_sound.mp3");

                    NotificationColor = Brushes.Red;
                    showBonusNotification(msg);
                }
            });
        }

        private void handleBonusEffects(string username, string bonusType)
        {
            if (username != SessionService.Username) return;

            string message = "";
            string soundFile = "";

            var bonuses = bonusType.Split(',');

            foreach (var bonus in bonuses)
            {
                switch (bonus)
                {
                    case "FIRST_BLOOD":
                        message = "🩸 FIRST BLOOD! (+25)";
                        soundFile = "bonus.mp3";
                        break;
                    case "STREAK":
                        message = "🔥 STREAK! (+10)";
                        soundFile = "bonus.mp3";
                        break;
                    case "FRENZY":
                        message = "⚡ FRENZY! (+40)";
                        soundFile = "bonus.mp3";
                        break;
                    case "LAST_HIT":
                        message = "🏆 LAST HIT! (+50)";
                        soundFile = "bonus.mp3";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                NotificationColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F")); 
                showBonusNotification(message);
                if (!string.IsNullOrEmpty(soundFile))
                    audioService.playSoundEffect(soundFile);
            }
        }

        private void showBonusNotification(string message)
        {
            BonusNotification = message;
            IsBonusVisible = true;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) =>
            {
                IsBonusVisible = false;
                timer.Stop();
            };
            timer.Start();
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
            catch { return null; }
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
                visualTimer?.Stop();
                currentMatchService.PuzzleReady -= OnPuzzleReady;
                MatchmakingCallbackHandler.PieceDragStartedHandler -= OnServerPieceDragStarted;
                MatchmakingCallbackHandler.PiecePlacedHandler -= OnServerPiecePlaced;
                MatchmakingCallbackHandler.PieceMovedHandler -= OnServerPieceMoved;
                MatchmakingCallbackHandler.PieceDragReleasedHandler -= OnServerPieceDragReleased;
                MatchmakingCallbackHandler.PlayerPenaltyHandler -= OnServerPlayerPenalty;
                MatchmakingCallbackHandler.GameEndedStatic -= OnGameEnded;
            }

            isDisposed = true;
        }

    }
}