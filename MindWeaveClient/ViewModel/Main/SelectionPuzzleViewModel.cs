using Microsoft.Win32;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Puzzle;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel.Main
{
    public class SelectionPuzzleViewModel : BaseViewModel
    {
        private readonly IPuzzleService puzzleService;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentLobbyService currentLobbyService;

        private const int MAX_IMAGE_SIZE_MB = 5;
        private const int MAX_IMAGE_SIZE_BYTES = MAX_IMAGE_SIZE_MB * 1024 * 1024;

        private ObservableCollection<PuzzleDisplayInfo> availablePuzzles = new ObservableCollection<PuzzleDisplayInfo>();
        private PuzzleDisplayInfo selectedPuzzle;

        private int selectedDifficultyIndex;

        public ObservableCollection<PuzzleDisplayInfo> AvailablePuzzles
        {
            get => availablePuzzles;
            set { availablePuzzles = value; OnPropertyChanged(); }
        }

        public PuzzleDisplayInfo SelectedPuzzle
        {
            get => selectedPuzzle;
            set
            {
                selectedPuzzle = value;
                OnPropertyChanged();
                (ConfirmAndCreateLobbyCommand as RelayCommand)?.raiseCanExecuteChanged();
            }
        }

        public int SelectedDifficultyIndex
        {
            get => selectedDifficultyIndex;
            set { selectedDifficultyIndex = value; OnPropertyChanged(); }
        }

        public ICommand LoadPuzzlesCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand ConfirmAndCreateLobbyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectPuzzleCommand { get; }

        public SelectionPuzzleViewModel(
            IPuzzleService puzzleService,
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ICurrentLobbyService currentLobbyService)
        {
            this.puzzleService = puzzleService;
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.currentLobbyService = currentLobbyService;

            SelectedDifficultyIndex = 0;

            LoadPuzzlesCommand = new RelayCommand(async p => await executeLoadPuzzlesAsync(), p => !IsBusy);
            UploadImageCommand = new RelayCommand(async p => await executeUploadImageAsync(), p => !IsBusy);
            ConfirmAndCreateLobbyCommand = new RelayCommand(async p => await executeConfirmAndCreateLobbyAsync(), p => canConfirmAndCreateLobby());
            CancelCommand = new RelayCommand(p => executeCancel(), p => !IsBusy);
            SelectPuzzleCommand = new RelayCommand(executeSelectPuzzle, p => p is PuzzleDisplayInfo);

            LoadPuzzlesCommand.Execute(null);
        }

        private bool canConfirmAndCreateLobby()
        {
            return SelectedPuzzle != null && !IsBusy;
        }

        private void executeSelectPuzzle(object parameter)
        {
            if (parameter is PuzzleDisplayInfo puzzleInfo)
            {
                if (SelectedPuzzle != null && SelectedPuzzle != puzzleInfo)
                {
                    SelectedPuzzle.IsSelected = false;
                }

                SelectedPuzzle = puzzleInfo;

                if (!SelectedPuzzle.IsSelected)
                {
                    SelectedPuzzle.IsSelected = true;
                }
            }
        }

        private async Task executeLoadPuzzlesAsync()
        {
            SetBusy(true);
            AvailablePuzzles.Clear();
            SelectedPuzzle = null;

            try
            {
                PuzzleInfoDto[] puzzlesFromServer = await puzzleService.getAvailablePuzzlesAsync();

                if (puzzlesFromServer != null)
                {
                    foreach (var pzlDto in puzzlesFromServer)
                    {
                        ImageSource puzzleImage;
                        byte[] puzzleBytes = null;

                        if (pzlDto.IsUploaded && pzlDto.ImageBytes != null)
                        {
                            puzzleImage = convertBytesToImageSource(pzlDto.ImageBytes);
                            puzzleBytes = pzlDto.ImageBytes;
                        }
                        else if (pzlDto.IsUploaded)
                        {
                            puzzleImage = new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative));
                        }
                        else
                        {
                            string clientImagePath = $"/Resources/Images/Puzzles/{pzlDto.ImagePath}";
                            puzzleImage = new BitmapImage(new Uri(clientImagePath, UriKind.Relative));
                        }


                        AvailablePuzzles.Add(new PuzzleDisplayInfo(
                            pzlDto.PuzzleId,
                            pzlDto.Name,
                            puzzleImage,
                            pzlDto.IsUploaded,
                            puzzleBytes
                        ));
                    }
                    OnPropertyChanged(nameof(AvailablePuzzles));
                }
            }
            catch (FaultException<PuzzleManagerService.ServiceFaultDto> ex)
            {
                dialogService.showError(ex.Detail.Message, Lang.ErrorTitle);
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
                handleError(Lang.ErrorLoadingPuzzles, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeUploadImageAsync()
        {
            string filePath = selectImageFile();
            if (string.IsNullOrEmpty(filePath)) return;

            SetBusy(true);
            try
            {
                byte[] imageBytes = readAndValidateImage(filePath);
                if (imageBytes == null) return;

                string fileName = Path.GetFileName(filePath);

                UploadResultDto uploadResult = await puzzleService.uploadPuzzleImageAsync(SessionService.Username, imageBytes, fileName);

                if (uploadResult.Success)
                {
                    ImageSource puzzleImage = convertBytesToImageSource(imageBytes);
                    var newPuzzleInfo = new PuzzleDisplayInfo(
                        uploadResult.NewPuzzleId,
                        fileName,
                        puzzleImage,
                        true,
                        imageBytes,
                        filePath
                    );
                    
                    AvailablePuzzles.Add(newPuzzleInfo);
                    executeSelectPuzzle(newPuzzleInfo);

                    dialogService.showInfo(uploadResult.Message, Lang.UploadSuccessful);

                }
                else
                {
                    dialogService.showWarning(uploadResult.Message, Lang.UploadFailed);
                }
            }
            catch (FaultException<PuzzleManagerService.ServiceFaultDto> ex)
            {
                dialogService.showError(ex.Detail.Message, Lang.ErrorTitle);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorUploadingImage, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeConfirmAndCreateLobbyAsync()
        {
            if (!canConfirmAndCreateLobby()) { return; }

            SetBusy(true);
            byte[] puzzleBytes = null;
            int? puzzleId = SelectedPuzzle.PuzzleId;

            if (SelectedPuzzle.IsUploaded)
            {
                puzzleBytes = SelectedPuzzle.PuzzleBytes;

                if (puzzleBytes == null || puzzleBytes.Length == 0)
                {
                    dialogService.showError(string.Format(Lang.ErrorReadingPuzzleBytes, puzzleId), Lang.ErrorTitle);
                    SetBusy(false);
                    return;
                }
            }

            var settings = new LobbySettingsDto
            {
                PreloadedPuzzleId = puzzleId,
                CustomPuzzleImage = puzzleBytes,
                DifficultyId = SelectedDifficultyIndex + 1
            };

            try
            {
                LobbyCreationResultDto result = await matchmakingService.createLobbyAsync(SessionService.Username, settings);

                if (result.Success && result.InitialLobbyState != null)
                {
                    currentLobbyService.setInitialState(result.InitialLobbyState);
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<MainWindow>();
                }
                else
                {
                    dialogService.showError(result.Message ?? Lang.FailedToCreateLobby, Lang.ErrorTitle);
                }
            }
            catch (FaultException<MatchmakingService.ServiceFaultDto> ex)
            {
                dialogService.showError(ex.Detail.Message, Lang.ErrorTitle);
                matchmakingService.disconnect();
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (CommunicationException ex)
            {
                if (ex.Message.Contains("Quota") || ex.Message.Contains("maximum message size"))
                {
                    handleError(Lang.ErrorFileTooBigForServer, ex);
                }
                else
                {
                    handleError(Lang.ErrorMsgServerOffline, ex);
                }
                matchmakingService.disconnect();
            }
            catch (Exception ex)
            {
                handleError(Lang.FailedToCreateLobby, ex);
                matchmakingService.disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private string selectImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = Lang.SelectPuzzleImageTitle
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private byte[] readAndValidateImage(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > MAX_IMAGE_SIZE_BYTES)
                {
                    dialogService.showWarning(string.Format(Lang.ErrorImageTooLarge, MAX_IMAGE_SIZE_MB), Lang.UploadFailed);
                    return null;
                }

                byte[] bytes = File.ReadAllBytes(filePath);
                return bytes;
            }
            catch (IOException ex)
            {
                handleError(Lang.ErrorReadingFile, ex);
                return null;
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorProcessingFile, ex);
                return null;
            }
        }

        private static ImageSource convertBytesToImageSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return getDefaultImage();
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
            catch (NotSupportedException)
            {
                return getDefaultImage();
            }
            catch (InvalidOperationException)
            {
                return getDefaultImage();
            }
            catch (Exception)
            {
                return getDefaultImage();
            }
        }

        private static ImageSource getDefaultImage()
        {
            try
            {
                return new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative));
            }
            catch
            {
                return null;
            }
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex?.Message ?? Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}:\n{errorDetails}", Lang.ErrorTitle);
        }

        private void executeCancel()
        {
            navigationService.goBack();
        }
    }
}