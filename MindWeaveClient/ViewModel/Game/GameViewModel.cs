using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Services.Callbacks;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }
    }

    public class GameViewModel : BaseViewModel
    {
        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public ObservableCollection<PlayerScoreViewModel> PlayerScores { get; }
        public PlayerScoreViewModel MyPlayer { get; private set; }

        private int puzzleWidth;
        public int PuzzleWidth
        {
            get => puzzleWidth;
            set
            {
                puzzleWidth = value;
                OnPropertyChanged();
            }
        }

        private int puzzleHeight;
        public int PuzzleHeight
        {
            get => puzzleHeight;
            set
            {
                puzzleHeight = value;
                OnPropertyChanged();
            }
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
            PlayerScores = new ObservableCollection<PlayerScoreViewModel>();

            initializePlayerScores();

            currentMatchService.PuzzleReady += OnPuzzleReady;
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
                if (puzzleLoaded)
                {
                    Trace.WriteLine("Puzzle already loaded, skipping OnPuzzleReady");
                    return;
                }

                var puzzleDto = currentMatchService.getCurrentPuzzle();
                if (puzzleDto != null)
                {
                    loadPuzzle(puzzleDto);
                }
            });
        }

        private void initializePlayerScores()
        {
            PlayerScores.Clear();
            MyPlayer = null;

            if (currentMatchService.Players == null || !currentMatchService.Players.Any())
            {
                Trace.WriteLine("No players available yet for score initialization");
                return;
            }

            foreach (var username in currentMatchService.Players)
            {
                int tempId = SessionService.Username == username
                    ? SessionService.PlayerId
                    : -Math.Abs(username.GetHashCode() % 1000000);

                var newPlayerVm = new PlayerScoreViewModel
                {
                    PlayerId = tempId,
                    Username = username,
                    Score = 0
                };

                PlayerScores.Add(newPlayerVm);

                if (tempId == SessionService.PlayerId)
                {
                    MyPlayer = newPlayerVm;
                }
            }
            OnPropertyChanged(nameof(MyPlayer));
        }

        private void loadPuzzle(PuzzleManagerService.PuzzleDefinitionDto puzzleDto)
        {
            if (puzzleLoaded)
            {
                Trace.WriteLine("Puzzle already loaded, skipping LoadPuzzle");
                return;
            }

            SetBusy(true);
            try
            {
                Trace.WriteLine($"Loading puzzle with {puzzleDto.Pieces?.Length ?? 0} pieces");

                PuzzleWidth = puzzleDto.PuzzleWidth;
                PuzzleHeight = puzzleDto.PuzzleHeight;
                OnPropertyChanged(nameof(PuzzleWidth));
                OnPropertyChanged(nameof(PuzzleHeight));

                BitmapSource fullImage = convertBytesToBitmapSource(puzzleDto.FullImageBytes);
                if (fullImage == null)
                {
                    dialogService.showError("Error: No se pudieron decodificar los bytes de la imagen.", "Error Crítico");
                    return;
                }

                PiecesCollection.Clear();

                if (puzzleDto.Pieces == null || puzzleDto.Pieces.Length == 0)
                {
                    dialogService.showError("Error: No hay piezas en la definición del puzzle.", "Error Crítico");
                    return;
                }

                foreach (var pieceDef in puzzleDto.Pieces)
                {
                    // ✅ CORRECCIÓN: Pasar los neighbor IDs en las posiciones correctas
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
                        pieceDef.InitialY,         
                        pieceDef.TopNeighborId,    
                        pieceDef.BottomNeighborId, 
                        pieceDef.LeftNeighborId,   
                        pieceDef.RightNeighborId   
                    );
                    PiecesCollection.Add(pieceViewModel);

                    Trace.WriteLine($"Piece {pieceDef.PieceId} created with neighbors: " +
                                    $"T={pieceDef.TopNeighborId}, B={pieceDef.BottomNeighborId}, " +
                                    $"L={pieceDef.LeftNeighborId}, R={pieceDef.RightNeighborId}");
                }

                foreach (var piece in PiecesCollection)
                {
                    piece.PieceGroup = new List<PuzzlePieceViewModel> { piece };
                }

                puzzleLoaded = true;
                Trace.WriteLine($"Puzzle loaded successfully with {PiecesCollection.Count} pieces");
            }
            catch (Exception ex)
            {
                dialogService.showError($"Error al procesar el puzzle: {ex.Message}", "Error Crítico");
                Trace.TraceError($"LoadPuzzle error: {ex}");
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
                return;
            }

            try
            {
                await matchmakingService.requestPieceDragAsync(
                    currentMatchService.LobbyId,
                    piece.PieceId
                );
            }
            catch (Exception ex)
            {
                dialogService.showError(ex.Message, "Error de red");
            }
        }

        public async Task dropPiece(PuzzlePieceViewModel piece, double newX, double newY)
        {
            if (piece == null)
            {
                return;
            }

            try
            {
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
                piece.X = piece.OriginalX;
                piece.Y = piece.OriginalY;
            }
        }

        public async Task releasePiece(PuzzlePieceViewModel piece)
        {
            if (piece == null)
            {
                return;
            }

            try
            {
                await matchmakingService.requestPieceReleaseAsync(
                    currentMatchService.LobbyId,
                    piece.PieceId
                );
            }
            catch (Exception ex)
            {
                dialogService.showError(ex.Message, "Error de red");
            }
        }

        private BitmapSource convertBytesToBitmapSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }
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
                Trace.TraceError($"convertBytesToBitmapSource error: {ex}");
                return null;
            }
        }

        private void OnServerPieceDragStarted(int pieceId, int playerId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null && !piece.IsPlaced)
                {
                    bool amIDragging = (playerId == SessionService.PlayerId);
                    piece.IsHeldByOther = !amIDragging;
                }
            });
        }

        private void OnServerPiecePlaced(int pieceId, double correctX, double correctY, int scoringPlayerId, int newScore)
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
                    piece.ZIndex = -1;
                }

                var player = PlayerScores.FirstOrDefault(p => p.PlayerId == scoringPlayerId);
                if (player != null)
                {
                    player.Score = newScore;
                }
            });
        }

        private void OnServerPieceMoved(int pieceId, double newX, double newY)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var piece = PiecesCollection.FirstOrDefault(p => p.PieceId == pieceId);
                if (piece != null && !piece.IsPlaced)
                {
                    piece.X = newX;
                    piece.Y = newY;
                    piece.IsHeldByOther = false;
                }
            });
        }

        private void OnServerPieceDragReleased(int pieceId)
        {
            Application.Current.Dispatcher.Invoke(() =>
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

        public void cleanup()
        {
            currentMatchService.PuzzleReady -= OnPuzzleReady;
            MatchmakingCallbackHandler.PieceDragStartedHandler -= OnServerPieceDragStarted;
            MatchmakingCallbackHandler.PiecePlacedHandler -= OnServerPiecePlaced;
            MatchmakingCallbackHandler.PieceMovedHandler -= OnServerPieceMoved;
            MatchmakingCallbackHandler.PieceDragReleasedHandler -= OnServerPieceDragReleased;
        }
    }
}