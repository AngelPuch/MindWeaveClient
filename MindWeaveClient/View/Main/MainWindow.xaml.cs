using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MindWeaveClient.View.Game;

namespace MindWeaveClient.View.Main
{
    public partial class MainWindow : Window
    {
        private readonly ISessionCleanupService cleanupService;
        private readonly IInvitationService invitationService;

        public bool IsExitConfirmed { get; set; } = false;

        public MainWindow(
            INavigationService navigationService, 
            IInvitationService invitationService, 
            ISessionCleanupService cleanupService)
        {
            InitializeComponent();
            this.invitationService = invitationService;
            this.cleanupService = cleanupService;

            navigationService.initialize(MainFrame);
            navigationService.navigateTo<MainMenuPage>();

            this.Loaded += mainWindow_Loaded;
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            invitationService.subscribeToGlobalInvites();
        }

        private async void windowClosing(object sender, CancelEventArgs e)
        {
            if (IsExitConfirmed)
            {
                return;
            }

            bool isTransitioningToGame = Application.Current.Windows
                .OfType<GameWindow>()
                .Any();

            if (isTransitioningToGame)
            {
                return;
            }

            e.Cancel = true;

            var result = MessageBox.Show(
                Lang.GlobalExitConfirmMessage,
                Lang.GlobalExitConfirmTitle,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await cleanupService.cleanUpSessionAsync();
                IsExitConfirmed = true;
                Application.Current.Shutdown();
            }
        }
    }
}