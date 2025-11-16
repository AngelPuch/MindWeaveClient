using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using MindWeaveClient.PuzzleManagerService;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel.Game
{
    public class GameViewModel : BaseViewModel
    {
        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly IPuzzleService puzzleService;

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }

        private int _puzzleWidth;
        public int PuzzleWidth
        {
            get => _puzzleWidth;
            set { _puzzleWidth = value; OnPropertyChanged(); }
        }

        private int _puzzleHeight;
        public int PuzzleHeight
        {
            get => _puzzleHeight;
            set { _puzzleHeight = value; OnPropertyChanged(); }
        }

        public ICommand SendPiecePlacedCommand { get; }

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

            SendPiecePlacedCommand = new RelayCommand(executeSendPiecePlaced);
            loadPuzzleAsync();
        }

        private async void loadPuzzleAsync()
        {
            LobbySettingsDto settings = currentMatchService.CurrentSettings;

            if (settings == null || !settings.preloadedPuzzleId.HasValue)
            {
                dialogService.showError("Error: No se pudieron cargar los ajustes de la partida.", "Error Crítico");
                return;
            }
            int puzzleId = settings.preloadedPuzzleId.Value;
            int difficultyId = settings.difficultyId;

            PuzzleManagerService.PuzzleDefinitionDto puzzleDto = null;

            try
            {
                SetBusy(true);
                puzzleDto = await puzzleService.getPuzzleDefinitionAsync(puzzleId, difficultyId);
                currentMatchService.setPuzzle(puzzleDto);
            }
            catch (Exception ex)
            {
                dialogService.showError("Error: No se pudo cargar la definición del puzzle desde el servidor.", "Error Crítico");
                return;
            }
            finally
            {
                SetBusy(false);
            }


            if (puzzleDto == null)
            {
                dialogService.showError("Error: El servidor no devolvió una definición de puzzle.", "Error Crítico");
                return;
            }



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
            Random randomGenerator = new Random();
            foreach (var pieceDef in puzzleDto.pieces)
            {
                double randomX = randomGenerator.Next(0, 300);
                double randomY = randomGenerator.Next(0, 300);

                var pieceViewModel = new PuzzlePieceViewModel(
                fullImage, pieceDef.pieceId,
                pieceDef.sourceX, pieceDef.sourceY,
                pieceDef.width, pieceDef.height,
                pieceDef.correctX, pieceDef.correctY,
                randomX, 
                randomY
        );
                PiecesCollection.Add(pieceViewModel);
            }
        }

        private async void executeSendPiecePlaced(object pieceIdObject)
        {
            if (pieceIdObject is int pieceId)
            {
                try
                {
                    await matchmakingService.sendPiecePlacedAsync(pieceId);
                }
                catch (Exception ex)
                {
                    dialogService.showError(ex.Message, "Error");
                }
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
    }
}