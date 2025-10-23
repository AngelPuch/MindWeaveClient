// MindWeaveClient/ViewModel/Main/SelectionPuzzleViewModel.cs
using Microsoft.Win32;
using MindWeaveClient.MatchmakingService;
// *** Using para Lang ***
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.PuzzleManagerService;
// *** Asegúrate que este namespace sea el correcto para tu referencia de servicio ***
using MindWeaveClient.PuzzleManagerService;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text; // Aunque quitamos el MessageBox, lo dejamos por si necesitas logs
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class PuzzleDisplayInfo : BaseViewModel
    {
        private bool isSelectedValue;
        public int puzzleId { get; set; }
        public string name { get; set; }
        public string imagePath { get; set; }
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
        private PuzzleManagerClient puzzleClient; // Proxy
        private MatchmakingManagerClient matchmakingClient => MatchmakingServiceClientManager.Instance.Proxy;

        private ObservableCollection<PuzzleDisplayInfo> availablePuzzlesValue = new ObservableCollection<PuzzleDisplayInfo>();
        private PuzzleDisplayInfo selectedPuzzleValue;
        private bool isBusyValue;

        public ObservableCollection<PuzzleDisplayInfo> availablePuzzles
        {
            get => availablePuzzlesValue;
            set { availablePuzzlesValue = value; OnPropertyChanged(); }
        }

        public PuzzleDisplayInfo selectedPuzzle
        {
            get => selectedPuzzleValue;
            set { selectedPuzzleValue = value; OnPropertyChanged(); ((RelayCommand)confirmAndCreateLobbyCommand).RaiseCanExecuteChanged(); }
        }

        public bool isBusy
        {
            get => isBusyValue;
            private set { SetBusy(value); }
        }

        public ICommand loadPuzzlesCommand { get; }
        public ICommand uploadImageCommand { get; }
        public ICommand confirmAndCreateLobbyCommand { get; }
        public ICommand cancelCommand { get; }
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

            loadPuzzlesCommand = new RelayCommand(async p => await executeLoadPuzzlesAsync(), p => !isBusy && puzzleClient != null);
            uploadImageCommand = new RelayCommand(async p => await executeUploadImageAsync(), p => !isBusy && puzzleClient != null);
            confirmAndCreateLobbyCommand = new RelayCommand(async p => await executeConfirmAndCreateLobbyAsync(), p => canConfirmAndCreateLobby());
            cancelCommand = new RelayCommand(p => navigateBack?.Invoke(), p => !isBusy);
            SelectPuzzleCommand = new RelayCommand(executeSelectPuzzle, p => p is PuzzleDisplayInfo);

            if (puzzleClient != null)
            {
                loadPuzzlesCommand.Execute(null);
            }
        }

        private bool canConfirmAndCreateLobby()
        {
            return selectedPuzzle != null && !isBusy && puzzleClient != null;
        }

        private void executeSelectPuzzle(object parameter)
        {
            if (parameter is PuzzleDisplayInfo puzzleInfo)
            {
                if (selectedPuzzle != null && selectedPuzzle != puzzleInfo)
                {
                    selectedPuzzle.IsSelected = false;
                }
                if (selectedPuzzle != puzzleInfo)
                {
                    selectedPuzzle = puzzleInfo;
                    if (!selectedPuzzle.IsSelected)
                    {
                        selectedPuzzle.IsSelected = true;
                    }
                    OnPropertyChanged(nameof(selectedPuzzle));
                    ((RelayCommand)confirmAndCreateLobbyCommand).RaiseCanExecuteChanged();
                }
                else if (!selectedPuzzle.IsSelected)
                {
                    selectedPuzzle.IsSelected = true;
                }
            }
        }


        private async Task executeLoadPuzzlesAsync()
        {
            if (puzzleClient == null) return;

            isBusy = true;
            availablePuzzles.Clear();
            selectedPuzzle = null;
            PuzzleInfoDto[] puzzlesFromServer = null;

            try
            {
                // --- LLAMADA REAL AL SERVIDOR ---
                puzzlesFromServer = await puzzleClient.getAvailablePuzzlesAsync();

                // --- MessageBox de Debug ELIMINADO ---

                if (puzzlesFromServer != null)
                {
                    Console.WriteLine($"Received {puzzlesFromServer.Length} puzzles from server."); // Log alternativo
                    foreach (var pzlDto in puzzlesFromServer)
                    {
                        string clientImagePath = pzlDto.imagePath;
                        // Ajustar ruta si solo viene el nombre de archivo para precargados
                        if (!string.IsNullOrEmpty(clientImagePath) && !clientImagePath.StartsWith("/"))
                        {
                            clientImagePath = $"/Resources/Images/Puzzles/{pzlDto.imagePath}";
                        }

                        availablePuzzles.Add(new PuzzleDisplayInfo
                        {
                            puzzleId = pzlDto.puzzleId,
                            name = pzlDto.name,
                            imagePath = clientImagePath
                        });
                    }
                    OnPropertyChanged(nameof(availablePuzzles));
                }
                else
                {
                    Console.WriteLine("Received NULL puzzle list from server."); // Log alternativo
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
                isBusy = false;
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
                isBusy = true;
                try
                {
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    string fileName = Path.GetFileName(openFileDialog.FileName);

                    UploadResultDto uploadResult = await puzzleClient.uploadPuzzleImageAsync(SessionService.username, imageBytes, fileName);

                    if (uploadResult.success)
                    {
                        int newPuzzleId = uploadResult.newPuzzleId;
                        var newPuzzleInfo = new PuzzleDisplayInfo
                        {
                            puzzleId = newPuzzleId,
                            name = fileName,
                            imagePath = openFileDialog.FileName
                        };
                        availablePuzzles.Add(newPuzzleInfo);
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
                    isBusy = false;
                }
            }
        }

        private async Task executeConfirmAndCreateLobbyAsync()
        {
            if (!canConfirmAndCreateLobby()) return;
            if (!MatchmakingServiceClientManager.Instance.EnsureConnected())
            {
                MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isBusy = true;

            var settings = new LobbySettingsDto
            {
                preloadedPuzzleId = selectedPuzzle.puzzleId,
                // difficultyId = 1, // ELIMINADO - Servidor pone default
                customPuzzleImage = null
            };

            try
            {
                LobbyCreationResultDto result = await matchmakingClient.createLobbyAsync(SessionService.username, settings);

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
                MatchmakingServiceClientManager.Instance.Disconnect();
            }
            finally
            {
                isBusy = false;
            }
        }

        private void RaiseCanExecuteChangedForAllCommands()
        {
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void SetBusy(bool value)
        {
            if (isBusyValue != value)
            {
                isBusyValue = value;
                OnPropertyChanged(nameof(isBusy));
                RaiseCanExecuteChangedForAllCommands();
            }
        }
    }
}