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

        public bool IsExitConfirmed { get; set; } = false;
        public bool GameEndedNaturally { get; set; } = false;

        public GameWindow(
            INavigationService navigationService,
            LobbyPage startPage,
            ISessionCleanupService cleanupService,
            ICurrentMatchService currentMatchService)
        {
            InitializeComponent();
            this.cleanupService = cleanupService;
            this.currentMatchService = currentMatchService;
            navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;
        }

        private async void windowClosing(object sender, CancelEventArgs e)
        {
            if (GameEndedNaturally || IsExitConfirmed) return;

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
                var result = MessageBox.Show(
                    Lang.GameExitConfirmMessage,
                    Lang.GameExitConfirmTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    await cleanupService.exitGameInProcessAsync();
                    forceShutdown();
                }
            }
            else if (isInLobby)
            {
                var result = MessageBox.Show(
                    Lang.LobbyExitConfirmMessage,
                    Lang.LobbyExitConfirmTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
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