using Microsoft.Win32;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

        private ObservableCollection<PuzzleDisplayInfo> availablePuzzles = new ObservableCollection<PuzzleDisplayInfo>();
        private PuzzleDisplayInfo selectedPuzzle;

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

                        if (pzlDto.isUploaded && pzlDto.imageBytes != null)
                        {
                            puzzleImage = convertBytesToImageSource(pzlDto.imageBytes);
                            puzzleBytes = pzlDto.imageBytes; 
                        }
                        else if (pzlDto.isUploaded)
                        {
                            puzzleImage = new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative));
                        }
                        else
                        {
                            string clientImagePath = $"/Resources/Images/Puzzles/{pzlDto.imagePath}";
                            puzzleImage = new BitmapImage(new Uri(clientImagePath, UriKind.Relative));
                        }

                        
                        AvailablePuzzles.Add(new PuzzleDisplayInfo(
                            pzlDto.puzzleId,
                            pzlDto.name,
                            puzzleImage,
                            pzlDto.isUploaded,
                            puzzleBytes 
                        ));
                    }
                    OnPropertyChanged(nameof(AvailablePuzzles));
                }
            }
            catch (Exception ex)
            {
                dialogService.showError(Lang.ErrorLoadingPuzzles + ": " + ex.Message, Lang.ErrorTitle);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeUploadImageAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = Lang.SelectPuzzleImageTitle
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SetBusy(true);
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    string fileName = Path.GetFileName(openFileDialog.FileName);

                    if (imageBytes.Length > 2147483647)
                    {
                        dialogService.showError("Image file is too large.", Lang.UploadFailed);
                        SetBusy(false);
                        return;
                    }

                    UploadResultDto uploadResult = await puzzleService.uploadPuzzleImageAsync(SessionService.Username, imageBytes, fileName);

                    if(uploadResult.success)
                    {
                        ImageSource puzzleImage = convertBytesToImageSource(imageBytes);
                        var newPuzzleInfo = new PuzzleDisplayInfo(
                            uploadResult.newPuzzleId,
                            fileName,
                            puzzleImage,
                            true,
                            imageBytes,
                            openFileDialog.FileName 
                        );



                        AvailablePuzzles.Add(newPuzzleInfo);
                        executeSelectPuzzle(newPuzzleInfo);

                        dialogService.showInfo(uploadResult.message, Lang.UploadSuccessful);
                    }
                    else
                    {
                        dialogService.showWarning(uploadResult.message, Lang.UploadFailed);
                    }
                }
                catch (Exception ex)
                {
                    dialogService.showError(Lang.ErrorUploadingImage + ": " + ex.Message, Lang.ErrorTitle);
                }
                finally
                {
                    SetBusy(false);
                }
            }
        }

        private async Task executeConfirmAndCreateLobbyAsync()
        {
            if (!canConfirmAndCreateLobby())
            {
                return;
            }

            SetBusy(true);

            byte[] puzzleBytes = null;
            int? puzzleId = SelectedPuzzle.PuzzleId; 


            
            if (SelectedPuzzle.IsUploaded)
            {
                puzzleBytes = SelectedPuzzle.PuzzleBytes;

              
                if (puzzleBytes == null || puzzleBytes.Length == 0)
                {
                    dialogService.showError($"Error reading puzzle file bytes for ID {puzzleId}.", Lang.ErrorTitle);
                    SetBusy(false);
                    return;
                }
            }

            var settings = new LobbySettingsDto
            {
                preloadedPuzzleId = puzzleId,
                customPuzzleImage = puzzleBytes,
                difficultyId = 1
            };

            try
            {
                LobbyCreationResultDto result = await matchmakingService.createLobbyAsync(SessionService.Username, settings);

                if (result.success && result.initialLobbyState != null)
                {
                    Debug.WriteLine(result.initialLobbyState.lobbyId);
                    currentLobbyService.setInitialState(result.initialLobbyState);
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<MainWindow>();
                }
                else
                {
                    dialogService.showError(result.message ?? Lang.FailedToCreateLobby, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                dialogService.showError(Lang.FailedToCreateLobby + ": " + ex.Message, Lang.ErrorTitle);
                matchmakingService.disconnect();
            }
            finally
            {
                SetBusy(false);
            }
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
                return new BitmapImage(new Uri("/Resources/Images/Puzzles/puzzleDefault.png", UriKind.Relative));
            }
        }



        private void executeCancel()
        {
            navigationService.goBack();
        }
    }



}