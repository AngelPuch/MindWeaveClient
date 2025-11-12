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
                        string clientImagePath = pzlDto.imagePath;
                        if (!string.IsNullOrEmpty(clientImagePath) && !clientImagePath.StartsWith("/"))
                        {
                            clientImagePath = $"/Resources/Images/Puzzles/{pzlDto.imagePath}";
                        }

                        AvailablePuzzles.Add(new PuzzleDisplayInfo(
                            pzlDto.puzzleId,
                            pzlDto.name,
                            clientImagePath
                        ));
                    }
                    OnPropertyChanged(nameof(AvailablePuzzles));
                }
            }
            catch (Exception ex)
            {
                dialogService.showError(Lang.ErrorLoadingPuzzles, Lang.ErrorTitle);
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

                    UploadResultDto uploadResult = await puzzleService.uploadPuzzleImageAsync(SessionService.Username, imageBytes, fileName);

                    if (uploadResult.success)
                    {
                        var newPuzzleInfo = new PuzzleDisplayInfo(
                            uploadResult.newPuzzleId,
                            fileName,
                            openFileDialog.FileName,
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
                    dialogService.showError(Lang.ErrorUploadingImage, Lang.ErrorTitle);
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
            int? puzzleId = null;

            if (SelectedPuzzle.IsUploaded)
            {
               
                try
                {
                    puzzleBytes = File.ReadAllBytes(SelectedPuzzle.LocalFilePath);
                    puzzleId = SelectedPuzzle.PuzzleId; 
                }
                catch (Exception ex)
                {
                    dialogService.showError($"Error reading uploaded file: {ex.Message}", Lang.ErrorTitle);
                    SetBusy(false);
                    return;
                }
            }
            else
            {
                
                puzzleId = SelectedPuzzle.PuzzleId;
            }

            var settings = new LobbySettingsDto
            {
                preloadedPuzzleId = SelectedPuzzle.PuzzleId,
                customPuzzleImage = null
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
                dialogService.showError(Lang.FailedToCreateLobby, Lang.ErrorTitle);
                matchmakingService.disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeCancel()
        {
            navigationService.goBack();
        }
    }
}