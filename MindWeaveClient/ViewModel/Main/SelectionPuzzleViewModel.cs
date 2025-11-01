using Microsoft.Win32;
using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class PuzzleDisplayInfo : BaseViewModel
    {
        private bool isSelectedValue;
        private int puzzleIdValue;
        private string nameValue;
        private string imagePathValue;

        public int PuzzleId { get => puzzleIdValue; set { puzzleIdValue = value; OnPropertyChanged(); } }
        public string Name { get => nameValue; set { nameValue = value; OnPropertyChanged(); } }
        public string ImagePath { get => imagePathValue; set { imagePathValue = value; OnPropertyChanged(); } }
        public bool IsSelected
        {
            get => isSelectedValue;
            set { isSelectedValue = value; OnPropertyChanged(); }
        }
    }

    public class SelectionPuzzleViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;
        private PuzzleManagerClient puzzleClient;
        private MatchmakingManagerClient matchmakingClient => MatchmakingServiceClientManager.instance.proxy;

        private ObservableCollection<PuzzleDisplayInfo> availablePuzzlesValue = new ObservableCollection<PuzzleDisplayInfo>();
        private PuzzleDisplayInfo selectedPuzzleValue;
        private bool isBusyValue;

        public ObservableCollection<PuzzleDisplayInfo> AvailablePuzzles
        {
            get => availablePuzzlesValue;
            set { availablePuzzlesValue = value; OnPropertyChanged(); }
        }

        public PuzzleDisplayInfo SelectedPuzzle
        {
            get => selectedPuzzleValue;
            set { selectedPuzzleValue = value; OnPropertyChanged(); ((RelayCommand)ConfirmAndCreateLobbyCommand).RaiseCanExecuteChanged(); }
        }

        public bool IsBusy
        {
            get => isBusyValue;
            private set { setBusy(value); }
        }

        public ICommand LoadPuzzlesCommand { get; }
        public ICommand UploadImageCommand { get; }
        public ICommand ConfirmAndCreateLobbyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectPuzzleCommand { get; }

        public SelectionPuzzleViewModel(Action<Page> navigateToAction, Action navigateBackAction)
        {
            navigateTo = navigateToAction;
            navigateBack = navigateBackAction;
            try
            {
                puzzleClient = new PuzzleManagerClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create Puzzle Service client: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
                puzzleClient = null;
            }

            LoadPuzzlesCommand = new RelayCommand(async p => await executeLoadPuzzlesAsync(), p => !IsBusy && puzzleClient != null);
            UploadImageCommand = new RelayCommand(async p => await executeUploadImageAsync(), p => !IsBusy && puzzleClient != null);
            ConfirmAndCreateLobbyCommand = new RelayCommand(async p => await executeConfirmAndCreateLobbyAsync(), p => canConfirmAndCreateLobby());
            CancelCommand = new RelayCommand(p => navigateBack?.Invoke(), p => !IsBusy);
            SelectPuzzleCommand = new RelayCommand(executeSelectPuzzle, p => p is PuzzleDisplayInfo);

            if (puzzleClient != null)
            {
                LoadPuzzlesCommand.Execute(null);
            }
        }

        private bool canConfirmAndCreateLobby()
        {
            return SelectedPuzzle != null && !IsBusy && puzzleClient != null;
        }

        private void executeSelectPuzzle(object parameter)
        {
            if (parameter is PuzzleDisplayInfo puzzleInfo)
            {
                if (SelectedPuzzle != null && SelectedPuzzle != puzzleInfo)
                {
                    SelectedPuzzle.IsSelected = false;
                }
                if (SelectedPuzzle != puzzleInfo)
                {
                    SelectedPuzzle = puzzleInfo;
                    if (!SelectedPuzzle.IsSelected)
                    {
                        SelectedPuzzle.IsSelected = true;
                    }
                    OnPropertyChanged(nameof(SelectedPuzzle));
                    ((RelayCommand)ConfirmAndCreateLobbyCommand).RaiseCanExecuteChanged();
                }
                else if (!SelectedPuzzle.IsSelected)
                {
                    SelectedPuzzle.IsSelected = true;
                }
            }
        }


        private async Task executeLoadPuzzlesAsync()
        {
            if (puzzleClient == null) return;

            IsBusy = true;
            AvailablePuzzles.Clear();
            SelectedPuzzle = null;
            PuzzleInfoDto[] puzzlesFromServer;

            try
            {
                puzzlesFromServer = await puzzleClient.getAvailablePuzzlesAsync();


                if (puzzlesFromServer != null)
                {
                    Console.WriteLine($"Received {puzzlesFromServer.Length} puzzles from server."); 
                    foreach (var pzlDto in puzzlesFromServer)
                    {
                        string clientImagePath = pzlDto.imagePath;
                        if (!string.IsNullOrEmpty(clientImagePath) && !clientImagePath.StartsWith("/"))
                        {
                            clientImagePath = $"/Resources/Images/Puzzles/{pzlDto.imagePath}";
                        }

                        AvailablePuzzles.Add(new PuzzleDisplayInfo
                        {
                            PuzzleId = pzlDto.puzzleId,
                            Name = pzlDto.name,
                            ImagePath = clientImagePath
                        });
                    }
                    OnPropertyChanged(nameof(AvailablePuzzles));
                }
                else
                {
                    Console.WriteLine("Received NULL puzzle list from server."); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Lang.ErrorLoadingPuzzles}: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Inner Exception: {ex.InnerException.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task executeUploadImageAsync()
        {
            if (puzzleClient == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
                Title = Lang.SelectPuzzleImageTitle
            };

            if (openFileDialog.ShowDialog() == true)
            {
                IsBusy = true;
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    string fileName = Path.GetFileName(openFileDialog.FileName);

                    UploadResultDto uploadResult = await puzzleClient.uploadPuzzleImageAsync(SessionService.Username, imageBytes, fileName);

                    if (uploadResult.success)
                    {
                        int newPuzzleId = uploadResult.newPuzzleId;
                        var newPuzzleInfo = new PuzzleDisplayInfo
                        {
                            PuzzleId = newPuzzleId,
                            Name = fileName,
                            ImagePath = openFileDialog.FileName
                        };
                        AvailablePuzzles.Add(newPuzzleInfo);
                        executeSelectPuzzle(newPuzzleInfo);
                        MessageBox.Show(uploadResult.message, Lang.UploadSuccessful, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(uploadResult.message, Lang.UploadFailed, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Lang.ErrorUploadingImage}: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task executeConfirmAndCreateLobbyAsync()
        {
            if (!canConfirmAndCreateLobby()) return;
            if (!MatchmakingServiceClientManager.instance.EnsureConnected())
            {
                MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;

            var settings = new LobbySettingsDto
            {
                preloadedPuzzleId = SelectedPuzzle.PuzzleId,
                customPuzzleImage = null
            };

            try
            {
                LobbyCreationResultDto result = await matchmakingClient.createLobbyAsync(SessionService.Username, settings);

                if (result.success && result.initialLobbyState != null)
                {
                    var lobbyPage = new LobbyPage();
                    lobbyPage.DataContext = new LobbyViewModel(result.initialLobbyState, navigateTo, navigateBack);
                    navigateTo(lobbyPage);
                }
                else
                {
                    MessageBox.Show(result.message ?? Lang.FailedToCreateLobby, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Lang.FailedToCreateLobby}: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                MatchmakingServiceClientManager.instance.Disconnect();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void raiseCanExecuteChangedForAllCommands()
        {
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void setBusy(bool value)
        {
            if (isBusyValue != value)
            {
                isBusyValue = value;
                OnPropertyChanged(nameof(IsBusy));
                raiseCanExecuteChangedForAllCommands();
            }
        }
    }
}