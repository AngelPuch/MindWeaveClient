using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.ViewModel.Game; // <-- Asegúrate de tener este
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel.Game
{
    // ¡ESTA ES LA CLASE QUE "INVENTÉ"!
    public class GameViewModel : BaseViewModel
    {
        private readonly ICurrentMatchService currentMatchService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;

        public ObservableCollection<PuzzlePieceViewModel> PiecesCollection { get; }
        public int PuzzleWidth { get; set; }
        public int PuzzleHeight { get; set; }

        public ICommand SendPiecePlacedCommand { get; }

        public GameViewModel(
            ICurrentMatchService currentMatchService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService)
        {
            this.currentMatchService = currentMatchService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.PiecesCollection = new ObservableCollection<PuzzlePieceViewModel>();

            SendPiecePlacedCommand = new RelayCommand(executeSendPiecePlaced);
            LoadPuzzle();
        }

        private void LoadPuzzle()
        {
            PuzzleDefinitionDto puzzleDto = currentMatchService.getCurrentPuzzle();
            if (puzzleDto == null)
            {
                dialogService.showError("Error: No se pudo cargar la definición del puzzle.", "Error Crítico");
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
            foreach (var pieceDef in puzzleDto.pieces)
            {
                var pieceViewModel = new PuzzlePieceViewModel(
                    fullImage, pieceDef.pieceId,
                    pieceDef.sourceX, pieceDef.sourceY,
                    pieceDef.width, pieceDef.height,
                    pieceDef.correctX, pieceDef.correctY
                );
                PiecesCollection.Add(pieceViewModel);
            }
        }

        private void executeSendPiecePlaced(object pieceIdObject)
        {
            if (pieceIdObject is int pieceId)
            {
                try
                {
                    matchmakingService.sendPiecePlaced(pieceId);
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