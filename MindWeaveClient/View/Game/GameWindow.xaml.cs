using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MindWeaveClient.View.Main;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private readonly ISessionCleanupService cleanupService;
        private readonly ICurrentMatchService currentMatchService;
        private readonly IDialogService dialogService;

        private bool isExitDialogShown = false;

        public bool IsExitConfirmed { get; set; } = false;
        public bool GameEndedNaturally { get; set; } = false;

        public GameWindow(
            INavigationService navigationService,
            LobbyPage startPage,
            ISessionCleanupService cleanupService,
            ICurrentMatchService currentMatchService,
            IDialogService dialogService)
        {
            InitializeComponent();
            this.cleanupService = cleanupService;
            this.currentMatchService = currentMatchService;
            this.dialogService = dialogService;
            navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;
        }

        private async void windowClosing(object sender, CancelEventArgs e)
        {
            if (GameEndedNaturally || IsExitConfirmed)
            {
                return;
            }

            if (isExitDialogShown)
            {
                e.Cancel = true;
                return;
            }

            bool isTransitioningToMain = Application.Current.Windows
                .OfType<MainWindow>()
                .Any();

            if (isTransitioningToMain)
            {
                return;
            }

            e.Cancel = true;

            bool isGameInProgress = GameFrame.Content is GamePage;
            bool isInLobby = GameFrame.Content is LobbyPage;

            if (isGameInProgress)
            {
                isExitDialogShown = true;
                bool result = dialogService.showConfirmation(
                    Lang.GameExitConfirmMessage,
                    Lang.GameExitConfirmTitle);
                isExitDialogShown = false;

                if (result)
                {
                    await cleanupService.exitGameInProcessAsync();
                    forceShutdown();
                }
            }
            else if (isInLobby)
            {
                isExitDialogShown = true;
                bool result = dialogService.showConfirmation(
                    Lang.LobbyExitConfirmMessage,
                    Lang.LobbyExitConfirmTitle);
                isExitDialogShown = false;

                if (result)
                {
                    string lobbyCode = currentMatchService.LobbyId;
                    await cleanupService.exitLobbyAsync(lobbyCode);
                    forceShutdown();
                }
            }
            else
            {
                await cleanupService.cleanUpSessionAsync();
                forceShutdown();
            }
        }

        private void forceShutdown()
        {
            IsExitConfirmed = true;
            Application.Current.Shutdown();
        }
    }
}