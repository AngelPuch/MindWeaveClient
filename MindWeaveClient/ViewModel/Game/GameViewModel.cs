using MindWeaveClient.MatchmakingService;
using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MindWeaveClient.ViewModel.Puzzle;

namespace MindWeaveClient.ViewModel.Game
{
    public class PlayerScoreViewModel : BaseViewModel
    {
        public int PlayerId { get; set; }
        public string Username { get; set; }

        private int score;
        public int Score
        {
            get => score;
            set { score = value; OnPropertyChanged(); }
        }
    }

    public class GameViewModel : BaseViewModel
    {
        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly IPuzzleService puzzleService;

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public ObservableCollection<PlayerScoreViewModel> PlayerScores { get; }

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

        public GameViewModel(
            ICurrentMatchService currentMatchService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            IPuzzleService puzzleService)
        {
            this.currentMatchService = currentMatchService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.puzzleService = puzzleService;

            PiecesCollection = new ObservableCollection<PuzzlePieceViewModel>();
            PlayerScores = new ObservableCollection<PlayerScoreViewModel>();

            initializePlayerScores();

            currentMatchService.PuzzleReady += OnPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler += OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler += OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler += OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler += OnServerPieceDragReleased;
        }

        private void OnPuzzleReady()
        {
            // El evento es llamado por el hilo del callback. Usar Dispatcher.
            App.Current.Dispatcher.Invoke(() =>
            {
                var puzzleDto = currentMatchService.getCurrentPuzzle();
                if (puzzleDto != null)
                {
                    LoadPuzzle(puzzleDto);
                }
            });
        }

        private void initializePlayerScores()
        {
            PlayerScores.Clear();
            if (currentMatchService.Players == null) return;

            foreach (var username in currentMatchService.Players)
            {
                int tempId = SessionService.Username == username
                    ? SessionService.PlayerId
                    : -Math.Abs(username.GetHashCode() % 1000000);

                PlayerScores.Add(new PlayerScoreViewModel
                {
                    PlayerId = tempId,
                    Username = username,
                    Score = 0
                });
            }
        }

        private void LoadPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzleDto)
        {
            if (PiecesCollection.Any())
            {
                // Ya cargamos el puzzle, no hacer nada.
                return;
            }

            SetBusy(true); // Ocultar "cargando"
            try
            {
                PuzzleWidth = puzzleDto.puzzleWidth;
                PuzzleHeight = puzzleDto.puzzleHeight;
                OnPropertyChanged(nameof(PuzzleWidth));
                OnPropertyChanged(nameof(PuzzleHeight));

                BitmapSource fullImage = convertBytesToBitmapSource(puzzleDto.fullImageBytes);
                if (fullImage == null)
                {
                    dialogService.showError("Error: No se pudieron decodificar los bytes de la imagen.", "Error Crítico");
                    return;
                }

                PiecesCollection.Clear();

                foreach (var pieceDef in puzzleDto.pieces)
                {
                    var pieceViewModel = new PuzzlePieceViewModel(
                        fullImage,
                        pieceDef.PieceId,
                        pieceDef.SourceX,
                        pieceDef.SourceY,
                        pieceDef.Width,
                        pieceDef.Height,
                        pieceDef.CorrectX,
                        pieceDef.CorrectY,
                        pieceDef.InitialX,
                        pieceDef.InitialY,
                        pieceDef.InitialX,
                        pieceDef.InitialY
                    );
                    PiecesCollection.Add(pieceViewModel);
                }
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al procesar el puzzle: {ex.Message}", "Error Crítico");
            }
            finally
            {
                SetBusy(false);
            }
        }

        public async Task startDraggingPiece(PuzzlePieceViewModel piece)
        {
            if (piece == null || piece.IsPlaced || piece.IsHeldByOther)
                return;

            try
            {
                // Notificar al servidor que estamos arrastrando esta pieza
                await matchmakingService.requestPieceDragAsync(
                    currentMatchService.LobbyId, // Necesitamos el LobbyCode
                    piece.PieceId
                );
            }
            catch (Exception ex)
            {
                dialogService.showError(ex.Message, "Error de red");
            }
        }

        public async Task DropPiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null)
                return;

            try
            {
                // Notificar al servidor dónde soltamos la pieza
                await matchmakingService.requestPieceDropAsync(
                    currentMatchService.LobbyId,
                    piece.PieceId,
                    newX,
                    newY
                );
            }
            catch (Exception ex)
            {
                dialogService.showError(ex.Message, "Error de red");
                // Si fallamos, devolvemos la pieza a su origen
                piece.X = piece.OriginalX;
                piece.Y = piece.OriginalY;
            }
        }

        // (Opcional pero recomendado) Si el jugador suelta la pieza
        // fuera del tablero, deberíamos llamar a 'Release'
        public async Task ReleasePiece(PuzzlePieceViewModel piece)
        {
            if (piece == null)
                return;

            try
            {
                // Notificar al servidor que "cancelamos" el drag
                await matchmakingService.requestPieceReleaseAsync(
                    currentMatchService.LobbyId,
                    piece.PieceId
                );
                // El callback 'OnPieceDragReleased' se encargará de mover la pieza
                // a su 'OriginalX/Y' en todos los clientes.
            }
            catch (Exception ex)
            {
                dialogService.showError(ex.Message, "Error de red");
            }
        }


        private BitmapSource convertBytesToBitmapSource(byte[] imageBytes)
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
            catch (Exception) { return null; }
        }

        private void OnServerPieceDragStarted(int pieceId, int playerId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null && !piece.IsPlaced)
                {
                    // Comprobar si soy yo quien arrastra
                    bool amIDragging = (playerId == SessionService.PlayerId);
                    piece.IsHeldByOther = !amIDragging;
                }
            });
        }

        private void OnServerPiecePlaced(int pieceId, double correctX, double correctY, int scoringPlayerId, int newScore)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null)
                {
                    piece.X = correctX; // Posición final y correcta
                    piece.Y = correctY;
                    piece.IsPlaced = true; // ¡Snap! Esto la deshabilita
                    piece.IsHeldByOther = false;
                }

                // Actualizar puntuación
                var player = PlayerScores.FirstOrDefault(p => p.PlayerId == scoringPlayerId);
                if (player != null)
                {
                    player.Score = newScore;
                }
            });
        }

        private void OnServerPieceMoved(int pieceId, double newX, double newY)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null && !piece.IsPlaced)
                {
                    piece.X = newX; // Actualizar a la nueva posición "flotante"
                    piece.Y = newY;
                    piece.IsHeldByOther = false; // Ya no está "sostenida"
                }
            });
        }

        private void OnServerPieceDragReleased(int pieceId)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null && !piece.IsPlaced)
                {
                    piece.IsHeldByOther = false;

                    piece.X = piece.OriginalX;
                    piece.Y = piece.OriginalY;
                }
            });
        }


        public void Cleanup()
        {
            currentMatchService.PuzzleReady -= OnPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler -= OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler -= OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler -= OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler -= OnServerPieceDragReleased;
        }
    }
}