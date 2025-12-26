using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Utilities.Abstractions;
using System.ComponentModel;
using System.Windows;
// Asegúrate de importar donde tienes tus páginas
using MindWeaveClient.View.Game;

namespace MindWeaveClient.View.Game
{
    public partial class GameWindow : Window
    {
        private readonly ISessionCleanupService cleanupService;
        private bool isExitConfirmed = false;
        public bool GameEndedNaturally { get; set; } = false;

        public GameWindow(
            INavigationService navigationService,
            LobbyPage startPage,
            ISessionCleanupService cleanupService)
        {
            InitializeComponent();
            this.cleanupService = cleanupService;

            navigationService.initialize(GameFrame);
            GameFrame.Content = startPage;
        }

        private async void windowClosing(object sender, CancelEventArgs e)
        {
            if (GameEndedNaturally || isExitConfirmed) return;

            e.Cancel = true;

            bool isGameInProgress = GameFrame.Content is GamePage;

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
            else
            {
                await cleanupService.cleanUpSessionAsync();
                forceShutdown();
            }
        }

        private void forceShutdown()
        {
            isExitConfirmed = true;
            Application.Current.Shutdown();
        }
    }
}