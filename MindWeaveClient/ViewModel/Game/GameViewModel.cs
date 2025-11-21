using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
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

namespace MindWeaveClient.ViewModel.Game
{
    public class PlayerScoreViewModel : BaseViewModel
    {
        public string Username { get; set; } // Usamos Username como identificador principal

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

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public ObservableCollection<PuzzleSlotViewModel> PuzzleSlots { get; }
        public ObservableCollection<PlayerScoreViewModel> PlayerScores { get; }
        public PlayerScoreViewModel MyPlayer { get; private set; }

        // CAMBIO CLAVE: Mapa por Username (string) en lugar de ID (int)
        private Dictionary<string, SolidColorBrush> playerColorsMap;

        private readonly SolidColorBrush[] definedColors = new SolidColorBrush[]
        {
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")), // Azul
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")), // Rojo
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")), // Verde
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F"))  // Amarillo
        };

        private int puzzleWidth;
        public int PuzzleWidth
        {
            get => puzzleWidth;
            set { puzzleWidth = value; OnPropertyChanged(); }
        }

        private int puzzleHeight;
        public int PuzzleHeight
        {
            get => puzzleHeight;
            set { puzzleHeight = value; OnPropertyChanged(); }
        }

        private bool puzzleLoaded;

        public GameViewModel(
            ICurrentMatchService currentMatchService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService)
        {
            this.currentMatchService = currentMatchService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;

            PiecesCollection = new ObservableCollection<PuzzlePieceViewModel>();
            PuzzleSlots = new ObservableCollection<PuzzleSlotViewModel>();
            PlayerScores = new ObservableCollection<PlayerScoreViewModel>();
            playerColorsMap = new Dictionary<string, SolidColorBrush>();

            initializePlayerScores();

            currentMatchService.PuzzleReady += OnPuzzleReady;

            // Suscribirse a los eventos (Nota que ahora esperan strings para el usuario)
            MatchmakingCallbackHandler.PieceDragStartedHandler += OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler += OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler += OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler += OnServerPieceDragReleased;

            tryLoadExistingPuzzle();
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

            var sortedUsernames = currentMatchService.Players.OrderBy(u => u).ToList();
            int colorIndex = 0;

            foreach (var username in sortedUsernames)
            {
                var newPlayerVm = new PlayerScoreViewModel
                {
                    Username = username,
                    Score = 0
                };

                PlayerScores.Add(newPlayerVm);

                // Asignamos color basado en el Username
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

                PiecesCollection.Clear();
                PuzzleSlots.Clear();

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

        // --- ACCIONES DE USUARIO (Requests no cambian, el server sabe quién eres) ---

        public async Task startDraggingPiece(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsPlaced || piece.IsHeldByOther) return;

            try
            {
                await matchmakingService.requestPieceDragAsync(currentMatchService.LobbyId, piece.PieceId);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al arrastrar pieza: {ex.Message}", "Error");
            }
        }

        public async Task dropPiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null) return;
            
            try
            {
                await matchmakingService.requestPieceDropAsync(currentMatchService.LobbyId, piece.PieceId, newX, newY);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al soltar pieza: {ex.Message}", "Error");

            }
        }

        public async Task releasePiece(PuzzlePieceViewModel piece)
        {
            if (piece == null) return;
            try
            {
                await matchmakingService.requestPieceReleaseAsync(currentMatchService.LobbyId, piece.PieceId);
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al liberar pieza: {ex.Message}", "Error");

            }
        }

        // --- CALLBACKS (Ahora reciben string username) ---

        private void OnServerPieceDragStarted(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                // No permitir si ya está colocada
                if (piece.IsPlaced) return;

                bool amIDragging = (username == SessionService.Username);

                // Marcar si otro jugador la sostiene
                piece.IsHeldByOther = !amIDragging;

                // Aplicar color del jugador que arrastra
                if (playerColorsMap.ContainsKey(username))
                {
                    piece.BorderColor = playerColorsMap[username];
                    piece.ZIndex = 100;
                }

                System.Diagnostics.Debug.WriteLine($"[DRAG STARTED] Piece {pieceId} by {username} (amI: {amIDragging})");
            });
        }


        private void OnServerPieceMoved(int pieceId, double newX, double newY, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                // No actualizar si ya está colocada
                if (piece.IsPlaced) return;

                // Actualizar posición
                piece.X = newX;
                piece.Y = newY;

                bool amIDragging = (username == SessionService.Username);
                piece.IsHeldByOther = !amIDragging;

                // Mantener color del jugador
                if (playerColorsMap.ContainsKey(username))
                {
                    piece.BorderColor = playerColorsMap[username];
                    piece.ZIndex = 100;
                }

                System.Diagnostics.Debug.WriteLine($"[PIECE MOVED] Piece {pieceId} to ({newX}, {newY}) by {username}");
            });
        }

        private void OnServerPieceDragReleased(int pieceId, string username)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                // Liberar la pieza visualmente
                piece.IsHeldByOther = false;
                piece.BorderColor = Brushes.Transparent;
                piece.ZIndex = 1;

                System.Diagnostics.Debug.WriteLine($"[DRAG RELEASED] Piece {pieceId} by {username}");
            });
        }


        private void OnServerPiecePlaced(int pieceId, double correctX, double correctY, string scoringUsername, int newScore)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece == null) return;

                // Colocar pieza en posición final
                piece.X = correctX;
                piece.Y = correctY;
                piece.IsPlaced = true;
                piece.IsHeldByOther = false;
                piece.BorderColor = Brushes.Transparent;
                piece.ZIndex = -1;

                // Actualizar score del jugador
                var player = PlayerScores.FirstOrDefault(p => p.Username == scoringUsername);
                if (player != null)
                {
                    player.Score = newScore;
                }

                System.Diagnostics.Debug.WriteLine($"[PIECE PLACED] Piece {pieceId} at ({correctX}, {correctY}) by {scoringUsername}, new score: {newScore}");
            });
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
            currentMatchService.PuzzleReady -= OnPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler -= OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler -= OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler -= OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler -= OnServerPieceDragReleased;

            PiecesCollection.Clear();
            PuzzleSlots.Clear();
            PlayerScores.Clear();
        }
    }
}